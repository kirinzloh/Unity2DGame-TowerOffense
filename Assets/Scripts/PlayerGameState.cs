using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class PlayerGameState : Photon.PunBehaviour, IPunObservable {

    public int playerId;
    public int hp;
    public int gold;
    public MapData map;
    public ViewMap viewMapRef;
    public Dictionary<int, Monster> monsterRef;
    public int monsterCount;
    
    public bool sendMapData = false;
    public bool gameOver = false;
    public bool winner = false;
    public int monsterGoldSpent;
    
    void Awake() {
        DontDestroyOnLoad(gameObject);
        hp = 10;
        gold = 1000;
    }

    // Use this for initialization
    void Start () {
        gameObject.transform.SetParent(GameManager.instance.transform);
        monsterRef = new Dictionary<int, Monster>();
        if (PhotonNetwork.connected && photonView.isMine) {
            photonView.RPC("setPlayerId", PhotonTargets.Others, playerId);
        }
        // DEBUG MODE. DELETE WHEN DONE!
        #if UNITY_EDITOR
        if (!PhotonNetwork.connected || photonView==null) {
            byte[] mapbyte = System.IO.File.ReadAllBytes("map"+playerId+".dat");
            map = MapData.deserializeNew(mapbyte);
            Debug.Log("PlayerGameState "+playerId+" loaded map from " + System.IO.Path.GetFullPath("map" + playerId + ".dat"));
        }
        #endif
        // DEBUG END
    }

    // Update is called once per frame
    void Update () {
        if (hp <= 0 && !gameOver) {
            gameOver = true;
            if (PhotonNetwork.connected && photonView.isMine) { // DEBUG
                GameManager.instance.photonView.RPC("gameOverLose", PhotonTargets.AllViaServer, playerId);
            }
        }
    }

    public void takeDamage(int damage) {
        if (photonView == null || photonView.isMine) {
            hp -= damage;
        }
    }

    public void destroyMonster(Monster monster) {
        monsterRef.Remove(monster.serializeId);
        Destroy(monster.gameObject);
    }

    public void increaseGold(int amount) {
        gold += amount;
        int extra = 0;
        for (int i = 0; i < map.numRows; ++i) {
            for (int j = 0; j < map.numCols; ++j) {
                if (map.getTileData(i, j).towerType/10 == 7) { // integer division truncates. 70+ is the gold towers.
                    extra += TowerR.getById(map.getTileData(i, j).towerType).damage;
                }
            }
        }
        gold += extra;
    }

    #region rpcs
    [PunRPC]
    public void setPlayerId(int id) {
        this.playerId = id;
        GameManager gm = GameManager.instance;
        int index = System.Array.IndexOf(gm.playerIds, id);
        gm.gameStates[index] = this;
    }

    [PunRPC]
    public void setSendMapData(bool sendmap) {
        Debug.Log("in setsendmap data: " + sendmap); // DEBUG
        if (photonView == null || photonView.isMine) {
            Debug.Log("Setting setsendmap data: " + sendmap); // DEBUG
            sendMapData = sendmap;
        }
    }

    [PunRPC]
    public void spawnMonster(int monsterId) {
        if (photonView==null || photonView.isMine) {
            Monster monster = Instantiate(MonsterR.getById(monsterId));
            // DEBUG TO IMPLEMENT Should maintain a pool of monsters in the game state.
            monster.gameState = this;
            monster.SetPath(viewMapRef.getPath());
            monster.serializeId = ++monsterCount;
            monsterRef[monster.serializeId] = monster;
        }
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(hp);
            stream.SendNext(gold);
            stream.SendNext(monsterGoldSpent);
            stream.SendNext(sendMapData);
            if (sendMapData) {
                stream.SendNext(map.serializePlay());
                stream.SendNext(serializeMonsters());
            }
        } else {
            // Note: In NetworkingPeer.cs : NetworkingPeer.OnSerializeWrite()
            // first 3 values are always viewID, false, null. Ignore them if using ToArray or Count.
            this.hp = (int)stream.ReceiveNext();
            this.gold = (int)stream.ReceiveNext();
            this.monsterGoldSpent = (int)stream.ReceiveNext();
            this.sendMapData = (bool)stream.ReceiveNext();
            if (sendMapData) {
                map.deserializePlay((byte[])stream.ReceiveNext());
                deserializeMonsters((byte[])stream.ReceiveNext());
                viewMapRef.refreshMap();
            }
        }
    }

    byte[] serializeMonsters() {
        byte[] monsterBytes = new byte[monsterRef.Count*Monster.serialize_size + 4];
        int index = 0;
        Protocol.Serialize(monsterRef.Count, monsterBytes, ref index);
        foreach (KeyValuePair<int, Monster> pair in monsterRef) {
            pair.Value.serializeTo(monsterBytes, ref index);
        }

        // DEBUG
        string x = "";
        for (int i = 0; i < monsterBytes.Length; ++i) { x += monsterBytes[i]; }
        UnityEngine.Debug.Log(playerId + " monsterbytes out: " + x);
        // DEBUG
        return monsterBytes; // DEBUG
    }

    void deserializeMonsters(byte[] monsterBytes) {
        // DEBUG
        string x = "";
        for (int i = 0; i < monsterBytes.Length; ++i) { x += monsterBytes[i]; }
        UnityEngine.Debug.Log(playerId + " monsterbytes in: " + x);
        // DEBUG
        int index = 0;
        int numMonsters;
        Protocol.Deserialize(out numMonsters, monsterBytes, ref index);
        HashSet<int> newSerializeIds = new HashSet<int>();
        for (int i = 0; i < numMonsters; ++i) {
            int serializeId;
            Protocol.Deserialize(out serializeId, monsterBytes, ref index);
            newSerializeIds.Add(serializeId);
            int monsterId;
            Protocol.Deserialize(out monsterId, monsterBytes, ref index);
            Monster monster;
            if (monsterRef.TryGetValue(serializeId, out monster)) {
                if (monster.monsterId != monsterId) { // Destroy if wrong monster.
                    destroyMonster(monster);
                    monster = null;
                }
            }
            if (monster == null) { // No monster or wrong (destroyed) monster
                monster = Instantiate(MonsterR.getById(monsterId));
                monster.gameState = this;
                monster.SetPath(viewMapRef.getPath());
                monster.serializeId = serializeId;
                monsterRef[serializeId] = monster;
            }
            // There is now a valid monster. Update
            monster.deserializeFrom(monsterBytes, ref index);
        }
        // Remove no longer existing monsters.
        List<Monster> toDestroy = new List<Monster>();
        foreach (KeyValuePair<int, Monster> pair in monsterRef) {
            if (!newSerializeIds.Contains(pair.Key)) {
                toDestroy.Add(pair.Value);
            }
        }
        foreach (Monster m in toDestroy) {
            destroyMonster(m);
        }
    }
}
