using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_CardManager : MonoBehaviourPun
{
    public static UI_CardManager Instance;

    private const string communityChestPrefabResourcePath = "UI/Cards/UI_CommunityChestCard";
    private const string chancePrefabResourcePath = "UI/Cards/UI_ChanceCard";

    [Header("UI Components")]
    [SerializeField] private GameObject mainUIObject;
    [SerializeField] private CanvasGroup CG_Background;
    [SerializeField] private TextMeshProUGUI TMP_Header;

    [Header("Component Target Transforms For Moving")]
    [SerializeField] private Transform headerTargetTransform;
    [SerializeField] private Transform cardSpawnTransform;
    [SerializeField] private Transform cardTargetTransform;  
    private Vector3 headerOffScreenPosition;

    [Header("UI Sequence Settings")]
    [SerializeField] private float timeToFadeBackground = 0.6f;
    [SerializeField] private float timeToMoveUIElements = 0.7f;
    [SerializeField] private float timeToLeaveCardOnScreen = 2.7f;

    private GameObject spawnedCard;
    private CardData cardDataToExecute;

    [Header("Cards to Use")]
    [SerializeField] private CardPile communityChestCardsPile;
    [SerializeField] private CardPile chanceCardsPile;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        headerOffScreenPosition = TMP_Header.transform.position;
    }

    ////Testing.
    //private void Update()
    //{
    //    //if (PhotonNetwork.IsMasterClient)
    //    //{
    //    //    if (Input.GetKeyDown(KeyCode.Space))
    //    //    {
    //    //        photonView.RPC(nameof(ShowCommunityChestCard), RpcTarget.All, PhotonNetwork.MasterClient.UserId);
    //    //        //ShowCommunityChestCard(PhotonNetwork.MasterClient.UserId);
    //    //    }
    //    //}
    //}

    public void ShowCommunityChestCard(string playerID)
    {
        photonView.RPC(nameof(ShowCommunityChestCardRPC), RpcTarget.All, playerID);
    }

    [PunRPC]
    private void ShowCommunityChestCardRPC(string playerID)
    {
        cardDataToExecute = communityChestCardsPile.GrabCard();

        mainUIObject.SetActive(true);

        TMP_Header.text = $"{GameManager.Instance.GetPlayerIdentityDisplay(playerID)} received a community chest card";

        //Player that drawed card spawns ui card and rpc's other clients to start the UI sequence.
        if (playerID == PhotonNetwork.LocalPlayer.UserId)
        {
            GameObject spawnedCard = PhotonNetwork.Instantiate(communityChestPrefabResourcePath, cardSpawnTransform.position, Quaternion.identity);
            int viewIdOfSpawnedCard = spawnedCard.GetPhotonView().ViewID;
            photonView.RPC(nameof(OnCardSpawned), RpcTarget.All, viewIdOfSpawnedCard);
        }
    }

    public void ShowChanceCard(string playerID)
    {
        photonView.RPC(nameof(ShowChanceCardRPC), RpcTarget.All, playerID);
    }

    [PunRPC]
    private void ShowChanceCardRPC(string playerID)
    {
        cardDataToExecute = chanceCardsPile.GrabCard();

        mainUIObject.SetActive(true);

        TMP_Header.text = $"{GameManager.Instance.GetPlayerIdentityDisplay(playerID)} received a chance card";

        //Player that drawed card spawns ui card and rpc's other clients to start the UI sequence.
        if (playerID == PhotonNetwork.LocalPlayer.UserId)
        {
            GameObject spawnedCard = PhotonNetwork.Instantiate(chancePrefabResourcePath, cardSpawnTransform.position, Quaternion.identity);
            int viewIdOfSpawnedCard = spawnedCard.GetPhotonView().ViewID;
            photonView.RPC(nameof(OnCardSpawned), RpcTarget.All, viewIdOfSpawnedCard);
        }
    }

    [PunRPC]
    private void OnCardSpawned(int viewIdOfSpawnedCard)
    {
        spawnedCard = PhotonNetwork.GetPhotonView(viewIdOfSpawnedCard).gameObject;
        FixDisplayOfSpawnedCard(spawnedCard.transform);

        spawnedCard.GetComponent<UI_Card>().UpdateDisplay(cardDataToExecute);

        StartCardUISequence();
    }

    private void StartCardUISequence()
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(LeanTween.alphaCanvas(CG_Background, 1f, timeToFadeBackground));
        sequence.append(LeanTween.move(TMP_Header.gameObject, headerTargetTransform.position, timeToMoveUIElements).setEase(LeanTweenType.easeInOutSine));
        sequence.append(0.9f);
        sequence.append(LeanTween.move(spawnedCard, cardTargetTransform.position, timeToMoveUIElements).setEase(LeanTweenType.easeInOutSine));
        sequence.append(timeToLeaveCardOnScreen);
        sequence.append(() =>
        {
            LeanTween.move(TMP_Header.gameObject, headerOffScreenPosition, timeToMoveUIElements).setEase(LeanTweenType.easeInOutSine);
            LeanTween.move(spawnedCard, cardSpawnTransform.position, timeToMoveUIElements).setEase(LeanTweenType.easeInOutSine);
            LeanTween.alphaCanvas(CG_Background, 0, timeToFadeBackground).setOnComplete(OnFinishedShowingCardSequence);
        });
    }
    private void OnFinishedShowingCardSequence()
    {
        mainUIObject.SetActive(false);
        Destroy(spawnedCard);

        //Only execute card method on client that drawed the card / is their turn.
        string currentPlayerTurn = PlayerTurnManager.Instance.GetCurrentTurn;
        if(currentPlayerTurn == PhotonNetwork.LocalPlayer.UserId)
            cardDataToExecute.Execute(currentPlayerTurn);
    }

    private void FixDisplayOfSpawnedCard(Transform spawnedCardTransform)
    {
        spawnedCardTransform.SetParent(this.transform);
        spawnedCardTransform.localEulerAngles = Vector3.zero;
        spawnedCardTransform.localScale = Vector3.one;
    }
}