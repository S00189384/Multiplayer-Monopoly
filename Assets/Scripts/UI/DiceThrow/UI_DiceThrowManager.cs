using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DiceThrowManager : MonoBehaviourPun
{
    [Header("Dice Throw Displays")]
    [SerializeField] private UI_SingleDiceThrowPanel singleDiceThrowPanel;
    [SerializeField] private UI_DoubleDiceThrowPanel doubleDiceThrowPanel;

    [Header("UI Components")]
    [SerializeField] private Transform T_DicePanelSpawn;
    [SerializeField] private CanvasGroup CG_Background;

    [Header("UI Settings")]
    [SerializeField] private float timeToTransitionUIElements;
    [SerializeField] private float timeToLeaveSingleDiceThrowOnScreen;
    [SerializeField] private float timeToLeaveDoubleDiceThrowOnScreen;
    [SerializeField] private LeanTweenType tweenType;

    //Showing a single dice throw.
    public void ShowSingleDiceThrow(int randomDiceValue, string playerName,Action callback)
    {
        CG_Background.gameObject.SetActive(true);
        singleDiceThrowPanel.gameObject.SetActive(true);

        singleDiceThrowPanel.UpdateDisplay(playerName, randomDiceValue);

        LeanTween.alphaCanvas(CG_Background, 1f, timeToTransitionUIElements).setEase(tweenType);
        LeanTween.scale(singleDiceThrowPanel.gameObject,Vector3.one,timeToTransitionUIElements).setEase(tweenType)
             .setOnComplete(() => { OnSingleDiceThrowPanelReachedCenterOfScreen(callback); });

        photonView.RPC(nameof(ShowSingleDiceThrowToRemoteClients), RpcTarget.Others, playerName, randomDiceValue);
    }

    [PunRPC]
    private void ShowSingleDiceThrowToRemoteClients(string playerName,int randomDiceValue)
    {
        CG_Background.gameObject.SetActive(true);
        singleDiceThrowPanel.gameObject.SetActive(true);

        singleDiceThrowPanel.UpdateDisplay(playerName, randomDiceValue);

        LeanTween.alphaCanvas(CG_Background, 1f, timeToTransitionUIElements).setEase(tweenType);
        LeanTween.scale(singleDiceThrowPanel.gameObject, Vector3.one,timeToTransitionUIElements).setEase(tweenType)
             .setOnComplete(() => OnSingleDiceThrowPanelReachedCenterOfScreen());
    }

    private void OnSingleDiceThrowPanelReachedCenterOfScreen(Action callback = null)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(timeToLeaveSingleDiceThrowOnScreen);
        sequence.append(() =>
        {
            LeanTween.alphaCanvas(CG_Background, 0f, timeToTransitionUIElements).setEase(tweenType);
            LeanTween.scale(singleDiceThrowPanel.gameObject, Vector3.zero, timeToTransitionUIElements).setEase(tweenType)
             .setOnComplete(()=>
             { 
                 callback?.Invoke();
                 CG_Background.gameObject.SetActive(false);
                 singleDiceThrowPanel.gameObject.SetActive(false);
             });
        });
    }

    //Showing a double dice throw.
    public void ShowDoubleDiceThrowAttempt(int firstDiceValue, int secondDiceValue, string playerName, Action callback)
    {
        //Spawn.
        CG_Background.gameObject.SetActive(true);
        doubleDiceThrowPanel.gameObject.SetActive(true);

        doubleDiceThrowPanel.UpdateDisplay(playerName, firstDiceValue,secondDiceValue);

        LeanTween.alphaCanvas(CG_Background, 1f, timeToTransitionUIElements).setEase(tweenType);
        LeanTween.scale(doubleDiceThrowPanel.gameObject, Vector3.one, timeToTransitionUIElements).setEase(tweenType)
             .setOnComplete(() => { OnDoubleDiceThrowPanelReachedCenterOfScreen(callback); });

        photonView.RPC(nameof(ShowDoubleDiceThrowToRemoteClients), RpcTarget.Others, playerName, firstDiceValue,secondDiceValue);
    }
    [PunRPC]
    private void ShowDoubleDiceThrowToRemoteClients(string playerName, int firstDiceValue, int secondDiceValue)
    {
        CG_Background.gameObject.SetActive(true);
        doubleDiceThrowPanel.gameObject.SetActive(true);

        doubleDiceThrowPanel.UpdateDisplay(playerName, firstDiceValue,secondDiceValue);

        LeanTween.alphaCanvas(CG_Background, 1f, timeToTransitionUIElements).setEase(tweenType);
        LeanTween.scale(doubleDiceThrowPanel.gameObject, Vector3.one, timeToTransitionUIElements).setEase(tweenType)
             .setOnComplete(() => OnDoubleDiceThrowPanelReachedCenterOfScreen());

    }
    private void OnDoubleDiceThrowPanelReachedCenterOfScreen(Action callback = null)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(timeToLeaveDoubleDiceThrowOnScreen);
        sequence.append(() =>
        {
            LeanTween.alphaCanvas(CG_Background, 0f, timeToTransitionUIElements).setEase(tweenType);
            LeanTween.scale(doubleDiceThrowPanel.gameObject, Vector3.zero, timeToTransitionUIElements).setEase(tweenType)
             .setOnComplete(() =>
             {
                 callback?.Invoke();
                 CG_Background.gameObject.SetActive(false);
                 doubleDiceThrowPanel.gameObject.SetActive(false);
             });
        });
    }

    public int GetRandomDiceValue()
    {
        return UnityEngine.Random.Range(1, 7);
    }
}