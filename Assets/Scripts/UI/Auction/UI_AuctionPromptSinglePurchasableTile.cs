using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AuctionPromptSinglePurchasableTile : MonoBehaviourPun
{
    [Header("Possible Tile Info Display Spawning")]
    [SerializeField] private UI_PropertyInformation propertyInfoPrefab;
    [SerializeField] private UI_StationInformation stationInfoPrefab;
    [SerializeField] private UI_UtilityInformation utilityInfoPrefab;
    [SerializeField] private Transform tileInfoSpawnTransform;
    [Header("Components of prompt")]
    [SerializeField] private TextMeshProUGUI TMP_OriginalTilePurchaseCost;
    [SerializeField] private UI_AuctionTurnDisplayManager turnDisplayManager;

    public void InitialisePrompt(string playerIDStartedAuction, int photonIDOfTileForAuction)
    {
        //Spawn info display of the tile that is up for auction.
        Dictionary<Type,Action<int>> tileInfoDisplayMethodCallDictionary = new Dictionary<Type, Action<int>>()
        {
             { typeof(TileInstance_Property), SpawnPropertyInfoDisplay},
             { typeof(TileInstance_Station), SpawnStationInfoDisplay},
             { typeof(TileInstance_Utility), SpawnUtilityInfoDisplay}
        };

        Type typeOfPurchasableTile = TileOwnershipManager.Instance.GetPurchasableTileTypeFromPhotonID(photonIDOfTileForAuction);
        tileInfoDisplayMethodCallDictionary[typeOfPurchasableTile](photonIDOfTileForAuction);

        turnDisplayManager.SpawnActivePlayersDisplay(GameManager.Instance.ActivePlayersIDList);
    }

    public void SpawnPropertyInfoDisplay(int photonViewID)
    {
        TileInstance_Property propertyLandedOn = PhotonNetwork.GetPhotonView(photonViewID).GetComponent<TileInstance_Property>();
        UI_PropertyInformation spawnedPropertyInformation = Instantiate(propertyInfoPrefab, tileInfoSpawnTransform.position, Quaternion.identity, tileInfoSpawnTransform);
        spawnedPropertyInformation.transform.localEulerAngles = Vector3.zero;
        spawnedPropertyInformation.UpdateDisplay(propertyLandedOn.propertyData);
        TMP_OriginalTilePurchaseCost.text = $"Original purchase cost: ${propertyLandedOn.PurchaseCost}";
    }
    private void SpawnStationInfoDisplay(int photonViewID)
    {
        TileInstance_Station stationLandedOn = PhotonNetwork.GetPhotonView(photonViewID).GetComponent<TileInstance_Station>();
        UI_StationInformation spawnedStationInformation = Instantiate(stationInfoPrefab, tileInfoSpawnTransform.position, Quaternion.identity, tileInfoSpawnTransform);
        spawnedStationInformation.transform.localEulerAngles = Vector3.zero;
        spawnedStationInformation.UpdateDisplay(stationLandedOn.tileDataStation);
        TMP_OriginalTilePurchaseCost.text = $"Original purchase cost: ${stationLandedOn.PurchaseCost}";
    }

    private void SpawnUtilityInfoDisplay(int photonViewID)
    {
        TileInstance_Utility utilityLandedOn = PhotonNetwork.GetPhotonView(photonViewID).GetComponent<TileInstance_Utility>();
        UI_UtilityInformation spawnedUtilityInformation = Instantiate(utilityInfoPrefab, tileInfoSpawnTransform.position, Quaternion.identity, tileInfoSpawnTransform);
        spawnedUtilityInformation.transform.localEulerAngles = Vector3.zero;
        spawnedUtilityInformation.UpdateDisplay(utilityLandedOn.tileDataUtility);
        TMP_OriginalTilePurchaseCost.text = $"Original purchase cost: ${utilityLandedOn.PurchaseCost}";
    }
}





//[Header("Prefabs of Purchasable Tile Info Displays")]
//[SerializeField] private UI_PropertyInformation propertyInfoPrefab;
//[SerializeField] private UI_StationInformation stationInfoPrefab;
//[SerializeField] private UI_UtilityInformation utilityInfoPrefab;

//[Header("Components")]
//[SerializeField] protected Transform tileInfoSpawnTransform;
//[SerializeField] protected BTN_Base btnPurchase;
//[SerializeField] protected BTN_Base btnStartAuction;

//[Header("Data of landed player")]
//protected string playerIDOfLandedPlayer;
//protected PlayerMoneyAccount moneyAccountOfLandedPlayer;
//private int photonViewIDOfLandedOnTile;
//int purchaseCost;

////Given a type, what method to call to spawn the display information of the landed on purchasable tile.
//private Dictionary<Type, Action<int>> tileInfoDisplayMethodCallDictionary = new Dictionary<Type, Action<int>>();



//protected void SetUpButtons(int purchaseCost)
//{
//    if (moneyAccountOfLandedPlayer.CanAffordPurchase(purchaseCost))
//    {
//        btnPurchase.SetButtonText($"Purchase for ${purchaseCost}");
//    }
//    else
//    {
//        btnPurchase.SetButtonInteractable(false);
//    }
//}

//public void DealWithProperty(int photonViewID)
//{
//    TileInstance_Property propertyLandedOn = PhotonNetwork.GetPhotonView(photonViewID).GetComponent<TileInstance_Property>();
//    purchaseCost = propertyLandedOn.PurchaseCost;
//    UI_PropertyInformation spawnedPropertyInformation = Instantiate(propertyInfoPrefab, tileInfoSpawnTransform.position, Quaternion.identity, tileInfoSpawnTransform);
//    spawnedPropertyInformation.UpdateDisplay(propertyLandedOn.propertyData);
//}
//private void DealWithStation(int photonViewID)
//{
//    TileInstance_Station stationLandedOn = PhotonNetwork.GetPhotonView(photonViewID).GetComponent<TileInstance_Station>();
//    purchaseCost = stationLandedOn.PurchaseCost;
//    UI_StationInformation spawnedStationInformation = Instantiate(stationInfoPrefab, tileInfoSpawnTransform.position, Quaternion.identity, tileInfoSpawnTransform);
//    spawnedStationInformation.UpdateDisplay(stationLandedOn.tileDataStation);
//}

//private void DealWithUtility(int photonViewID)
//{
//    TileInstance_Utility utilityLandedOn = PhotonNetwork.GetPhotonView(photonViewID).GetComponent<TileInstance_Utility>();
//    purchaseCost = utilityLandedOn.PurchaseCost;
//    UI_UtilityInformation spawnedUtilityInformation = Instantiate(utilityInfoPrefab, tileInfoSpawnTransform.position, Quaternion.identity, tileInfoSpawnTransform);
//    spawnedUtilityInformation.UpdateDisplay(utilityLandedOn.tileDataUtility);
//}



//public void InitialiseDisplay(string playerID, int photonViewID)
//{
//    tileInfoDisplayMethodCallDictionary = new Dictionary<Type, Action<int>>()
//        {
//            { typeof(TileInstance_Property), DealWithProperty},
//            { typeof(TileInstance_Station), DealWithStation},
//            { typeof(TileInstance_Utility), DealWithUtility}
//        };

//    btnStartAuction.AddOnClickListener()

//        RecieveLandedPlayerInformation(playerID, photonViewID);

//    //Spawn info prefab based on type of purchasable tile.
//    Type typeOfPurchasableTile = TileOwnershipManager.Instance.photonIDPropertyTypeDictionary[photonViewID];
//    tileInfoDisplayMethodCallDictionary[typeOfPurchasableTile](photonViewID);


//    //Set up buttons.
//    SetUpButtons(purchaseCost);
//}

//private void RecieveLandedPlayerInformation(string playerID, int tilePhotonViewID)
//{
//    playerIDOfLandedPlayer = playerID;
//    photonViewIDOfLandedOnTile = tilePhotonViewID;
//    moneyAccountOfLandedPlayer = Bank.Instance.GetPlayerMoneyAccountByID(playerIDOfLandedPlayer);
//}

//public void OnPurchaseTileButtonClicked()
//{
//    //Don't need to check if they can afford the tile since that was done during the button setup.
//    //if moneyAccountOfLandedPlayer.CanAffordPurchase(purchasableTileLandedOn.PurchaseCost)

//    TileOwnershipManager.Instance.ProcessPlayerPropertyPurchase(playerIDOfLandedPlayer, propertyTileLandedOn);

//    Destroy(gameObject);
//}

//public void OnStartAuctionButtonPressed()
//{
//    //Raise event across clients.
//    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
//    object[] eventContent = new object[] { PhotonNetwork.LocalPlayer.UserId, photonViewIDOfLandedOnTile };
//    PhotonNetwork.RaiseEvent(EventCodes.SinglePropertyAuctionStartedEventCode, eventContent, raiseEventOptions, SendOptions.SendReliable);

//    Destroy(gameObject);
//}

//private void OnDestroy()
//{
//    //TileInstance_Property.PlayerLandedOnUnownedPropertyEvent -= InitialiseDisplay;
//}