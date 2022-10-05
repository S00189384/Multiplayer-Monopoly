using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

//Manager for spawning a notification that darkens the screen and shows text to the player.

public class UI_NotificationManager : MonoBehaviourPun
{
    public static UI_NotificationManager Instance;

    [SerializeField] private CanvasGroup CG_Background;
    [SerializeField] private TextMeshProUGUI TMP_Notification;
    [SerializeField] private Transform spawnTransform;

    private const float timeToLeaveNotificationOnScreen = 2.5f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void RPC_ShowNotification(string message,RpcTarget rpcTarget)
    {
        photonView.RPC(nameof(ShowNotification), rpcTarget, message);
    }
    public void RPC_ShowNotificationWithLocalCallback(string message, Action callback)
    {
        ShowNotification(message, callback);
        photonView.RPC(nameof(ShowNotification), RpcTarget.Others, message);
    }


    [PunRPC]
    public void ShowNotification(string message)
    {
        CG_Background.blocksRaycasts = true;
        TMP_Notification.text = message;

        LeanTween.alphaCanvas(CG_Background, 1f, 0.4f);
        LeanTween.move(TMP_Notification.gameObject, transform.position, 0.4f).setEase(LeanTweenType.easeInOutSine).setOnComplete
            (() => 
            {
                LeanTween.alphaCanvas(CG_Background, 0f, 0.4f).setDelay(timeToLeaveNotificationOnScreen);
                LeanTween.move(TMP_Notification.gameObject, spawnTransform.position, 0.4f).setEase(LeanTweenType.easeInOutSine).setDelay(timeToLeaveNotificationOnScreen).setOnComplete(OnFinishedNotification);
            });
    }

    public void ShowNotification(string message,Action callback)
    {
        CG_Background.blocksRaycasts = true;
        TMP_Notification.text = message;

        LeanTween.alphaCanvas(CG_Background, 1f, 0.4f);
        LeanTween.move(TMP_Notification.gameObject, transform.position, 0.4f).setEase(LeanTweenType.easeInOutSine).setOnComplete
            (() =>
            {
                LeanTween.alphaCanvas(CG_Background, 0f, 0.4f).setDelay(timeToLeaveNotificationOnScreen);
                LeanTween.move(TMP_Notification.gameObject, spawnTransform.position, 0.4f).setEase(LeanTweenType.easeInOutSine).setDelay(timeToLeaveNotificationOnScreen).setOnComplete(() => { OnFinishedNotification();callback?.Invoke(); });
            });
    }

    private void OnFinishedNotification()
    {
        TMP_Notification.text = string.Empty;
        CG_Background.blocksRaycasts = false;
    }
}