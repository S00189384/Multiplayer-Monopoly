using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LandedOnUnownedStationPrompt : UI_LandedOnUnownedTilePrompt
{
    [SerializeField] private UI_StationInformation stationInfoDisplay;

    [Header("Data of landed player")]
    private TileInstance_Station stationTileLandedOn;

    public void InitialiseDisplay(string playerID, TileInstance_Station stationLandedOn)
    {
        RecieveLandedPlayerInformation(playerID, stationLandedOn);

        stationInfoDisplay.UpdateDisplay(stationLandedOn.tileDataStation);

        //Set up buttons.
        SetUpButtons(stationLandedOn.PurchaseCost);

        stationTileLandedOn = stationLandedOn;
    }

    private void RecieveLandedPlayerInformation(string playerID, TileInstance_Station stationTileInstance)
    {
        playerIDOfLandedPlayer = playerID;
        stationTileLandedOn = stationTileInstance;
        photonViewIDOfLandedOnTile = stationTileLandedOn.photonView.ViewID;
        moneyAccountOfLandedPlayer = Bank.Instance.GetPlayerMoneyAccountByID(playerIDOfLandedPlayer);
    }

    public override void PurchaseMyTile()
    {
        UI_NotificationManager.Instance.RPC_ShowNotification($"{GameManager.Instance.GetPlayerNicknameFromID(playerIDOfLandedPlayer)} purchased {stationTileLandedOn.tileDataStation.Name} for ${stationTileLandedOn.PurchaseCost}", RpcTarget.All);
        TileOwnershipManager.Instance.ProcessPlayerStationPurchase(playerIDOfLandedPlayer, stationTileLandedOn);
    }

    //public void OnPurchaseTileButtonClicked()
    //{
    //    //Don't need to check if they can afford the tile since that was done during the button setup.
    //    //if moneyAccountOfLandedPlayer.CanAffordPurchase(purchasableTileLandedOn.PurchaseCost)
    //    UI_NotificationManager.Instance.RPC_ShowNotification($"{GameManager.Instance.GetPlayerNicknameFromID(playerIDOfLandedPlayer)} purchased {stationTileLandedOn.tileDataStation.Name} for ${stationTileLandedOn.PurchaseCost}", RpcTarget.All);
    //    TileOwnershipManager.Instance.ProcessPlayerStationPurchase(playerIDOfLandedPlayer, stationTileLandedOn);

    //    Destroy(gameObject);
    //}
}