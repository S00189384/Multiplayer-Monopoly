using Photon.Pun;
using UnityEngine;

//Prompt shown to player when landing on an unowned property.

public class UI_LandedOnUnownedPropertyPrompt : UI_LandedOnUnownedTilePrompt
{
    [SerializeField] private UI_PropertyInformation propertyInfoDisplay;

    private TileInstance_Property propertyTileLandedOn;

    public void InitialiseDisplay(string playerID, TileInstance_Property propertyLandedOn)
    {
        RecieveLandedPlayerInformation(playerID, propertyLandedOn);

        propertyInfoDisplay.UpdateDisplay(propertyLandedOn.propertyData);

        //Set up buttons.
        SetUpButtons(propertyLandedOn.PurchaseCost);

        propertyTileLandedOn = propertyLandedOn;
    }

    private void RecieveLandedPlayerInformation(string playerID, TileInstance_Property propertyTileInstance)
    {
        playerIDOfLandedPlayer = playerID;
        propertyTileLandedOn = propertyTileInstance;
        photonViewIDOfLandedOnTile = propertyTileLandedOn.photonView.ViewID;
        moneyAccountOfLandedPlayer = Bank.Instance.GetPlayerMoneyAccountByID(playerIDOfLandedPlayer);
    }

    public override void PurchaseMyTile()
    {
        UI_NotificationManager.Instance.RPC_ShowNotification($"{GameManager.Instance.GetPlayerNicknameFromID(playerIDOfLandedPlayer)} purchased {propertyTileLandedOn.propertyData.Name} for ${propertyTileLandedOn.PurchaseCost}", RpcTarget.All);
        Bank.Instance.ProcessPlayerTilePurchase(playerIDOfLandedPlayer, propertyTileLandedOn.photonView.ViewID, propertyTileLandedOn.PurchaseCost);
    }
}