using Photon.Pun;
using UnityEngine;

//Prompt shown to player when landing on an unowned utility.

public class UI_LandedOnUnownedUtilityPrompt : UI_LandedOnUnownedTilePrompt
{
    [SerializeField] private UI_UtilityInformation utilityInfoDisplay;

    private TileInstance_Utility utilityTileLandedOn;

    public void InitialiseDisplay(string playerID, TileInstance_Utility utilityLandedOn)
    {
        RecieveLandedPlayerInformation(playerID, utilityLandedOn);

        utilityInfoDisplay.UpdateDisplay(utilityLandedOn.tileDataUtility);

        //Set up buttons.
        SetUpButtons(utilityLandedOn.PurchaseCost);

        utilityTileLandedOn = utilityLandedOn;
    }

    private void RecieveLandedPlayerInformation(string playerID, TileInstance_Utility utilityTileInstance)
    {
        playerIDOfLandedPlayer = playerID;
        utilityTileLandedOn = utilityTileInstance;
        photonViewIDOfLandedOnTile = utilityTileLandedOn.photonView.ViewID;
        moneyAccountOfLandedPlayer = Bank.Instance.GetPlayerMoneyAccountByID(playerIDOfLandedPlayer);
    }

    public override void PurchaseMyTile()
    {
        UI_NotificationManager.Instance.RPC_ShowNotification($"{GameManager.Instance.GetPlayerNicknameFromID(playerIDOfLandedPlayer)} purchased {utilityTileLandedOn.tileDataUtility.Name} for ${utilityTileLandedOn.PurchaseCost}", RpcTarget.All);
        Bank.Instance.ProcessPlayerTilePurchase(playerIDOfLandedPlayer, utilityTileLandedOn.photonView.ViewID, utilityTileLandedOn.PurchaseCost);
    }
}