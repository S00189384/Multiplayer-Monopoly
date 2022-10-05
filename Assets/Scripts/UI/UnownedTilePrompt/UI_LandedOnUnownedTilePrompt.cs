using Photon.Pun;
using System;
using UnityEngine;

//Base class for prompt shown to player when landing on an unowned tile.


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