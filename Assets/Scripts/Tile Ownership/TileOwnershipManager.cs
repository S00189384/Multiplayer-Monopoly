using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

//Class for handling the acquiring / removing of tiles from players.
//This is one of the major scripts I would rework when coming back to this project as there are several things that should be changed.

public class TileOwnershipManager : MonoBehaviourPunCallbacks
{
    //Singleton.
    public static TileOwnershipManager Instance;

    public Board board;

    //Each players owned tile trackers.
    private Dictionary<string, OwnedPlayerTileTracker> PlayersOwnedTileTrackerDictionary = new Dictionary<string, OwnedPlayerTileTracker>();
    public OwnedPlayerTileTracker GetOwnedPlayerTileTracker(string playerID) => PlayersOwnedTileTrackerDictionary[playerID];
    public OwnedPlayerTileTracker GetLocalPlayersOwnedTileTracker { get { return PlayersOwnedTileTrackerDictionary[PhotonNetwork.LocalPlayer.UserId]; } }

    //Type of purchasable tile from its photon ID. Move to game manager / board instead?
    private Dictionary<int,Type> photonIDPropertyTypeDictionary = new Dictionary<int, Type>();
    public Type GetPurchasableTileTypeFromPhotonID(int photonIDOfTile) => photonIDPropertyTypeDictionary[photonIDOfTile];

    //Which process ownership method to call based on the type of purchasable tile.
    private Dictionary<Type, Action<string,int>> processTilePurchaseBasedOnTypeDictionary = new Dictionary<Type, Action<string, int>>();
    private Dictionary<Type, Action<string,int>> processTileRemovalBasedOnTypeDictionary = new Dictionary<Type, Action<string, int>>();

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
        Bank.PlayerDeclaredBankruptDueToPlayerPaymentEvent += OnPlayerBankruptCausedByOtherPlayer;
    }

    private void OnPlayerBankruptCausedByOtherPlayer(string playerIDBankrupt, string playerIDThatCausedBankrupsy)
    {
        //Transfer owned tiles.
        TransferAllPlayerOwnedTilesToAnotherPlayer(playerIDBankrupt, playerIDThatCausedBankrupsy);
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
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Remove their owned tiles.
        if(Bank.Instance.PlayerIsBankrupt(otherPlayer.UserId) == false)
            RemoveAllPlayerOwnedTiles(otherPlayer.UserId,true);
    }

    #region Processing player acquiring a property
    public void ProcessPlayerTilePurchaseAllClients(string playerID,int tilePhotonID)
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

        PropertyColourSet colourOfProperty = propertyTile.propertyData.PropertyColourSet;
        if (PlayerOwnsAllOfPropertyType(playerOwnedTileTracker, colourOfProperty))
        {
            playerOwnedTileTracker.RemoveFromAllOwnedPropertyOfTypeList(colourOfProperty);
        }

        playerOwnedTileTracker.RemoveProperty(propertyTile);
        propertyTile.RemovePlayerOwnershipForLocalClient();
    }
    private void ProcessPlayerStationRemoval(string playerID, TileInstance_Station stationTile)
    {
        OwnedPlayerTileTracker playerOwnedTileTracker = PlayersOwnedTileTrackerDictionary[playerID];
        playerOwnedTileTracker.RemoveStation(stationTile);
        stationTile.RemovePlayerOwnershipForLocalClient();
    }
    private void ProcessPlayerUtilityRemoval(string playerID, TileInstance_Utility utilityTile)
    {
        OwnedPlayerTileTracker playerOwnedTileTracker = PlayersOwnedTileTrackerDictionary[playerID];
        playerOwnedTileTracker.RemoveUtility(utilityTile);
        utilityTile.RemovePlayerOwnershipForLocalClient();
    }


    //Transfering all owned tiles to between players.
    public void TransferAllPlayerOwnedTilesToAnotherPlayer(string playerIDOfGiver, string playerIDOfReceiver)
    {
        photonView.RPC(nameof(TransferAllPlayerOwnedTilesToAnotherPlayerRPC), RpcTarget.All, playerIDOfGiver, playerIDOfReceiver);    
    }

    [PunRPC]
    private void TransferAllPlayerOwnedTilesToAnotherPlayerRPC(string playerIDOfGiver, string playerIDOfReceiver)
    {
        OwnedPlayerTileTracker ownedTileTrackerOfGiver = PlayersOwnedTileTrackerDictionary[playerIDOfGiver];

        //Player does not own any tiles - nothing to give to the other player.
        if (ownedTileTrackerOfGiver.OwnsAPurchasableTile == false)
            return;

        if (ownedTileTrackerOfGiver.OwnsAProperty)
        {
            for (int i = ownedTileTrackerOfGiver.ownedPropertiesList.Count - 1; i >= 0; i--)
            {
                var property = ownedTileTrackerOfGiver.ownedPropertiesList[i];
                ProcessPlayerPropertyRemoval(playerIDOfGiver, property);
                ProcessPlayerPropertyPurchase(playerIDOfReceiver, property);
            }
        }

        if (ownedTileTrackerOfGiver.OwnsAStation)
        {
            for (int i = ownedTileTrackerOfGiver.ownedStationsList.Count - 1; i >= 0; i--)
            {
                var station = ownedTileTrackerOfGiver.ownedStationsList[i];
                ProcessPlayerStationRemoval(playerIDOfGiver, station);
                ProcessPlayerStationPurchase(playerIDOfReceiver, station);
            }
        }

        if (ownedTileTrackerOfGiver.OwnsAUtility)
        {
            for (int i = ownedTileTrackerOfGiver.ownedUtilitiesList.Count - 1; i >= 0; i--)
            {
                var utility = ownedTileTrackerOfGiver.ownedUtilitiesList[i];
                ProcessPlayerUtilityRemoval(playerIDOfGiver, utility);
                ProcessPlayerUtilityPurchase(playerIDOfReceiver, utility);
            }
        }         
    }

    //Player disconnects.
    private void RemoveAllPlayerOwnedTiles(string playerID,bool unmortgageTile)
    {
        OwnedPlayerTileTracker ownedTileTrackerOfGiver = PlayersOwnedTileTrackerDictionary[playerID];

        if (ownedTileTrackerOfGiver.OwnsAPurchasableTile == false)
            return;

        if (ownedTileTrackerOfGiver.OwnsAProperty)
        {
            for (int i = ownedTileTrackerOfGiver.ownedPropertiesList.Count - 1; i >= 0; i--)
            {
                var property = ownedTileTrackerOfGiver.ownedPropertiesList[i];
                ProcessPlayerPropertyRemoval(playerID, property);
                if (unmortgageTile && property.IsMortgaged)
                    property.UnmortgageTileOnTileReset();

                if (property.HasConstructedBuildings)
                    property.DestroyBuildings();
            }
        }
        if (ownedTileTrackerOfGiver.OwnsAStation)
        {
            for (int i = ownedTileTrackerOfGiver.ownedStationsList.Count - 1; i >= 0; i--)
            {
                var station = ownedTileTrackerOfGiver.ownedStationsList[i];
                ProcessPlayerStationRemoval(playerID, station);
                if (unmortgageTile && station.IsMortgaged)
                    station.UnmortgageTileOnTileReset();
            }
        }
        if (ownedTileTrackerOfGiver.OwnsAUtility)
        {
            for (int i = ownedTileTrackerOfGiver.ownedUtilitiesList.Count - 1; i >= 0; i--)
            {
                var utility = ownedTileTrackerOfGiver.ownedUtilitiesList[i];
                ProcessPlayerUtilityRemoval(playerID, utility);
                if (unmortgageTile && utility.IsMortgaged)
                    utility.UnmortgageTileOnTileReset();
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawned;
        Bank.PlayerDeclaredBankruptDueToPlayerPaymentEvent -= OnPlayerBankruptCausedByOtherPlayer;
    }
}