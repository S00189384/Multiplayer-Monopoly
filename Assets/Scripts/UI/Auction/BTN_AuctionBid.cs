using Photon.Pun;
using System;
using UnityEngine;

//Button to bid during an auction.
//Can't bid if your balance is too low.
//Button is disabled when its not your turn in the auction.


public class BTN_AuctionBid : BTN_Base
{
    [SerializeField] private UI_AuctionBidSlider auctionBidSlider;

    public static event Action<string,int> PlayerBiddedAtAuction;

    private PlayerMoneyAccount playerMoneyAccount;

    public override void Awake()
    {
        if(GameManager.Instance.LocalPlayerIsAnActivePlayer)
        {
            playerMoneyAccount = Bank.Instance.GetLocalPlayerMoneyAccount;

            AuctionTurnManager.NewPlayerAuctionTurnEvent += OnNewPlayerAuctionTurn;
            AuctionTurnManager.PlayerWonAuctionEvent += OnPlayerWonAuction;
            BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent += OnPlayerFoldedFromAuction;

            AddOnClickListener(Bid);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnPlayerWonAuction(string playerID,int finalBid,AuctionType auctionType)
    {
        SetButtonInteractable(false);
    }

    private void OnPlayerFoldedFromAuction(string playerID)
    {
        SetButtonInteractable(false);
    }

    private void OnNewPlayerAuctionTurn(string newPlayerTurn,int currentBid)   
    {
        if (newPlayerTurn == PhotonNetwork.LocalPlayer.UserId)
        {
            if(playerMoneyAccount.Balance > currentBid)
                SetButtonInteractable(true);
            else
                SetButtonInteractable(false);
        }
        else
            SetButtonInteractable(false);
    }

    public void Bid()
    {
        int bidAmount = auctionBidSlider.PlayerBid;
        if (bidAmount >= playerMoneyAccount.Balance)
            SetButtonText("Can't bet more");

        SetButtonInteractable(false);

        PlayerBiddedAtAuction?.Invoke(PhotonNetwork.LocalPlayer.UserId, bidAmount);
    }

    private void OnDestroy()
    {
        AuctionTurnManager.NewPlayerAuctionTurnEvent -= OnNewPlayerAuctionTurn;
        AuctionTurnManager.PlayerWonAuctionEvent -= OnPlayerWonAuction;
        BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent -= OnPlayerFoldedFromAuction;
    }
}