using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: have countdown timer for auction start if player cant afford property?


public class UI_LandedOnUnownedTilePromptManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private UI_LandedOnUnownedTilePrompt landedOnUnownedTilePromptPrefab;
    [SerializeField] private UI_LandedOnUnownedPropertyPrompt landedOnUnownedPropertyPromptPrefab;
    [SerializeField] private UI_LandedOnUnownedStationPrompt landedOnUnownedStationPromptPrefab;
    [SerializeField] private UI_LandedOnUnownedUtilityPrompt landedOnUnownedUtiltyPromptPrefab;

    [SerializeField] private Transform T_promptSpawn;
    [SerializeField] private CanvasGroup CG_Background;

    [Header("Movement On/Off Screen Settings")]
    [SerializeField] private float timeToMoveToPosition;
    [SerializeField] private LeanTweenType tweenType;

    private UI_LandedOnUnownedTilePrompt spawnedLandedOnTilePropmt;

    private void Awake()
    {
        //TileInstance_Purchasable.PlayerLandedOnUnownedPurchasableTileEvent += OnPlayerLandedOnUnownedPurchasableTile;
        TileInstance_Property.PlayerLandedOnUnownedPropertyEvent += OnPlayerLandedOnUnownedProperty;
        TileInstance_Station.PlayerLandedOnUnownedStationEvent += OnPlayerLandedOnUnownedStation;
        TileInstance_Utility.PlayerLandedOnUnownedUtilityEvent += OnPlayerLandedOnUnknownUtility;
    }

    private void OnPlayerLandedOnUnownedPurchasableTile(string playerID, int photonViewIDOfTile)
    {
        //UI_LandedOnUnownedTilePrompt spawnedUnownedTilePrompt = Instantiate(landedOnUnownedTilePromptPrefab, this.transform);
        //spawnedUnownedTilePrompt.InitialiseDisplay(playerID, photonViewIDOfTile);
    }

    private void OnPlayerLandedOnUnownedProperty(string playerID, TileInstance_Property propertyInstanceLandedOn)
    {
        //UI_LandedOnUnownedPropertyPrompt spawnedUnownedPropertyPrompt = Instantiate(landedOnUnownedPropertyPromptPrefab,this.transform);

        UI_LandedOnUnownedPropertyPrompt spawnedUnownedPropertyPrompt = Instantiate(landedOnUnownedPropertyPromptPrefab,T_promptSpawn.position,Quaternion.identity, this.transform);
        spawnedUnownedPropertyPrompt.InitialiseDisplay(playerID, propertyInstanceLandedOn);
        spawnedLandedOnTilePropmt = spawnedUnownedPropertyPrompt;

        FixDisplayOfPrompt();
        SetUpButtonsOnPrompt();
        MovePromptOnScreen();
        //CG_Background.gameObject.SetActive(true);
        //LeanTween.alphaCanvas(CG_Background, 1f, timeToMoveToPosition);
        //LeanTween.move(spawnedUnownedPropertyPrompt.gameObject, transform.position, timeToMoveToPosition).setEase(tweenType);
    }

    private void OnPurchaseTileButtonClickedOnPrompt()
    {
        //LeanTween.alphaCanvas(CG_Background, 0f, timeToMoveToPosition);

        //LeanTween.scale(spawnedLandedOnTilePropmt.gameObject, Vector3.zero, timeToMoveToPosition).setEase(tweenType)
        //    .setOnComplete(() =>
        //    {
        //        spawnedLandedOnTilePropmt.PurchaseMyTile();
        //        CG_Background.gameObject.SetActive(false);
        //        Destroy(spawnedLandedOnTilePropmt.gameObject);
        //    });

        //Move away code below..

        LeanTween.alphaCanvas(CG_Background, 0f, timeToMoveToPosition);
        LeanTween.move(spawnedLandedOnTilePropmt.gameObject, T_promptSpawn.position, timeToMoveToPosition).setEase(tweenType)
            .setOnComplete(() =>
            {
                spawnedLandedOnTilePropmt.PurchaseMyTile();
                CG_Background.gameObject.SetActive(false);
                Destroy(spawnedLandedOnTilePropmt.gameObject);
            });
    }
    private void OnStartAuctionButtonClickedOnPrompt()
    {
        LeanTween.alphaCanvas(CG_Background, 0f, timeToMoveToPosition);

        LeanTween.alphaCanvas(CG_Background, 0f, timeToMoveToPosition);
        LeanTween.move(spawnedLandedOnTilePropmt.gameObject, T_promptSpawn.position, timeToMoveToPosition).setEase(tweenType)
            .setOnComplete(() =>
            {
                spawnedLandedOnTilePropmt.OnStartAuctionButtonPressed();
                CG_Background.gameObject.SetActive(false);
                Destroy(spawnedLandedOnTilePropmt.gameObject);
            });

        //LeanTween.alphaCanvas(CG_Background, 0f, timeToMoveToPosition);
        //LeanTween.scale(spawnedLandedOnTilePropmt.gameObject, Vector3.zero, timeToMoveToPosition).setEase(tweenType)
        //    .setOnComplete(() =>
        //    {
        //        spawnedLandedOnTilePropmt.OnStartAuctionButtonPressed();
        //        CG_Background.gameObject.SetActive(false);
        //        Destroy(spawnedLandedOnTilePropmt.gameObject);
        //    });
    }


    private void OnPlayerLandedOnUnownedStation(string playerID, TileInstance_Station stationInstanceLandedOn)
    {
        UI_LandedOnUnownedStationPrompt spawnedUnownedStationPrompt = Instantiate(landedOnUnownedStationPromptPrefab, T_promptSpawn.position, Quaternion.identity, this.transform);
        spawnedUnownedStationPrompt.InitialiseDisplay(playerID, stationInstanceLandedOn);
        spawnedLandedOnTilePropmt = spawnedUnownedStationPrompt;

        FixDisplayOfPrompt();
        SetUpButtonsOnPrompt();
        MovePromptOnScreen();
    }
    private void OnPlayerLandedOnUnknownUtility(string playerID, TileInstance_Utility utilityInstanceLandedOn)
    {
        UI_LandedOnUnownedUtilityPrompt spawnedUnownedUtilityPrompt = Instantiate(landedOnUnownedUtiltyPromptPrefab, T_promptSpawn.position, Quaternion.identity, this.transform);
        spawnedUnownedUtilityPrompt.InitialiseDisplay(playerID, utilityInstanceLandedOn);
        spawnedLandedOnTilePropmt = spawnedUnownedUtilityPrompt;

        FixDisplayOfPrompt();
        SetUpButtonsOnPrompt();
        MovePromptOnScreen();
    }

    private void FixDisplayOfPrompt()
    {
        spawnedLandedOnTilePropmt.transform.localEulerAngles = Vector3.zero;
    }
    private void SetUpButtonsOnPrompt()
    {
        spawnedLandedOnTilePropmt.AddOnClickListenerToPurchaseTileButton(OnPurchaseTileButtonClickedOnPrompt);
        spawnedLandedOnTilePropmt.AddOnClickListenerToStartAuctionButton(OnStartAuctionButtonClickedOnPrompt);
    }
    private void MovePromptOnScreen()
    {
        CG_Background.gameObject.SetActive(true);
        LeanTween.alphaCanvas(CG_Background, 1f, timeToMoveToPosition);
        LeanTween.move(spawnedLandedOnTilePropmt.gameObject, transform.position, timeToMoveToPosition).setEase(tweenType);
    }




    private void OnDestroy()
    {
        TileInstance_Property.PlayerLandedOnUnownedPropertyEvent -= OnPlayerLandedOnUnownedProperty;
        TileInstance_Station.PlayerLandedOnUnownedStationEvent -= OnPlayerLandedOnUnownedStation;
        TileInstance_Utility.PlayerLandedOnUnownedUtilityEvent -= OnPlayerLandedOnUnknownUtility;
    }

    ////Spawn position of purchasable display.
    ////Prefabs for utility, station and property display.
    //[Header("Prefabs")]
    //[SerializeField] private UI_PropertyInformation propertyInfoPrefab;
    //[SerializeField] private UI_StationInformation stationInfoPrefab;
    //[SerializeField] private UI_UtilityInformation utilityInfoPrefab;

    //[SerializeField] private Transform tilePrefabTargetTransform;

    //[Header("UI Belonging To Prompt")]
    //[SerializeField] private BTN_Base btnPurchase;
    //[SerializeField] private BTN_Base btnStartAuction;
    //[SerializeField] private GameObject UIPanel; 

    //[Header("Data of landed player")]
    //private string playerIDOfLandedPlayer;
    //private int photonIDOfPurchasableTile;
    //private TileInstance_Purchasable purchasableTileLandedOn;

    //private TileInstance_Property propertyTileLandedOn;
    //private TileInstance_Station stationTileLandedOn;
    //private TileInstance_Utility utilityTileLandedOn;

    //private PlayerMoneyAccount moneyAccountOfLandedPlayer;
    //private GameObject spawnedTileInfo;

    //private void Awake()
    //{
    //    btnPurchase.AddOnClickListener(OnPurchaseTileButtonClicked);
    //    btnStartAuction.AddOnClickListener(OnStartAuctionButtonPressed);

    //    TileInstance_Property.PlayerLandedOnUnownedPropertyEvent += OnPlayerLandedOnUnownedProperty;
    //    TileInstance_Station.PlayerLandedOnUnownedStationEvent += OnPlayerLandedOnUnownedStation;
    //    TileInstance_Utility.PlayerLandedOnUnownedUtilityEvent += OnPlayerLandedOnUnknownUtility;
    //}

    //private void SetUpButtons(int purchaseCost)
    //{
    //    if (moneyAccountOfLandedPlayer.CanAffordPurchase(purchaseCost))
    //    {
    //        btnPurchase.SetButtonText($"Purchase for ${purchaseCost}");
    //    }
    //    else
    //    {
    //        //Show more info in this case? "Cant afford property"...
    //        //Start countdown timer..
    //        btnPurchase.SetButtonInteractable(false);
    //    }
    //}
    //private void OnPlayerLandedOnUnownedProperty(string playerID, TileInstance_Property propertyInstance)
    //{
    //    RecieveLandedPlayerInformation(playerID, propertyInstance);

    //    //Set up UI.
    //    UIPanel.gameObject.SetActive(true);

    //    //Spawn info for property.
    //    UI_PropertyInformation spawnedPropertyInfo = Instantiate(propertyInfoPrefab, tilePrefabTargetTransform.position, Quaternion.identity);
    //    spawnedTileInfo = spawnedPropertyInfo.gameObject;
    //    spawnedPropertyInfo.transform.SetParent(UIPanel.transform);
    //    spawnedPropertyInfo.UpdateDisplay(propertyInstance.propertyData);

    //    //Set up buttons.
    //    SetUpButtons(propertyInstance.PurchaseCost);

    //    propertyTileLandedOn = propertyInstance;
    //}

    //private void OnPlayerLandedOnUnownedStation(string playerID, TileInstance_Station stationInstance)
    //{
    //    RecieveLandedPlayerInformation(playerID,stationInstance);

    //    UIPanel.gameObject.SetActive(true);
    //    UI_StationInformation spawnedStationInfo = Instantiate(stationInfoPrefab, tilePrefabTargetTransform.position, Quaternion.identity);
    //    spawnedTileInfo = spawnedStationInfo.gameObject;
    //    spawnedStationInfo.transform.SetParent(UIPanel.transform);
    //    spawnedStationInfo.UpdateDisplay(stationInstance.tileDataStation);

    //    SetUpButtons(stationInstance.PurchaseCost);

    //    stationTileLandedOn = stationInstance;
    //}

    //private void OnPlayerLandedOnUnknownUtility(string playerID, TileInstance_Utility utilityInstance)
    //{
    //    RecieveLandedPlayerInformation(playerID,utilityInstance);

    //    UIPanel.gameObject.SetActive(true);
    //    UI_UtilityInformation spawnedUtilityInfo = Instantiate(utilityInfoPrefab, tilePrefabTargetTransform.position, Quaternion.identity);
    //    spawnedTileInfo = spawnedUtilityInfo.gameObject;
    //    spawnedUtilityInfo.transform.SetParent(UIPanel.transform);
    //    spawnedUtilityInfo.UpdateDisplay(utilityInstance.tileDataUtility);

    //    SetUpButtons(utilityInstance.PurchaseCost);

    //    utilityTileLandedOn = utilityInstance;
    //}

    //private void RecieveLandedPlayerInformation(string playerID,TileInstance_Purchasable purchasableTileInstance)
    //{
    //    playerIDOfLandedPlayer = playerID;
    //    purchasableTileLandedOn = purchasableTileInstance;
    //    moneyAccountOfLandedPlayer = Bank.Instance.GetPlayerMoneyAccountByID(playerIDOfLandedPlayer);
    //}


    //private void OnPurchaseTileButtonClicked()
    //{
    //    //Don't need to check if they can afford the tile since that was done during the button setup.
    //    //if moneyAccountOfLandedPlayer.CanAffordPurchase(purchasableTileLandedOn.PurchaseCost)

    //    //TileOwnershipManager.Instance.ProcessPlayerTilePurchase();
    //    if(propertyTileLandedOn)
    //    {
    //        TileOwnershipManager.Instance.ProcessPlayerPropertyPurchase(playerIDOfLandedPlayer,propertyTileLandedOn);
    //    }
    //    else if(stationTileLandedOn)
    //    {

    //    }
    //    else if(utilityTileLandedOn)
    //    {

    //    }

    //    Bank.Instance.ProcessPlayerTilePurchase(moneyAccountOfLandedPlayer, purchasableTileLandedOn);
    //    CloseUIPanel();
    //}

    //private void OnStartAuctionButtonPressed()
    //{
    //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
    //    object[] eventContent = new object[] { PhotonNetwork.LocalPlayer.UserId, purchasableTileLandedOn.photonView.ViewID };
    //    PhotonNetwork.RaiseEvent(EventCodes.SinglePropertyAuctionStartedEventCode,eventContent, raiseEventOptions, SendOptions.SendReliable);
    //    CloseUIPanel();
    //}

    //private void CloseUIPanel()
    //{
    //    Destroy(spawnedTileInfo.gameObject);
    //    btnPurchase.SetButtonInteractable(true);
    //    UIPanel.gameObject.SetActive(false);
    //    playerIDOfLandedPlayer = null;
    //    purchasableTileLandedOn = null;
    //}

    //private void OnDestroy()
    //{
    //    TileInstance_Property.PlayerLandedOnUnownedPropertyEvent -= OnPlayerLandedOnUnownedProperty;
    //    TileInstance_Station.PlayerLandedOnUnownedStationEvent -= OnPlayerLandedOnUnownedStation;
    //    TileInstance_Utility.PlayerLandedOnUnownedUtilityEvent -= OnPlayerLandedOnUnknownUtility;
    //}
}