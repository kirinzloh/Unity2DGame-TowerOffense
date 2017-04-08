﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameState : Photon.PunBehaviour, IPunObservable {

    public int playerId;
    public int hp;
    public int gold;
    public MapData map;
    
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
        if (!PhotonNetwork.connected || photonView==null) {
            byte[] mapbyte = System.IO.File.ReadAllBytes("map"+playerId+".dat");
            map = MapData.deserializeNew(mapbyte);
            Debug.Log("PlayerGameState "+playerId+" loaded map from " + System.IO.Path.GetFullPath("map" + playerId + ".dat"));
        }
        // DEBUG END
    }

    // Update is called once per frame
    void Update () {
    }

    [PunRPC]
    public void setPlayerId(int id) {
        this.playerId = id;
        GameManager gm = GameManager.instance;
        int index = System.Array.IndexOf(gm.playerIds, id);
        gm.gameStates[index] = this;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(hp);
            stream.SendNext(gold);
        } else {
            this.hp = (int)stream.ReceiveNext();
            this.gold = (int)stream.ReceiveNext();
            /*Debug.Log("1"); // Experiment
            byte[] experi = (byte[])stream.ReceiveNext();
            Debug.Log("2");
            Debug.Log(experi); // Experiment
            Debug.Log(experi.Length); // Experiment
            Debug.Log("3");
            for (int i = 0; i < 100; ++i) {
                Debug.Log(experi[i]);
            }*/
        }
    }
}
