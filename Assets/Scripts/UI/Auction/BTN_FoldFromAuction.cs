using Photon.Pun;
using System;

//Button to fold from an auction. 
//Player can fold even if not their turn. 
//Disabled if player is bankrupt and only spectating the auction 

public class BTN_FoldFromAuction : BTN_Base
{
    public static event Action<string> PlayerFoldedFromAuctionEvent;

    public override void Awake()
    {
        if (GameManager.Instance.LocalPlayerIsAnActivePlayer)
        {
            base.Awake();

            AddOnClickListener(FoldFromAuction);
            AuctionTurnManager.PlayerWonAuctionEvent += OnPlayerWonAuction;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnPlayerWonAuction(string playerIDThatWonAuction,int finalBid,AuctionType auctionType)
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