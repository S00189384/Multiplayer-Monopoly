using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileOwnershipManager : MonoBehaviourPun
{
    //Singleton.
    public static TileOwnershipManager Instance;

    public Board board;

    //Each players owned tile trackers.
    private Dictionary<string, OwnedPlayerTileTracker> PlayersOwnedTileTrackerDictionary = new Dictionary<string, OwnedPlayerTileTracker>();
    public List<OwnedPlayerTileTracker> DebugList = new List<OwnedPlayerTileTracker>();
    public OwnedPlayerTileTracker GetOwnedPlayerTileTracker(string playerID) => PlayersOwnedTileTrackerDictionary[playerID];
    public OwnedPlayerTileTracker GetLocalPlayersOwnedTileTracker { get { return PlayersOwnedTileTrackerDictionary[PhotonNetwork.LocalPlayer.UserId]; } }

    //Type of purchasable tile from its photon ID. Move to game manager / board instead?
    private Dictionary<int,Type> photonIDPropertyTypeDictionary = new Dictionary<int, Type>();
    public Type GetPurchasableTileTypeFromPhotonID(int photonIDOfTile) => photonIDPropertyTypeDictionary[photonIDOfTile];

    //Which process ownership method to call based on the type of purchasable tile.
    private Dictionary<Type, Action<string,int>> processTilePurchaseBasedOnTypeDictionary = new Dictionary<Type, Action<string, int>>();
    private Dictionary<Type, Action<string,int>> processTileRemovalBasedOnTypeDictionary = new Dictionary<Type, Action<string, int>>();


    public int GetNumberOfPropertyTypeOnBoard(PropertyColourSet propertyColour)
    {
        return board.propertyTypeInstanceCountDictionary[propertyColour];
    }

    public bool PlayerOwnsAllOfPropertyType(OwnedPlayerTileTracker playerOwnedTileTracker, PropertyColourSet propertyColour)
    {
        return playerOwnedTileTracker.GetNumberOfOwnedPropertyType(propertyColour) == board.propertyTypeInstanceCountDictionary[propertyColour];
    }

    #region Start / Awake.
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;
    }
    private void Start()
    {
        for (int i = 0; i < board.boardTiles.Count; i++)
        {
            TileInstance_Purchasable tileInstance_Purchasable;
            board.boardTiles[i].TryGetComponent(out tileInstance_Purchasable);
            if (tileInstance_Purchasable != null)
            {
                photonIDPropertyTypeDictionary.Add(tileInstance_Purchasable.GetComponent<PhotonView>().ViewID, tileInstance_Purchasable.GetType());
            }
        }

        processTilePurchaseBasedOnTypeDictionary = new Dictionary<Type, Action<string, int>>()
        {
            {typeof(TileInstance_Property), ProcessPlayerPropertyPurchase },
            {typeof(TileInstance_Station), ProcessPlayerStationPurchase },
            {typeof(TileInstance_Utility), ProcessPlayerUtilityPurchase },       
        };

        processTileRemovalBasedOnTypeDictionary = new Dictionary<Type, Action<string, int>>()
        {
            {typeof(TileInstance_Property), ProcessPlayerRemovingTile },
        };
    }
    #endregion

    public void RecieveBoard(Board board) => this.board = board;
    private void OnAllPlayersSpawned()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            PlayersOwnedTileTrackerDictionary.Add(players[i].UserId, GameManager.Instance.GetPlayerPieceByID(players[i].UserId).GetComponent<OwnedPlayerTileTracker>());
            DebugList.Add(GameManager.Instance.GetPlayerPieceByID(players[i].UserId).GetComponent<OwnedPlayerTileTracker>());
        }
    }

    #region Processing player acquiring a property
    public void ProcessPlayerTilePurchase(string playerID,int tilePhotonID)
    {
        photonView.RPC(nameof(ProcessPlayerTilePurchaseRPC), RpcTarget.All, playerID, tilePhotonID);
    }
    [PunRPC]
    private void ProcessPlayerTilePurchaseRPC(string playerID, int tilePhotonID)
    {
        Type typeOfTile = photonIDPropertyTypeDictionary[tilePhotonID];
        processTilePurchaseBasedOnTypeDictionary[typeOfTile].Invoke(playerID, tilePhotonID);
    }
    private void ProcessPlayerPropertyPurchase(string playerID,int photonIDOfTile)
    {
        TileInstance_Property propertyInstance = PhotonNetwork.GetPhotonView(photonIDOfTile).GetComponent<TileInstance_Property>();
        ProcessPlayerPropertyPurchase(playerID, propertyInstance);
    }
    private void ProcessPlayerStationPurchase(string playerID, int photonIDOfTile)
    {
        TileInstance_Station stationInstance = PhotonNetwork.GetPhotonView(photonIDOfTile).GetComponent<TileInstance_Station>();
        ProcessPlayerStationPurchase(playerID, stationInstance);
    }
    private void ProcessPlayerUtilityPurchase(string playerID, int photonIDOfTile)
    {
        TileInstance_Utility utilityInstance = PhotonNetwork.GetPhotonView(photonIDOfTile).GetComponent<TileInstance_Utility>();
        ProcessPlayerUtilityPurchase(playerID, utilityInstance);
    }
    public void ProcessPlayerPropertyPurchase(string playerID, TileInstance_Property propertyTile)
    {
        OwnedPlayerTileTracker playerOwnedTileTracker = PlayersOwnedTileTrackerDictionary[playerID];
        playerOwnedTileTracker.GainProperty(propertyTile);
        propertyTile.GiveOwnershipToPlayer(playerID);

        PropertyColourSet colourOfProperty = propertyTile.propertyData.PropertyColourSet;
        //If the player has not owned all of any property type, check if they now own all of the properties of the type they have just acquired.

        if (PlayerOwnsAllOfPropertyType(playerOwnedTileTracker, colourOfProperty))
        {
            playerOwnedTileTracker.AddToAllOwnedPropertyOfTypeList(colourOfProperty);
            //playerOwnedTileTracker.OwnsAllOfAPropertyType = true;
        }
    }
    public void ProcessPlayerStationPurchase(string playerID, TileInstance_Station stationTile)
    {
        PlayersOwnedTileTrackerDictionary[playerID].GainStation(stationTile);
        stationTile.GiveOwnershipToPlayer(playerID);
    }
    public void ProcessPlayerUtilityPurchase(string playerID, TileInstance_Utility utilityTile)
    {
        PlayersOwnedTileTrackerDictionary[playerID].GainUtility(utilityTile);
        utilityTile.GiveOwnershipToPlayer(playerID);
    }
    #endregion

    public void ProcessPlayerRemovingTile(string playerID,int tilePhotonID)
    {
        photonView.RPC(nameof(ProcessPlayerRemovingTileRPC), RpcTarget.All, playerID, tilePhotonID);
    }
    [PunRPC]
    public void ProcessPlayerRemovingTileRPC(string playerID,int tilePhotonID)
    {
        Type typeOfTile = photonIDPropertyTypeDictionary[tilePhotonID];
        processTileRemovalBasedOnTypeDictionary[typeOfTile].Invoke(playerID, tilePhotonID);
    }
    private void ProcessPlayerPropertyRemoval(string playerID, int photonIDOfTile)
    {
        TileInstance_Property propertyInstance = PhotonNetwork.GetPhotonView(photonIDOfTile).GetComponent<TileInstance_Property>();
        ProcessPlayerPropertyRemoval(playerID, propertyInstance);
    }
    private void ProcessPlayerPropertyRemoval(string playerID, TileInstance_Property propertyTile)
    {
        OwnedPlayerTileTracker playerOwnedTileTracker = PlayersOwnedTileTrackerDictionary[playerID];
        playerOwnedTileTracker.RemoveProperty(propertyTile);
        propertyTile.RemovePlayerOwnership();

        PropertyColourSet colourOfProperty = propertyTile.propertyData.PropertyColourSet;

        if (PlayerOwnsAllOfPropertyType(playerOwnedTileTracker, colourOfProperty))
        {
            playerOwnedTileTracker.RemoveFromAllOwnedPropertyOfTypeList(colourOfProperty);
        }
    }

    //Not tested.
    //public bool PlayerOwnsAllOfPropertyColour(string playerID, PropertyColourSet propertyColour)
    //{
    //    int numOwnedPropertiesOfThatColour = PlayersOwnedTileTrackerDictionary[playerID].ownedPropertiesList.Count(prop => prop.propertyData.PropertyColourSet == propertyColour);
    //    return board.propertyTypeInstanceCountDictionary[propertyColour] == numOwnedPropertiesOfThatColour;
    //}

    private void OnDestroy()
    {
        GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawned;
    }
}