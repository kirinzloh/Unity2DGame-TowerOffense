using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameState : Photon.PunBehaviour, IPunObservable {

    public int playerId;

    private bool changed = true;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        gameObject.transform.parent = GameManager.instance.transform;
    }

    // Use this for initialization
    void Start () {
        
    }
    

    // Update is called once per frame
    void Update () {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting) {
            if (changed)
            {
                stream.SendNext(playerId);
                stream.SendNext(new byte[100]); // Experiment
                changed = false;
            }
        } else {
            this.playerId = (int)stream.ReceiveNext();
            Debug.Log("1"); // Experiment
            byte[] experi = (byte[])stream.ReceiveNext();
            Debug.Log("2");
            Debug.Log(experi); // Experiment
            Debug.Log(experi.Length); // Experiment
            Debug.Log("3");
            for (int i = 0; i < 100; ++i) {
                Debug.Log(experi[i]);
            }
        }
    }
}
