using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

//To do - make prefab for player and change its model by loading it in resources.

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
    //public List<string> BankruptPlayersIDList = new List<string>();
    public List<string> DisconnectedPlayersIDList = new List<string>();


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
    
    //Summary: returns a string that reflects how a player's name will be shown to that client.
    //Returns "You" for a local player - 
    //Returns player's name for a non local player.
    public string GetPlayerIdentityDisplay(string userID) => IsUserIDLocalPlayers(userID) ? "You" : PlayersNameDictionary[userID];
    public string GetPlayerIdentityDisplayPronoun(string userID) => IsUserIDLocalPlayers(userID) ? "Your" : PlayersNameDictionary[userID];

    public static event Action AllPlayersSpawnedEvent;
    public static event Action<string, List<string>> PlayerWonGameEvent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        PlayerTurnManager.OneRemainingPlayerEvent += OnOneRemainingPlayerLeftActiveInGame;
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                TileOwnershipManager.Instance.TransferAllPlayerOwnedTilesToAnotherPlayer(PhotonNetwork.LocalPlayer.UserId, ActivePlayersIDList[1],true);
            }
        }
    }

    private void OnOneRemainingPlayerLeftActiveInGame(string playerIDThatWonGame, List<string> remainingPlayerIDList)
    {
        //RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        //PhotonNetwork.RaiseEvent(EventCodes.PlayerWonGameEvent, playerIDThatWonGame, raiseEventOptions, SendOptions.SendReliable);

        print("Game manager reacting to player winning!");

        //string remainingPlayerNamesString = string.Empty;
        //for (int i = 0; i < remainingPlayerIDList.Count; i++)
        //{
        //    remainingPlayerNamesString += GetPlayerNicknameFromID(remainingPlayerIDList[i]) + " ";
        //}

        //print($"{GetPlayerNicknameFromID(playerIDThatWonGame)} won game. Remaining players: {remainingPlayerNamesString}");

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

        //print($"Detected spawned player! {spawnedPlayer}  {viewID}");
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

        //myPlayerPiece = player.GetComponent<Player_Piece>();
        //PieceMover.Instance.SetPlayerPositionStartOfGame(player);

        int viewID = spawnedPlayer.GetComponent<PhotonView>().ViewID;
        photonView.RPC(nameof(OnPlayerSpawned), RpcTarget.All, viewID);
    }

    private void OnDestroy()
    {
        PlayerTurnManager.OneRemainingPlayerEvent -= OnOneRemainingPlayerLeftActiveInGame;
    }
}





//OLD AWAKE

//player = PhotonNetwork.Instantiate($"Pieces/PlayerPiece", Vector3.zero, Quaternion.identity);
//player.name = PhotonNetwork.LocalPlayer.NickName;

//playerNameList.Add(PhotonNetwork.LocalPlayer.NickName);


//for (int i = 0; i < GameDataSlinger.NUM_PLAYERS; i++)
//{
//    string playerName = $"Player {i + 1}";
//    PlayerData playerData = new PlayerData(playerName);
//    playerNameList.Add(playerName);
//    PlayerIDs.Add(playerData.ID);
//    PlayerNameDict.Add(playerData.ID, playerName);
//    GameObject spawnedPlayer = PhotonNetwork.Instantiate($"Pieces/{playerName}", Vector3.zero, Quaternion.identity);
//    Player_Piece spawnedPiece = Instantiate(Resources.Load<Player_Piece>($"Pieces/{playerName}"), Vector3.zero, Quaternion.identity);
//    spawnedPiece.RecievePlayerData(playerData);
//    spawnedPiece.GetComponent<PlayerMoneyAccount>().PlayerID = playerData.ID;
//    spawnedPlayerDict.Add(playerData.ID, spawnedPiece);
//}

//NumActivePlayers = PlayerIDs.Count;