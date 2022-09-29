using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//What if an auction gets triggered and a player has < 0 balance?
//UI doesn't allow a player to roll dice to move if they are in the red so this shouldn't happen.


public class BTN_AuctionBid : BTN_Base
{
    [SerializeField] private UI_AuctionBidSlider auctionBidSlider;

    public static event Action<string,int> PlayerBiddedAtAuction;

    private PlayerMoneyAccount playerMoneyAccount;
    private bool hasBalanceToBid;

    public override void Awake()
    {
        playerMoneyAccount = Bank.Instance.GetLocalPlayerMoneyAccount;
        if (playerMoneyAccount.Balance >= 0)
            hasBalanceToBid = true;

        AuctionTurnManager.NewPlayerAuctionTurnEvent += OnNewPlayerAuctionTurn;
        AuctionTurnManager.PlayerWonAuctionEvent += OnPlayerWonAuction;
        BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent += OnPlayerFoldedFromAuction;


        AddOnClickListener(Bid);
    }

    private void OnPlayerWonAuction(string playerID,int finalBid)
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
            {
                print("My turn and can afford the bid of " + currentBid + "with my balance of " + playerMoneyAccount.Balance);
                SetButtonInteractable(true);
            }
            else
            {
                SetButtonInteractable(false);
                SetButtonText("Can't bet more");
            }
        }
        else
        {
            SetButtonInteractable(false);
        }
    }

    public void Bid()
    {
        //Read this eventually.
        int bidAmount = auctionBidSlider.PlayerBid;
        if (bidAmount >= playerMoneyAccount.Balance)
        {
            SetButtonText("Can't bet more");
            hasBalanceToBid = false;
        }

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
