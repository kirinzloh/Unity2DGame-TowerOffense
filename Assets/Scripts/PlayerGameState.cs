using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameState : Photon.PunBehaviour, IPunObservable {

    public int playerId;
    public int hp;
    public int gold;
    public MapData map;
    public ViewMap viewMapReference;
    
    public bool sendMapData = false;

    void Awake() {
        DontDestroyOnLoad(gameObject);
        hp = 10;
        gold = 1000;
    }

    // Use this for initialization
    void Start () {
        gameObject.transform.parent = GameManager.instance.transform;
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
        if (photonView.isMine) {
            Debug.Log("Setting setsendmap data: " + sendmap); // DEBUG
            sendMapData = sendmap;
        }
    }

    [PunRPC]
    public void spawnMonster(int monsterId) {
        //Should maintain a pool of monsters in the game state.
        //viewMapReference.spawnMonster();
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(hp);
            stream.SendNext(gold);
            stream.SendNext(sendMapData);
            if (sendMapData) {
                stream.SendNext(map.serializePlay());
            }
        } else {
            // Note: In NetworkingPeer.cs : NetworkingPeer.OnSerializeWrite()
            // first 3 values are always viewID, false, null. Ignore them if using ToArray or Count.
            this.hp = (int)stream.ReceiveNext();
            this.gold = (int)stream.ReceiveNext();
            this.sendMapData = (bool)stream.ReceiveNext();
            if (sendMapData) {
                map.deserializePlay((byte[])stream.ReceiveNext());
            }
        }
    }
}
