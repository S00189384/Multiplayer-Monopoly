using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_BankrupcyConfirmation : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private UI_BankrupcyConfirmationPrompt confirmationPanelPrefab;

    [Header("UI Components")]
    [SerializeField] private GameObject mainUIObject;
    [SerializeField] private CanvasGroup CG_Background;

    [Header("Component Target Transforms For Moving")]
    [SerializeField] private Transform confirmpromptSpawnTransform;

    [Header("UI Sequence Settings")]
    [SerializeField] private float timeToFadeBackground = 0.5f;
    [SerializeField] private float timeToMoveUIElements = 0.5f;
    [SerializeField] private LeanTweenType tweenType;

    private GameObject GO_spawnedPrompt;

    public void ShowConfirmationPrompt()
    {
        mainUIObject.SetActive(true);

        UI_BankrupcyConfirmationPrompt spawnedPrompt = Instantiate(confirmationPanelPrefab, confirmpromptSpawnTransform.position, Quaternion.identity, this.transform);
        spawnedPrompt.UpdateDisplay(OnNoButtonClickedOnPanel, OnYesButtonClickedOnPanel, true);
        GO_spawnedPrompt = spawnedPrompt.gameObject;
        GO_spawnedPrompt.transform.localEulerAngles = Vector3.zero;

        LeanTween.alphaCanvas(CG_Background, 1f, timeToMoveUIElements).setEase(tweenType);
        LeanTween.move(GO_spawnedPrompt, transform.position, timeToMoveUIElements).setEase(tweenType);
    }

    private void OnNoButtonClickedOnPanel()
    {
        LeanTween.alphaCanvas(CG_Background, 0f, timeToMoveUIElements).setEase(tweenType);
        LeanTween.move(GO_spawnedPrompt, confirmpromptSpawnTransform.position, timeToMoveUIElements).setEase(tweenType)
            .setOnComplete(() => 
            {
                mainUIObject.SetActive(false);
                Destroy(GO_spawnedPrompt);
            });
    }
    private void OnYesButtonClickedOnPanel()
    {
        LeanTween.alphaCanvas(CG_Background, 0f, timeToMoveUIElements).setEase(tweenType);
        LeanTween.move(GO_spawnedPrompt, confirmpromptSpawnTransform.position, timeToMoveUIElements).setEase(tweenType)
            .setOnComplete(()=> 
            {
                Destroy(GO_spawnedPrompt);

                UI_NotificationManager.Instance.RPC_ShowNotificationWithLocalCallback($"{GameManager.Instance.GetPlayerNicknameFromID(PhotonNetwork.LocalPlayer.UserId)} declared bankrupcy!",
                    callback: () => 
                    {
                        Bank.Instance.BankruptPlayer(PhotonNetwork.LocalPlayer.UserId);
                        mainUIObject.SetActive(false);
                    });
            });
    }
}
