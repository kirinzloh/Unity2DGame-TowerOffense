using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameState : Photon.PunBehaviour, IPunObservable {

    public int playerId;
    public int hp;
    public int gold;
    public MapData map;
    public ViewMap viewMapRef;
    public Dictionary<int, Monster> monsterRef;
    public int monsterCount;
    
    public bool sendMapData = false;

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
        serializeMonsters(); // Debug
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
            this.sendMapData = (bool)stream.ReceiveNext();
            if (sendMapData) {
                map.deserializePlay((byte[])stream.ReceiveNext());
                viewMapRef.refreshMap();
            }
        }
    }

    byte[] serializeMonsters() {
        byte[] monsterBytes = new byte[monsterRef.Count*Monster.serialize_size];
        int index = 0;
        foreach (KeyValuePair<int, Monster> pair in monsterRef) {
            pair.Value.serializeTo(monsterBytes, ref index);
        }

        // DEBUG
        string x = "";
        for (int i = 0; i < monsterBytes.Length; ++i) { x += monsterBytes[i]; }
        UnityEngine.Debug.Log(x);
        // DEBUG
        return monsterBytes; // DEBUG
    }

    void deserializeMonsters(byte[] monsterBytes) {
        
    }
}
