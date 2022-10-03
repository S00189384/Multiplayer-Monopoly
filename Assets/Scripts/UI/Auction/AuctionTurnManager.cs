using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;


public class AuctionTurnManager : MonoBehaviourPun
{
    private TurnManager<string> auctionTurns;

    public static event Action<string,int> NewPlayerAuctionTurnEvent;
    public static event Action<string,int> PlayerWonAuctionEvent;

    private int currentAuctionBid;

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

        NewPlayerAuctionTurnEvent?.Invoke(auctionTurns.CurrentTurn,currentAuctionBid);
    }


    private void OnPlayerBiddedAtAuction(string playerID, int bidAmount)
    {
        photonView.RPC(nameof(SetCurrentAuctionBidRPC), RpcTarget.All, bidAmount);
        MoveToNextTurn();
    }

    [PunRPC]
    private void SetCurrentAuctionBidRPC(int currentAuctionBid)
    {
        this.currentAuctionBid = currentAuctionBid;
    }

    public void MoveToNextTurn() 
    {
        photonView.RPC(nameof(MoveToNextTurnRPC), RpcTarget.All);
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
        auctionTurns.RemoveTurn(playerID);

        if (auctionTurns.OneRemainingTurn)
        {
            PlayerWonAuctionEvent?.Invoke(auctionTurns.GetFirst,currentAuctionBid);
        }
        else
        {
            MoveToNextTurn();
        }
    }
    private void OnDestroy()
    {
        BTN_AuctionBid.PlayerBiddedAtAuction -= OnPlayerBiddedAtAuction;
        BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent -= RemovePlayer;
    }
}