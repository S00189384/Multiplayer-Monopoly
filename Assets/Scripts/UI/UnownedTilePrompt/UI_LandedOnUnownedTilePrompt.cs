using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LandedOnUnownedTilePrompt : MonoBehaviour
{
    [Header("Base Unowned Tile Prompt UI")]
    [SerializeField] protected BTN_Base btnPurchase;
    [SerializeField] protected BTN_StartAuction btnStartAuction;

    [Header("Data of landed player")]
    protected string playerIDOfLandedPlayer;
    protected int photonViewIDOfLandedOnTile;
    protected PlayerMoneyAccount moneyAccountOfLandedPlayer;

    protected void SetUpButtons(int purchaseCost)
    {
        if (moneyAccountOfLandedPlayer.CanAffordPurchase(purchaseCost))
        {
            btnPurchase.SetButtonText($"Purchase for ${purchaseCost}");
        }
        else
        {
            btnPurchase.SetButtonInteractable(false);
            btnPurchase.SetButtonText("Can't afford purchase");
        }
    }

    public virtual void OnStartAuctionButtonPressed()
    {
        btnStartAuction.RaiseAuctionStartedEvent(PhotonNetwork.LocalPlayer.UserId, photonViewIDOfLandedOnTile);

        Destroy(gameObject);
    }

    public virtual void PurchaseMyTile() { }
    public virtual void AddOnClickListenerToPurchaseTileButton(Action action)
    {
        btnPurchase.AddOnClickListener(() => action());
    }
    public virtual void AddOnClickListenerToStartAuctionButton(Action action)
    {
        btnStartAuction.AddOnClickListener(() => action());
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
