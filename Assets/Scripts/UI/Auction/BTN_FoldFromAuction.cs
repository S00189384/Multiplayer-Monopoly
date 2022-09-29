using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//If player hasn't folded - its always can be pressed (player can fold at any time).
//Player folds - disable button for that player, raise network event notifying other players.

public class BTN_FoldFromAuction : BTN_Base
{
    public static event Action<string> PlayerFoldedFromAuctionEvent;

    public override void Awake()
    {
        base.Awake();

        AddOnClickListener(FoldFromAuction);
        AuctionTurnManager.PlayerWonAuctionEvent += OnPlayerWonAuction;
    }

    private void OnPlayerWonAuction(string playerIDThatWonAuction,int finalBid)
    {
        SetButtonInteractable(false);
    }

    public void FoldFromAuction()
    {
        string localPlayerID = PhotonNetwork.LocalPlayer.UserId;

        PlayerFoldedFromAuctionEvent?.Invoke(localPlayerID);

        SetButtonInteractable(false);
    }

    private void OnDestroy()
    {
        AuctionTurnManager.PlayerWonAuctionEvent -= OnPlayerWonAuction;
    }
}