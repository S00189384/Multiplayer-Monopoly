using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


//Class which spawns displays to notify the player on what has happened throughout an auction.
//"Player x has folded", "player x has bid y amount" etc.

public class UI_AuctionReporter : MonoBehaviourPunCallbacks
{
    //To ensure that the player won auction display remains at the top of the list, hold a reference to it so we can set to to be the last sibling later.
    private Transform spawnedPlayerWonDisplayTransform;

    [Header("Components")]
    [Tooltip("The transform that auction event displays will be children of.")]
    [SerializeField] private Transform contentTransform;

    [Header("Prefabs")]
    [Tooltip("The prefab for displaying an auction event.")]
    [SerializeField] private UI_AuctionEventDisplay eventDisplayPrefab;

    private void Awake()
    {
        BTN_AuctionBid.PlayerBiddedAtAuction += ReportPlayerBid;
        BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent += ReportPlayerFold;
        AuctionTurnManager.PlayerWonAuctionEvent += ReportPlayerWin;
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ReportPlayerDisconnect(otherPlayer.UserId);
    }

    //Fold.
    private void ReportPlayerFold(string playerIDThatFolded)
    {
        photonView.RPC(nameof(ReportPlayerFoldRPC), RpcTarget.All, playerIDThatFolded);
    }

    [PunRPC]
    private void ReportPlayerFoldRPC(string playerIDThatFolded)
    {
        string playerNameThatFolded = GameManager.Instance.GetPlayerIdentityDisplay(playerIDThatFolded);

        UI_AuctionEventDisplay spawnedEventDisplay = ReportAuctionEvent($"{playerNameThatFolded} folded.", Color.red);
        if (spawnedPlayerWonDisplayTransform)
        {
            spawnedPlayerWonDisplayTransform.SetAsLastSibling();
        }
    }

    private void ReportPlayerDisconnect(string playerIDThatDisconnected)
    {
        string playerNameThatDisconnected = GameManager.Instance.GetPlayerIdentityDisplay(playerIDThatDisconnected);

        UI_AuctionEventDisplay spawnedEventDisplay = ReportAuctionEvent($"{playerNameThatDisconnected} disconnected.", Color.red);
        if (spawnedPlayerWonDisplayTransform)
        {
            spawnedPlayerWonDisplayTransform.SetAsLastSibling();
        }
    }

    private void ReportPlayerBid(string playerIDThatBidded, int bidAmount)
    {
        photonView.RPC(nameof(ReportPlayerBidRPC), RpcTarget.All, playerIDThatBidded, bidAmount);
    }

    [PunRPC]
    private void ReportPlayerBidRPC(string playerIDThatBidded, int bidAmount)
    {
        string playerNameBidded = GameManager.Instance.GetPlayerIdentityDisplay(playerIDThatBidded);
        ReportAuctionEvent($"{playerNameBidded} Bidded ${bidAmount}", Color.white);
    }

    //Win.
    private void ReportPlayerWin(string playerIDThatWon,int finalBid,AuctionType auctionType)
    {
        string playerNameThatWon = GameManager.Instance.GetPlayerIdentityDisplay(playerIDThatWon);
        spawnedPlayerWonDisplayTransform = ReportAuctionEvent($"{playerNameThatWon} won the auction.", Color.green).transform;
    }

    private UI_AuctionEventDisplay ReportAuctionEvent(string eventText,Color eventColour)
    {
        UI_AuctionEventDisplay spawnedEventDisplay = Instantiate(eventDisplayPrefab, contentTransform);
        spawnedEventDisplay.UpdateDisplay(eventColour, eventText);
        return spawnedEventDisplay;
    }
    private void OnDestroy()
    {
        BTN_AuctionBid.PlayerBiddedAtAuction -= ReportPlayerBid;
        BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent -= ReportPlayerFold;
        AuctionTurnManager.PlayerWonAuctionEvent -= ReportPlayerWin;
    }
}