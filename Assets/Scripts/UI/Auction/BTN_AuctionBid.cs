using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Raise network event that a player has bidded.
//If player folds, disable their bid button. 

public class BTN_AuctionBid : BTN_Base
{
    [SerializeField] private UI_AuctionBidSlider auctionBidSlider;

    public static event Action<string,int> PlayerBiddedAtAuction;

    public override void Awake()
    {
        AuctionTurnManager.NewPlayerAuctionTurnEvent += OnNewPlayerAuctionTurn;
        AuctionTurnManager.PlayerWonAuctionEvent += OnPlayerWonAuction;
        BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent += OnPlayerFoldedFromAuction;
        AddOnClickListener(Bid);
    }
    private void OnDestroy()
    {
        AuctionTurnManager.NewPlayerAuctionTurnEvent -= OnNewPlayerAuctionTurn;
        AuctionTurnManager.PlayerWonAuctionEvent -= OnPlayerWonAuction;
        BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent -= OnPlayerFoldedFromAuction;
    }

    private void OnPlayerWonAuction(string playerID,int finalBid)
    {
        SetButtonInteractable(false);
    }


    private void OnPlayerFoldedFromAuction(string playerID)
    {
        SetButtonInteractable(false);
    }

    private void OnNewPlayerAuctionTurn(string newPlayerTurn)   
    {
        if (newPlayerTurn == PhotonNetwork.LocalPlayer.UserId)
        {
            SetButtonInteractable(true);
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
        SetButtonInteractable(false);

        PlayerBiddedAtAuction?.Invoke(PhotonNetwork.LocalPlayer.UserId, bidAmount);
    }
}
