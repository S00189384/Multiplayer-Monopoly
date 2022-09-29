using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameOver : MonoBehaviour
{
    private const string gameOverUIPrefabResourcePath = "UI/GameOver/GameOverPrompt";

    [SerializeField] private UI_GameOverPrompt gameOverPromptPrefab;

    [SerializeField] private GameObject GO_UI;
    [SerializeField] private CanvasGroup CG_Background;
    [SerializeField] private Transform T_PromptSpawnTransform;

    //private GameObject spawnedGameOverPrompt;
    private UI_GameOverPrompt spawnedGameOverPrompt;

    private void Awake()
    {
        GameManager.PlayerWonGameEvent += OnPlayerWonGame;
    }

    //private void OnPlayerWonGame(string winningPlayerID, List<string> otherPlayersIDList)
    //{
    //    PhotonNetwork.AutomaticallySyncScene = false;

    //    GameObject spawnedPrompt = PhotonNetwork.Instantiate(gameOverUIPrefabResourcePath, T_PromptSpawnTransform.position, Quaternion.identity);
    //    int promptPhotonID = spawnedPrompt.GetComponent<PhotonView>().ViewID;
    //    photonView.RPC(nameof(OnPromptSpawned), RpcTarget.All, promptPhotonID);
    //}

    //[PunRPC]
    //private void OnPromptSpawned(int photonIdOfPrompt)
    //{
    //    spawnedGameOverPrompt = PhotonNetwork.GetPhotonView(photonIdOfPrompt).gameObject;
    //    FixDisplayOfSpawnedPrompt(spawnedGameOverPrompt.transform);

    //    MovePromptOnScreen();
    //}

    private void MovePromptOnScreen()
    {
        GO_UI.SetActive(true);

        LeanTween.alphaCanvas(CG_Background, 1f, 0.6f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.move(spawnedGameOverPrompt.gameObject, transform.position, 0.6f).setEase(LeanTweenType.easeInOutSine);
    }

    private void FixDisplayOfSpawnedPrompt(Transform spawnedCardTransform)
    {
        spawnedCardTransform.SetParent(this.transform);
        spawnedCardTransform.transform.position = T_PromptSpawnTransform.position;
        spawnedCardTransform.localEulerAngles = Vector3.zero;
        spawnedCardTransform.localScale = Vector3.one;
    }

    private void OnPlayerWonGame(string playerIDThatWon,List<string> otherPlayersList)
    {
        PhotonNetwork.AutomaticallySyncScene = false;

        spawnedGameOverPrompt = Instantiate(gameOverPromptPrefab, T_PromptSpawnTransform.position, Quaternion.identity, this.transform);
        spawnedGameOverPrompt.transform.localEulerAngles = Vector3.zero;
        spawnedGameOverPrompt.UpdateDisplay(playerIDThatWon);

        MovePromptOnScreen();
    }
    //public void OnEvent(EventData photonEvent)
    //{
    //    if (photonEvent.Code == EventCodes.PlayerWonGameEvent)
    //    {
    //        string playerIDThatWonGame = (string)photonEvent.CustomData;

    //        OnPlayerWonGame(playerIDThatWonGame);
    //    }
    //}

    private void OnDestroy()
    {
        //PhotonNetwork.RemoveCallbackTarget(this);
        GameManager.PlayerWonGameEvent -= OnPlayerWonGame;
    }

}
