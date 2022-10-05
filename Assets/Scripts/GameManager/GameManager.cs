using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

//Game manager holds data about the current players in the game - their ids, names, pieces etc.
//Game manager makes the master client spawn in each player piece.

//Various things in this script should be moved to seperate scripts eventually.
//When a player disconnects, their data is not fully taken care of at the moment. This will be worked on in the future.

public class GameManager : MonoBehaviourPunCallbacks
{
    //Singleton.
    public static GameManager Instance;

    public Board board;

    public GameObject spawnedPlayer;
    public Player_Piece myPlayerPiece;
    public List<GameObject> spawnedPlayerList;

    public Dictionary<string, GameObject> SpawnedPlayersDictionary = new Dictionary<string, GameObject>();//ID - spawned gameobject.
    public Dictionary<string, string> PlayersNameDictionary = new Dictionary<string, string>(); //ID - Name.
    public Dictionary<string, Player_Piece> PlayersPieceDictionary = new Dictionary<string, Player_Piece>(); //ID - Player Piece.

    //Active players.
    //Players that are not bankrupt.
    public List<string> ActivePlayersIDList = new List<string>();
    public bool LocalPlayerIsAnActivePlayer { get { return ActivePlayersIDList.Contains(PhotonNetwork.LocalPlayer.UserId); } }


    public List<string> DisconnectedPlayersIDList = new List<string>();

    [SerializeField] private Sprite[] playerSprites;
    [SerializeField] private Dictionary<string, Sprite> playerSpriteDictionary = new Dictionary<string, Sprite>();
    public Sprite GetPlayerSprite(string playerID) => playerSpriteDictionary[playerID];

    public List<string> GetActivePlayersIgnoringID(string playerIDToIgnore)
    {
        List<string> activePlayerIDs = new List<string>(ActivePlayersIDList);
        activePlayerIDs.Remove(playerIDToIgnore);
        return activePlayerIDs;
    }

    public int NumActivePlayers { get { return ActivePlayersIDList.Count; } }

    public string GetPlayerNicknameFromID(string ID) => PlayersNameDictionary[ID];
    public string LocalPlayerNickname => PlayersNameDictionary[PhotonNetwork.LocalPlayer.UserId];
    private bool IsUserIDLocalPlayers(string userID) => userID == PhotonNetwork.LocalPlayer.UserId;
    
    public string GetPlayerIdentityDisplay(string userID) => IsUserIDLocalPlayers(userID) ? "You" : PlayersNameDictionary[userID];

    public static event Action AllPlayersSpawnedEvent;
    public static event Action<string, List<string>> PlayerWonGameEvent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        PlayerTurnManager.OneRemainingPlayerEvent += OnOneRemainingPlayerLeftActiveInGame;
        Bank.PlayerDeclaredBankruptDueToBankPaymentEvent += OnPlayerDeclaredBankruptDueToBank;
        Bank.PlayerDeclaredBankruptDueToPlayerPaymentEvent += OnPlayerDeclaredBankruptDueToPlayer;
    }

    private void OnPlayerDeclaredBankruptDueToPlayer(string playerIDBankrupt, string playerIDThatCausedBankrupcy) => photonView.RPC(nameof(RemovePlayerFromActivePlayers),RpcTarget.All,playerIDBankrupt);
    private void OnPlayerDeclaredBankruptDueToBank(string playerIDBankrupt) => photonView.RPC(nameof(RemovePlayerFromActivePlayers), RpcTarget.All, playerIDBankrupt);

    [PunRPC]
    private void RemovePlayerFromActivePlayers(string playerID)
    {
        ActivePlayersIDList.Remove(playerID);
    }

    private void OnOneRemainingPlayerLeftActiveInGame(string playerIDThatWonGame, List<string> remainingPlayerIDList)
    {
        PlayerWonGameEvent?.Invoke(playerIDThatWonGame, remainingPlayerIDList);
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        //Call to each player to spawn their piece.
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            //Spawn.
            spawnedPlayer = PhotonNetwork.Instantiate($"Pieces/PlayerPiece", Vector3.zero, Quaternion.identity);

            //Notify all clients of spawned player.
            PhotonView playerPhotonView = spawnedPlayer.GetComponent<PhotonView>();
            photonView.RPC(nameof(OnPlayerSpawned), RpcTarget.All, playerPhotonView.ViewID);

            //Move spawned player to starting tile.
            PieceMover.Instance.SetPlayerPositionStartOfGame(spawnedPlayer);

            //Transfer ownership of spawned player from the master client to the actual player.
            playerPhotonView.TransferOwnership(players[i]);
        }
    }

    public Player_Piece GetPlayerPieceByID(string playerID) => PlayersPieceDictionary[playerID];
    public Player_Piece GetLocalPlayerPiece() => PlayersPieceDictionary[PhotonNetwork.LocalPlayer.UserId];


    [PunRPC] //When a player is spawned - add to list.
    private void OnPlayerSpawned(int viewID)
    {
        PhotonView photonViewOfSpawnedPlayer = PhotonNetwork.GetPhotonView(viewID);
        GameObject spawnedPlayer = photonViewOfSpawnedPlayer.gameObject;

        spawnedPlayerList.Add(spawnedPlayer);

        if (spawnedPlayerList.Count == PhotonNetwork.PlayerList.Length)
            OnRecievingAllSpawnedPlayers();
    }
    private void OnRecievingAllSpawnedPlayers()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            SpawnedPlayersDictionary.Add(players[i].UserId, spawnedPlayerList[i]);
            PlayersNameDictionary.Add(players[i].UserId, players[i].NickName);

            Player_Piece spawnedPlayerPiece = spawnedPlayerList[i].GetComponent<Player_Piece>();
            spawnedPlayerPiece.RecievePlayerData(players[i]);
            PlayersPieceDictionary.Add(players[i].UserId, spawnedPlayerPiece);

            ActivePlayersIDList.Add(players[i].UserId);
            playerSpriteDictionary.Add(players[i].UserId, playerSprites[i]);
        }

        AllPlayersSpawnedEvent?.Invoke();
    }


    //Not tested.
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Display notification?
        ActivePlayersIDList.Remove(otherPlayer.UserId);

        //SpawnedPlayersDictionary.Remove(otherPlayer.UserId);
        //PlayersNameDictionary.Remove(otherPlayer.UserId);
        //PlayersPieceDictionary.Remove(otherPlayer.UserId);

        //Improve this?
        if (otherPlayer.IsMasterClient)
        {
            PhotonNetwork.SetMasterClient(PhotonNetwork.PlayerList[0]);
        }

        DisconnectedPlayersIDList.Add(otherPlayer.UserId);
    }

    [PunRPC] //Spawn a player and set their position at start of board. Then notify master client that they were spawned.
    private void SpawnPlayer()
    {
        spawnedPlayer = PhotonNetwork.Instantiate($"Pieces/PlayerPiece", Vector3.zero, Quaternion.identity);

        int viewID = spawnedPlayer.GetComponent<PhotonView>().ViewID;
        photonView.RPC(nameof(OnPlayerSpawned), RpcTarget.All, viewID);
    }

    private void OnDestroy()
    {
        PlayerTurnManager.OneRemainingPlayerEvent -= OnOneRemainingPlayerLeftActiveInGame;
        Bank.PlayerDeclaredBankruptDueToBankPaymentEvent -= OnPlayerDeclaredBankruptDueToBank;
        Bank.PlayerDeclaredBankruptDueToPlayerPaymentEvent -= OnPlayerDeclaredBankruptDueToPlayer;
    }
}