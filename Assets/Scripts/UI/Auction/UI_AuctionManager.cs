using Photon.Pun;
using TMPro;
using UnityEngine;
public enum AuctionType { SingleTile, PlayerPossessions }

//Handles the spawning of an auction prompt and it moving on / off the screen. 

public class UI_AuctionManager : MonoBehaviourPunCallbacks
{
    private const string singleTilePromptResourcesPrefabPath = "UI/AuctionPromptSingleTile";
    private const string playerPossessionsPromptResourcesPrefabPath = "UI/AuctionPromptPlayerPossessions";

    [Header("UI Components")]
    [SerializeField] private GameObject GO_auctionUI;
    [SerializeField] private CanvasGroup CG_Background;
    [SerializeField] private TextMeshProUGUI TMP_Header;

    [Header("Component Target Transforms For Moving")]
    [SerializeField] private Transform headerTargetTransform;
    [SerializeField] private Transform auctionPanelSpawnTransform;
    private Vector3 headerOffScreenPosition;

    [Header("UI Sequence Settings")]
    [SerializeField] private float timeToFadeBackground = 0.7f;
    [SerializeField] private float timeToMoveUIElements = 0.8f;
    [SerializeField] private float delayOfClearingUIAfterPlayerWonAuction = 1.8f;
    [SerializeField] private LeanTweenType tweenType;

    private GameObject spawnedSingleTileAuctionPrompt;
    private int photonIDOfTileForAuction;
    private string playerIDThatBankruptedDueToBank;

    //Start.
    private void Awake()
    {
        BTN_StartAuction.AuctionStartedEvent += OnSingleTileAuction;
        AuctionTurnManager.PlayerWonAuctionEvent += OnPlayerWonAuction;
        Bank.PlayerDeclaredBankruptDueToBankPaymentEvent += OnPlayerDeclaredBankruptDueToBankPayment;
    }

    private void OnPlayerDeclaredBankruptDueToBankPayment(string playerID)
    {
        OwnedPlayerTileTracker ownedPlayerTileTracker = TileOwnershipManager.Instance.GetOwnedPlayerTileTracker(playerID);
        PlayerInventory playerInventory = GameManager.Instance.GetPlayerPieceByID(playerID).GetComponent<PlayerInventory>();

        if (ownedPlayerTileTracker.OwnsAPurchasableTile || playerInventory.HasAGetOutOfJailFreeCard)       
            StartPlayerPossessionsAuction(playerID);      
    }

    private void Start()
    {
        headerOffScreenPosition = TMP_Header.transform.position;
    }

    private void StartPlayerPossessionsAuction(string playerID)
    {
        GameObject spawnedPrompt = PhotonNetwork.Instantiate(playerPossessionsPromptResourcesPrefabPath, auctionPanelSpawnTransform.position, Quaternion.identity);
        int photonIDOfSpawnedPrompt = spawnedPrompt.GetComponent<PhotonView>().ViewID;

        photonView.RPC(nameof(FixDisplayOfAuctionPrompt), RpcTarget.All, photonIDOfSpawnedPrompt);
        photonView.RPC(nameof(InitialisePlayersPossessionsAuctionPrompt), RpcTarget.All, photonIDOfSpawnedPrompt, playerID);
    }

    private void OnPlayerWonAuction(string playerIDThatWonAuction,int finalBid,AuctionType auctionType)
    {
        var sequence = LeanTween.sequence();
        sequence.append(delayOfClearingUIAfterPlayerWonAuction);
        sequence.append(() =>
        {
            MoveAuctionPanelOffScreen();

            if(playerIDThatWonAuction == PhotonNetwork.LocalPlayer.UserId)
            {
                switch (auctionType)
                {
                    case AuctionType.SingleTile:
                        Bank.Instance.ProcessPlayerTilePurchase(playerIDThatWonAuction, photonIDOfTileForAuction, finalBid);
                        break;
                    case AuctionType.PlayerPossessions:
                        TileOwnershipManager.Instance.TransferAllPlayerOwnedTilesToAnotherPlayer(playerIDThatBankruptedDueToBank, playerIDThatWonAuction);
                        break;
                    default:
                        break;
                }
            }
        });
    }

    private void OnSingleTileAuction(string playerIDThatStartedAuction, int photonIDOfTileForAuction)
    {
        GameObject spawnedPrompt = PhotonNetwork.Instantiate(singleTilePromptResourcesPrefabPath, auctionPanelSpawnTransform.position, Quaternion.identity);
        int photonIDOfSpawnedPrompt = spawnedPrompt.GetComponent<PhotonView>().ViewID;

        photonView.RPC(nameof(FixDisplayOfAuctionPrompt), RpcTarget.All, photonIDOfSpawnedPrompt);
        photonView.RPC(nameof(InitialiseSingleTileAuctionPrompt), RpcTarget.All, photonIDOfSpawnedPrompt, playerIDThatStartedAuction, photonIDOfTileForAuction);
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
    private void InitialiseSingleTileAuctionPrompt(int spawnedPromptPhotonID,string playerIDThatStartedAuction,int photonIDOfTile)
    {
        this.photonIDOfTileForAuction = photonIDOfTile;

        TMP_Header.text = $"{GameManager.Instance.GetPlayerNicknameFromID(playerIDThatStartedAuction)} started an auction!";
        GO_auctionUI.SetActive(true);
        PhotonNetwork.GetPhotonView(spawnedPromptPhotonID).GetComponent<UI_AuctionPromptSinglePurchasableTile>().InitialisePrompt(playerIDThatStartedAuction, photonIDOfTile);
        MoveAuctionPanelOnScreen();
    }

    [PunRPC]
    private void InitialisePlayersPossessionsAuctionPrompt(int spawnedPromptPhotonID, string playerID)
    {
        playerIDThatBankruptedDueToBank = playerID;
        TMP_Header.text = $"{GameManager.Instance.GetPlayerNicknameFromID(playerID)} is bankrupt and their possessions are up for auction!";
        GO_auctionUI.SetActive(true);
        PhotonNetwork.GetPhotonView(spawnedPromptPhotonID).GetComponent<UI_AuctionPromptPlayerPossessions>().InitialisePrompt(playerID);
        MoveAuctionPanelOnScreen();
    }
    private void MoveAuctionPanelOnScreen()
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(LeanTween.alphaCanvas(CG_Background, 1f, timeToFadeBackground));
        sequence.append(LeanTween.move(TMP_Header.gameObject, headerTargetTransform.position, timeToMoveUIElements).setEase(tweenType));
        sequence.append(1.2f);
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
        Bank.PlayerDeclaredBankruptDueToBankPaymentEvent -= OnPlayerDeclaredBankruptDueToBankPayment;
    }
}
