using Photon.Pun;
using UnityEngine;

//Prompt shown to player when landing on an unowned station.

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
        Bank.Instance.ProcessPlayerTilePurchase(playerIDOfLandedPlayer, stationTileLandedOn.photonView.ViewID, stationTileLandedOn.PurchaseCost);
    }
}