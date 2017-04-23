using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour {

    public static GameManager instance;
    public Text infoDisplay;
    public ProjectilePool projectilePool;

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
    private float goldTimer;
    public int goldAmount;
    public float goldInterval; // in s

    public int gameTimeLimit; // in ms. Preferably 600 000 (10 min)
    public int startTime;
    public bool timeout;

    public PlayerGameState getOwnGameState() {
        return gameStates[LocalIdIndex];
    }

    public PlayerGameState getOpponentGameState() {
        return gameStates[OpponentIdIndex];
    }

    public int getTime() { // Returns a timestamp counting number of ms. Only makes sense for time intervals.
        if (PhotonNetwork.inRoom) {
            return PhotonNetwork.ServerTimestamp;
        } else {
            return (int)(Time.time * 1000);
        }
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
        // Don't auto sync scenes. We will manually synchronize to load scene when players are ready.
        PhotonNetwork.automaticallySyncScene = false;
        
        // Reduce sendrate. // Not reduced anymore
        //PhotonNetwork.sendRate = 10; // Default 20
        //PhotonNetwork.sendRateOnSerialize = 5; // Default 10

        PhotonNetwork.logLevel = Loglevel;

        projectilePool = new ProjectilePool();
    }
    
    void Start() {
        if (networked) {
            Connect();
        }
    }

    #region api

    public void Connect() {
        infoDisplay.text = "Connecting to photon...";
        // we check if we are connected or not, we join if we are, else we initiate the connection to the server.
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
        goldTimer = goldInterval;
        if (PhotonNetwork.connected) {
            photonView.RPC("SendMap", PhotonTargets.Others, (object)getOwnGameState().map.serializeNew(), LocalId);
        } else {
            // DEBUG For testing.
            Debug.Log("Testing offline. Loading scene directly");
            SceneManager.LoadScene(3);
        }
    }

    public void toggleOpponentSendMapData(bool sendData) {
        if (PhotonNetwork.connected) {
            getOpponentGameState().photonView.RPC("setSendMapData", PhotonTargets.Others, sendData);
        } else {
            // DEBUG For testing.
            getOpponentGameState().sendMapData = sendData;
        }
    }

    public void sendMonster(Monster monsterPrefab) {
        getOwnGameState().gold -= monsterPrefab.price;
        getOwnGameState().monsterGoldSpent += monsterPrefab.price;
        SoundManager.instance.PlaySendMonster();
        if (PhotonNetwork.connected) {
            getOpponentGameState().photonView.RPC("spawnMonster", PhotonTargets.Others, monsterPrefab.monsterId);
        } else {
            // DEBUG For testing.
            getOwnGameState().spawnMonster(monsterPrefab.monsterId);
        }
    }

    public void shootProjectile(ProjectileData projData) {
        try {
            PlayerGameState ownGameState = getOwnGameState();
            Monster target = ownGameState.monsterRef[projData.targetSerializeId];
            Vector2 source = ownGameState.viewMapRef.getTile(projData.startCoord.row, projData.startCoord.col).transform.position;
            Projectile proj = projectilePool.GetProjectile();
            proj.projData = projData;
            proj.target = target;
            proj.source = source;
            proj.spriteR.sprite = TowerR.getById(projData.towerId).projectileSprite;
            proj.splashR.color = TowerR.getById(projData.towerId).splashColor;
            if (!getOpponentGameState().sendMapData) {
                SoundManager.instance.PlayShoot(TowerR.getById(projData.towerId).shootingSound);
            }
            proj.Initialize();
            if (getOwnGameState().sendMapData && PhotonNetwork.connected) {
                photonView.RPC("shootViewProjectile", PhotonTargets.Others, projData.serialize());
            }
        } catch (KeyNotFoundException e) {
            Debug.LogWarning(e.StackTrace);
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
                if (scene == 3) {
                    startTime = getTime();
                }
            }
        }
    }

    [PunRPC]
    public void SendMap(byte[] serialisedMap, int playerid) {
        int index = System.Array.IndexOf(playerIds, playerid);
        gameStates[index].map = MapData.deserializeNew(serialisedMap);
        photonView.RPC("LoadWhenReady", PhotonTargets.AllViaServer, 3);
    }

    [PunRPC]
    public void shootViewProjectile(byte[] ProjectileBytes) {
        try {
            ProjectileData projData = ProjectileData.deserialize(ProjectileBytes);
            projData.startTime = getTime();
            if (projData.hitTime < projData.startTime + 100) { // Min take 100ms to hit.
                projData.hitTime = projData.startTime + 100;
            }
            projData.isView = true;
            PlayerGameState oppGameState = getOpponentGameState();
            Monster target = oppGameState.monsterRef[projData.targetSerializeId];
            Vector2 source = oppGameState.viewMapRef.getTile(projData.startCoord.row, projData.startCoord.col).transform.position;
            Projectile proj = projectilePool.GetProjectile();
            proj.projData = projData;
            proj.target = target;
            proj.source = source;
            proj.spriteR.sprite = TowerR.getById(projData.towerId).projectileSprite;
            proj.splashR.color = TowerR.getById(projData.towerId).splashColor;
            if (oppGameState.sendMapData) {
                SoundManager.instance.PlayShoot(TowerR.getById(projData.towerId).shootingSound);
            }
            proj.Initialize();
        } catch (KeyNotFoundException e) {
            Debug.LogWarning(e.StackTrace);
        }
    }

    [PunRPC]
    public void increaseGold() {
        getOwnGameState().increaseGold(goldAmount);
    }

    [PunRPC]
    public void gameOverLose(int playerId) {
        if (playerId != LocalId) {
            getOwnGameState().winner = true;
            getOpponentGameState().hp = 0; // Enforce hp = 0 if not yet updated
        } else {
            getOpponentGameState().winner = true;
            getOwnGameState().hp = 0;
        }
        PhotonNetwork.LoadLevel(4);
    }

    [PunRPC]
    public void gameOverTimeout() {
        timeout = true;
        PlayerGameState ownGS = getOwnGameState();
        PlayerGameState opponentGS = getOpponentGameState();

        if (ownGS.hp == opponentGS.hp) {
            if (ownGS.monsterGoldSpent > opponentGS.monsterGoldSpent) {
                ownGS.winner = true;
            } else if (ownGS.monsterGoldSpent < opponentGS.monsterGoldSpent) {
                opponentGS.winner = true;
            }
        } else if (ownGS.hp < opponentGS.hp) {
            opponentGS.winner = true;
        } else if (ownGS.hp > opponentGS.hp) {
            ownGS.winner = true;
        }
        PhotonNetwork.LoadLevel(4);
    }
    #endregion

    // Update is called once per frame
    void Update(){
        if (SceneManager.GetActiveScene().buildIndex != 3) { return; }
        if (PhotonNetwork.connected && !PhotonNetwork.isMasterClient) { return; }
        if (getTime() >= (startTime + gameTimeLimit)) {
            if (PhotonNetwork.connected) {
                photonView.RPC("gameOverTimeout", PhotonTargets.AllViaServer);
            } else {
                gameOverTimeout();
            }
        }
        if (goldTimer > 0) {
            goldTimer -= Time.deltaTime;
        }
        if (goldTimer <= 0) {
            if (PhotonNetwork.connected) {
                photonView.RPC("increaseGold", PhotonTargets.AllViaServer);
            } else {
                increaseGold();
            }
            goldTimer = goldInterval;
        }
    }
}
