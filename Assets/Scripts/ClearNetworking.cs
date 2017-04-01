using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearNetworking : MonoBehaviour {

    // Use this for initialization
    void Start () {
        if (GameManager.instance != null) {
            Destroy(GameManager.instance.gameObject);
            GameManager.instance = null;
        }
        foreach (PlayerGameState gamestate in GameObject.FindObjectsOfType<PlayerGameState>()){
            Destroy(gamestate);
        }
        PhotonNetwork.Disconnect();
    }
}
