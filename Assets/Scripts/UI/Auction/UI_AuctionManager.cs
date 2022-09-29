using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AuctionManager : MonoBehaviourPunCallbacks
{
    private const string singleTilePromptResourcesPrefabPath = "UI/AuctionPromptSingleTile";

    [Header("UI Components")]
    [SerializeField] private GameObject GO_auctionUI;
    [SerializeField] private CanvasGroup CG_Background;
    [SerializeField] private TextMeshProUGUI TMP_Header;

    [Header("Component Target Transforms For Moving")]
    [SerializeField] private Transform headerTargetTransform;
    [SerializeField] private Transform auctionPanelSpawnTransform;
    private Vector3 headerOffScreenPosition;

    [Header("UI Sequence Settings")]
    [SerializeField] private float timeToFadeBackground = 0.4f;
    [SerializeField] private float timeToMoveUIElements = 0.7f;
    [SerializeField] private float delayOfClearingUIAfterPlayerWonAuction = 1.8f;
    [SerializeField] private LeanTweenType tweenType;

    private GameObject spawnedSingleTileAuctionPrompt;
    private int photonIDOfTileForAuction;

    public Queue<string> playersToAuctionPossessionsQueue = new Queue<string>();

    //Start.
    private void Awake()
    {
        BTN_StartAuction.AuctionStartedEvent += OnSingleTileAuction;
        AuctionTurnManager.PlayerWonAuctionEvent += OnPlayerWonAuction;
        Bank.BankruptedPlayerEvent += OnPlayerDeclaredBankrupcy;
    }

    private void Start()
    {
        headerOffScreenPosition = TMP_Header.transform.position;
    }

    //Auctioning player possessions on disconnect / bankrupcy.
    private void OnPlayerDeclaredBankrupcy(string playerID) => AddToAuctionPlayerPossessionsQueue(playerID);
    public override void OnPlayerLeftRoom(Player otherPlayer) => AddToAuctionPlayerPossessionsQueue(otherPlayer.UserId);

    private void AddToAuctionPlayerPossessionsQueue(string playerID)
    {
        if(playersToAuctionPossessionsQueue.Count <= 0)
            PlayerTurnManager.PlayerFinishedTurnEvent += OnPlayerFinishedTurn;

        playersToAuctionPossessionsQueue.Enqueue(playerID);
    }

    private void OnPlayerFinishedTurn()
    {
        print($"auction manager spawns UI for possession auction now {playersToAuctionPossessionsQueue.Count}");
    }

    private void OnPlayerWonAuction(string playerIDThatWonAuction,int finalBid)
    {
        var sequence = LeanTween.sequence();
        sequence.append(delayOfClearingUIAfterPlayerWonAuction);
        sequence.append(() =>
        {
            MoveAuctionPanelOffScreen();

            if(playerIDThatWonAuction == PhotonNetwork.LocalPlayer.UserId)
                Bank.Instance.ProcessPlayerTilePurchase(playerIDThatWonAuction, photonIDOfTileForAuction, finalBid);
        });
    }

    private void OnSingleTileAuction(string playerIDThatStartedAuction, int photonIDOfTileForAuction)
    {
        GameObject spawnedPrompt = PhotonNetwork.Instantiate(singleTilePromptResourcesPrefabPath, auctionPanelSpawnTransform.position, Quaternion.identity);
        int photonIDOfSpawnedPrompt = spawnedPrompt.GetComponent<PhotonView>().ViewID;

        photonView.RPC(nameof(FixDisplayOfAuctionPrompt), RpcTarget.All, photonIDOfSpawnedPrompt);
        photonView.RPC(nameof(InitialiseAuctionPrompt), RpcTarget.All, photonIDOfSpawnedPrompt, playerIDThatStartedAuction, photonIDOfTileForAuction);
    }

    [PunRPC]
    private void FixDisplayOfAuctionPrompt(int photonIDOfSpawnedPrompt)
    {
        spawnedSingleTileAuctionPrompt = PhotonNetwork.GetPhotonView(photonIDOfSpawnedPrompt).gameObject;
        spawnedSingleTileAuctionPrompt.transform.SetParent(this.transform);
        spawnedSingleTileAuctionPrompt.transform.localScale = Vector3.one;
        spawnedSingleTileAuctionPrompt.transform.localEulerAngles = Vector3.zero;
    }
    [PunRPC]
    private void InitialiseAuctionPrompt(int spawnedPromptPhotonID,string playerIDThatStartedAuction,int photonIDOfTile)
    {
        this.photonIDOfTileForAuction = photonIDOfTile;

        TMP_Header.text = $"{GameManager.Instance.GetPlayerNicknameFromID(playerIDThatStartedAuction)} started an auction!";
        GO_auctionUI.SetActive(true);
        PhotonNetwork.GetPhotonView(spawnedPromptPhotonID).GetComponent<UI_AuctionPromptSinglePurchasableTile>().InitialisePrompt(playerIDThatStartedAuction, photonIDOfTile);
        MoveAuctionPanelOnScreen();
    }
    private void MoveAuctionPanelOnScreen()
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(LeanTween.alphaCanvas(CG_Background, 1f, timeToFadeBackground));
        sequence.append(LeanTween.move(TMP_Header.gameObject, headerTargetTransform.position, timeToMoveUIElements).setEase(tweenType));
        sequence.append(0.7f);
        sequence.append(LeanTween.move(spawnedSingleTileAuctionPrompt, transform.position, timeToMoveUIElements).setEase(tweenType));
    }
    private void MoveAuctionPanelOffScreen()
    {
        LeanTween.alphaCanvas(CG_Background, 0f, timeToFadeBackground);
        LeanTween.move(TMP_Header.gameObject, headerOffScreenPosition, timeToMoveUIElements).setEase(tweenType);
        LeanTween.move(spawnedSingleTileAuctionPrompt, auctionPanelSpawnTransform.position, timeToMoveUIElements).setEase(tweenType)
            .setOnComplete(()=> 
            {
                GO_auctionUI.SetActive(false);
                Destroy(spawnedSingleTileAuctionPrompt);  
            });
    }

    private void OnDestroy()
    {
        BTN_StartAuction.AuctionStartedEvent -= OnSingleTileAuction;
        AuctionTurnManager.PlayerWonAuctionEvent -= OnPlayerWonAuction;
        Bank.BankruptedPlayerEvent -= OnPlayerDeclaredBankrupcy;
        PlayerTurnManager.PlayerFinishedTurnEvent -= OnPlayerFinishedTurn;
    }
}