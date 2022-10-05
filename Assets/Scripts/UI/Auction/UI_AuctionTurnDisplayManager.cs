using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Script which spawns in player icons representing each player in the auction and their turn.
//It also tells the player whos turn it is in the auction.

public class UI_AuctionTurnDisplayManager : MonoBehaviourPunCallbacks
{
    private bool LocalPlayerHasFolded;

    [SerializeField] private TextMeshProUGUI TMP_TurnDisplay;

    [SerializeField] private Transform playerTurnDisplaySpawnTransform;
    [SerializeField] private UI_AuctionPlayerTurnIcon playerTurnIconPrefab;

    [SerializeField] private Dictionary<string, UI_AuctionPlayerTurnIcon> SpawnedPlayerTurnIconsDictionary = new Dictionary<string, UI_AuctionPlayerTurnIcon>();
    private string previousPlayerTurn;

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
        AuctionTurnManager.NewPlayerAuctionTurnEvent += OnNewPlayerAuctionTurn;
        AuctionTurnManager.PlayerWonAuctionEvent += OnPlayerWonAuction;
        BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent += OnPlayerFoldedFromAuction;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SpawnedPlayerTurnIconsDictionary[otherPlayer.UserId].ChangeToFolded();
    }

    private void OnPlayerWonAuction(string playerIDThatWonAuction,int finalBid,AuctionType auctionType)
    {
        if(PhotonNetwork.LocalPlayer.UserId == playerIDThatWonAuction)
        {
            TMP_TurnDisplay.text = $"You won the auction";
            TMP_TurnDisplay.color = Color.green;
        }
    }

    private void OnPlayerFoldedFromAuction(string playerIDThatFolded)
    {
        TMP_TurnDisplay.text = $"You have folded";
        TMP_TurnDisplay.color = Color.red;

        LocalPlayerHasFolded = true;

        photonView.RPC(nameof(OnPlayerFoldedFromAuctionRPC), RpcTarget.All, playerIDThatFolded);
    }

    [PunRPC]
    private void OnPlayerFoldedFromAuctionRPC(string playerIDThatFolded)
    {
        SpawnedPlayerTurnIconsDictionary[playerIDThatFolded].ChangeToFolded();
    }


    private void OnNewPlayerAuctionTurn(string newPlayerTurn,int currentBid)
    {
        if (!string.IsNullOrEmpty(previousPlayerTurn))
            SpawnedPlayerTurnIconsDictionary[previousPlayerTurn].ChangeOutlineColour(Color.white);

        SpawnedPlayerTurnIconsDictionary[newPlayerTurn].ChangeOutlineColour(Color.green);

        previousPlayerTurn = newPlayerTurn;

        if (LocalPlayerHasFolded) //If I have folded, don't update my text in response to the auction turn changing.
            return;

        //Update text.
        bool isMyTurn = newPlayerTurn == PhotonNetwork.LocalPlayer.UserId;
        if (isMyTurn)
        {
            TMP_TurnDisplay.text = $"Your turn to bid";
            TMP_TurnDisplay.color = Color.green;
        }
        else
        {
            TMP_TurnDisplay.text = $"Waiting for {GameManager.Instance.GetPlayerNicknameFromID(newPlayerTurn)} to bid";
            TMP_TurnDisplay.color = Color.red;
        }
    }

    public void SpawnPlayerIcon(string playerID,Sprite playerSprite)
    {
        UI_AuctionPlayerTurnIcon spawnedPlayerIcon = Instantiate(playerTurnIconPrefab, playerTurnDisplaySpawnTransform);
        spawnedPlayerIcon.UpdateDisplay(playerSprite);
        SpawnedPlayerTurnIconsDictionary.Add(playerID, spawnedPlayerIcon);
    }

    public void SpawnActivePlayersDisplay(List<string> activePlayerIDs)
    {
        for (int i = 0; i < activePlayerIDs.Count; i++)
        {
            SpawnPlayerIcon(activePlayerIDs[i],GameManager.Instance.GetPlayerSprite(activePlayerIDs[i]));
        }
    }
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        AuctionTurnManager.NewPlayerAuctionTurnEvent -= OnNewPlayerAuctionTurn;
        AuctionTurnManager.PlayerWonAuctionEvent -= OnPlayerWonAuction;
        BTN_FoldFromAuction.PlayerFoldedFromAuctionEvent -= OnPlayerFoldedFromAuction;
    }
}