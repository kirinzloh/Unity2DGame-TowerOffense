using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour {

    public static GameManager instance;
    public Text infoDisplay;

    // Client version number.
    string _gameVersion = "1";
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
    public bool networked = true;

    public int LocalId;
    public int LocalIdIndex;
    public int OpponentIdIndex;
    public int[] playerIds;
    public PlayerGameState[] gameStates;

    private int sceneLatch;

    public PlayerGameState getOwnGameState() {
        return gameStates[LocalIdIndex];
    }

    public PlayerGameState getOpponentGameState() {
        return gameStates[OpponentIdIndex];
    }

    // Called on startup/construction
    void Awake() {
        // For singleton
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        // Don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;
        // Don't auto sync scenes. Ignore-->Makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = false;
        
        // Reduce sendrate.
        PhotonNetwork.sendRate = 10; // Default 20
        PhotonNetwork.sendRateOnSerialize = 5; // Default 10

        PhotonNetwork.logLevel = Loglevel;
    }
    
    void Start() {
        if (networked) {
            Connect();
        }
    }

    #region api

    public void Connect() {
        infoDisplay.text = "Connecting to photon...";
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.connected) {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        } else {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public void Cancel() {
        SceneManager.LoadScene(0);
    }

    public void buildReady() {
        // For testing.
        if (PhotonNetwork.connected) {
            photonView.RPC("SendMap", PhotonTargets.Others, (object)getOwnGameState().map.serializeNew(), LocalId);
        } else {
            Debug.Log("Testing offline. Loading scene directly");
            SceneManager.LoadScene(3);
        }
    }

    public void toggleOpponentSendMapData(bool sendData) {
        if (PhotonNetwork.connected) {
            getOpponentGameState().photonView.RPC("setSendMapData", PhotonTargets.Others, sendData);
        } else {
            getOpponentGameState().sendMapData = sendData;
        }
    }

    public void SendMonsterToOpponenet(Monster monsterPrefab) {
        if (PhotonNetwork.connected) {
            getOpponentGameState().photonView.RPC("spawnMonster", PhotonTargets.Others, monsterPrefab.monsterId);
        } else {
            getOpponentGameState().spawnMonster(monsterPrefab.monsterId);
        }
    }
    #endregion

    #region networking callbacks
    public override void OnConnectedToMaster() {
        infoDisplay.text = "Connected, joining room...";
        Debug.Log("GameManager: OnConnectedToMaster() was called by PUN");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
        Debug.Log("GameManager:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so create one.");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom() {
        infoDisplay.text = "Joined room, waiting for another player to join...";
        Debug.Log("GameManager: OnJoinedRoom() called by PUN. Now this client is in a room.");
        LocalId = PhotonNetwork.player.ID;
        sceneLatch = 2;
        if (PhotonNetwork.room.PlayerCount == 2) {
            photonView.RPC("StartGame", PhotonTargets.All);
        }
    }

    public override void OnDisconnectedFromPhoton() {
        Debug.LogWarning("GameManager: OnDisconnectedFromPhoton() was called by PUN");
        SceneManager.LoadScene(0);
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
        Debug.LogWarning("GameManager: OnPhotonPlayerDisconnected() was called by PUN");
        SceneManager.LoadScene(0);
    }

    #endregion

    #region rpcs

    [PunRPC]
    public void StartGame() {
        infoDisplay.text = "Starting game...";
        Debug.Log("GameManager: Starting Game...");
        PhotonPlayer[] players = PhotonNetwork.playerList;
        playerIds = new int[2];
        for (int i = 0; i < players.Length; ++i) {
            playerIds[i] = players[i].ID;
        }
        System.Array.Sort(playerIds);
        LocalIdIndex = System.Array.IndexOf(playerIds, LocalId);
        // Hardcode Opponent ID reference. Need more general design for multi opponent (which is not in the plan, so this is ok).
        if (LocalIdIndex == 0) {
            OpponentIdIndex = 1;
        } else {
            OpponentIdIndex = 0;
        }
        GameObject gs = PhotonNetwork.Instantiate("GameState", new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
        gameStates[LocalIdIndex] = gs.GetComponent<PlayerGameState>();
        gameStates[LocalIdIndex].playerId = LocalId;
        photonView.RPC("LoadWhenReady", PhotonTargets.AllViaServer, 2);
    }

    [PunRPC]
    public void LoadWhenReady(int scene) {
        lock (this) {
            sceneLatch -= 1;
            if (sceneLatch <= 0) {
                PhotonNetwork.LoadLevel(scene);
                sceneLatch = 2;
            }
        }
    }

    [PunRPC]
    public void SendMap(byte[] serialisedMap, int playerid) {
        int index = System.Array.IndexOf(playerIds, playerid);
        gameStates[index].map = MapData.deserializeNew(serialisedMap);
        photonView.RPC("LoadWhenReady", PhotonTargets.AllViaServer, 3);
    }
    #endregion

    // Update is called once per frame
    void Update(){
    }
}
