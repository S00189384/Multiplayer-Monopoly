﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;


public class AuctionTurnManager : MonoBehaviourPunCallbacks
{
    private TurnManager<string> auctionTurns;

    public static event Action<string,int> NewPlayerAuctionTurnEvent;
    public static event Action<string,int,AuctionType> PlayerWonAuctionEvent;

    private int currentAuctionBid;
    private AuctionType auctionType;
    public void ReceiveAuctionTypeOfCurrentAuction(AuctionType auctionType) => this.auctionType = auctionType;

    private void Awake()
    {
        BTN_AuctionBid.PlayerBiddedAtAuction += OnPlayerBiddedAtAuction;
        BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent += RemovePlayer;
    }

    private void Start()
    {
        currentAuctionBid = GameDataSlinger.MIN_AUCTION_BET;

        auctionTurns = new TurnManager<string>();
        auctionTurns.Initialise(GameManager.Instance.ActivePlayersIDList);
        print(GameManager.Instance.ActivePlayersIDList.Count);

        NewPlayerAuctionTurnEvent?.Invoke(auctionTurns.CurrentTurn, currentAuctionBid);
    }

    private void OnPlayerBiddedAtAuction(string playerID, int bidAmount)
    {
        photonView.RPC(nameof(SetCurrentAuctionBidRPC), RpcTarget.All, bidAmount);
        MoveToNextTurnAllClients();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerLocalClient(otherPlayer.UserId);
    }

    [PunRPC]
    private void SetCurrentAuctionBidRPC(int currentAuctionBid)
    {
        this.currentAuctionBid = currentAuctionBid;
    }

    public void MoveToNextTurnAllClients() 
    {
        photonView.RPC(nameof(MoveToNextTurnRPC), RpcTarget.All);
    }

    public void MoveToNextTurnLocalClient()
    {
        auctionTurns.MoveToNextTurn();
        NewPlayerAuctionTurnEvent?.Invoke(auctionTurns.CurrentTurn, currentAuctionBid);
    }

    [PunRPC]
    private void MoveToNextTurnRPC()
    {
        auctionTurns.MoveToNextTurn();
        NewPlayerAuctionTurnEvent?.Invoke(auctionTurns.CurrentTurn, currentAuctionBid);
    }

    public void RemovePlayer(string playerID)
    {
        photonView.RPC(nameof(RemovePlayerRPC), RpcTarget.All,playerID);
    }

    [PunRPC]
    private void RemovePlayerRPC(string playerID)
    {
        if(auctionTurns.CurrentTurn == playerID)
            MoveToNextTurnLocalClient();

        auctionTurns.RemoveTurn(playerID);

        if (auctionTurns.OneRemainingTurn)
        {
            PlayerWonAuctionEvent?.Invoke(auctionTurns.GetFirst,currentAuctionBid,auctionType);
        }
    }

    private void RemovePlayerLocalClient(string playerID)
    {
        if (auctionTurns.CurrentTurn == playerID)
            MoveToNextTurnLocalClient();

        auctionTurns.RemoveTurn(playerID);

        if (auctionTurns.OneRemainingTurn)
        {
            PlayerWonAuctionEvent?.Invoke(auctionTurns.GetFirst, currentAuctionBid, auctionType);
        }
    }

    private void OnDestroy()
    {
        BTN_AuctionBid.PlayerBiddedAtAuction -= OnPlayerBiddedAtAuction;
        BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent -= RemovePlayer;
    }
}