using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

//UI Script that listens for the game over event and spawns a prompt informing the player of who won and options to return to the main menu or quit.

public class UI_GameOver : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject GO_UI;
    [SerializeField] private CanvasGroup CG_Background;
    [SerializeField] private Transform T_PromptSpawnTransform;

    [Header("Prefabs")]
    [SerializeField] private UI_GameOverPrompt gameOverPromptPrefab;
    private UI_GameOverPrompt spawnedGameOverPrompt;

    [Header("UI Transition Settings")]
    [SerializeField] private float timeToTransition;
    [SerializeField] private LeanTweenType tweenType;

    private void Awake()
    {
        GameManager.PlayerWonGameEvent += OnPlayerWonGame;
    }

    private void OnPlayerWonGame(string playerIDThatWon, List<string> otherPlayersList)
    {
        PhotonNetwork.AutomaticallySyncScene = false;

        spawnedGameOverPrompt = Instantiate(gameOverPromptPrefab, T_PromptSpawnTransform.position, Quaternion.identity, this.transform);
        spawnedGameOverPrompt.transform.localEulerAngles = Vector3.zero;
        spawnedGameOverPrompt.UpdateDisplay(playerIDThatWon);

        MovePromptOnScreen();
    }
    private void MovePromptOnScreen()
    {
        GO_UI.SetActive(true);

        LeanTween.alphaCanvas(CG_Background, 1f, timeToTransition).setEase(tweenType);
        LeanTween.move(spawnedGameOverPrompt.gameObject, transform.position, timeToTransition).setEase(tweenType).setOnComplete(() => Time.timeScale = 0);
    }

    private void OnDestroy()
    {
        GameManager.PlayerWonGameEvent -= OnPlayerWonGame;
    }
}