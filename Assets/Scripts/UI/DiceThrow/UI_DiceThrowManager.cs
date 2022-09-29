using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DiceThrowManager : MonoBehaviourPun
{
    [SerializeField] private UI_SingleDiceThrowPanel singleDiceThrowPanel;
    [SerializeField] private UI_DoubleDiceThrowPanel doubleDiceThrowPanel;

    [SerializeField] private Transform T_DicePanelSpawn;
    [SerializeField] private CanvasGroup CG_Background;

    public void ShowSingleDiceThrow(int randomDiceValue, string playerName,Action callback)
    {
        //Spawn.
        CG_Background.gameObject.SetActive(true);
        singleDiceThrowPanel.gameObject.SetActive(true);

        singleDiceThrowPanel.UpdateDisplay(playerName, randomDiceValue);

        LeanTween.alphaCanvas(CG_Background, 1f, 0.5f);
        LeanTween.scale(singleDiceThrowPanel.gameObject,Vector3.one,0.5f).setEase(LeanTweenType.easeInOutSine)
             .setOnComplete(() => { OnSingleDiceThrowPanelReachedCenterOfScreen(callback); });

        photonView.RPC(nameof(ShowSingleDiceThrowToRemoteClients), RpcTarget.Others, playerName, randomDiceValue);
    }

    private void OnSingleDiceThrowPanelReachedCenterOfScreen(Action callback = null)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(1.2f);
        sequence.append(() =>
        {
            LeanTween.alphaCanvas(CG_Background, 0f, 0.5f);
            LeanTween.scale(singleDiceThrowPanel.gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInOutSine)
             .setOnComplete(()=>
             { 
                 callback?.Invoke();
                 CG_Background.gameObject.SetActive(false);
                 singleDiceThrowPanel.gameObject.SetActive(false);
             });

            //CG_Background.gameObject.SetActive(false);
            //LeanTween.alphaCanvas(CG_Background, 0f, 0.6f);
            //LeanTween.move(singleDiceThrowPanel.gameObject, T_DicePanelSpawn.position, 0.6f).setEase(LeanTweenType.easeInOutSine).setOnComplete(callback);
        });
    }
    private void OnDoubleDiceThrowPanelReachedCenterOfScreen(Action callback = null)
    {
        LTSeq sequence = LeanTween.sequence();
        sequence.append(1.2f);
        sequence.append(() =>
        {
            LeanTween.alphaCanvas(CG_Background, 0f, 0.5f);
            LeanTween.scale(doubleDiceThrowPanel.gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInOutSine)
             .setOnComplete(() =>
             {
                 callback?.Invoke();
                 CG_Background.gameObject.SetActive(false);
                 doubleDiceThrowPanel.gameObject.SetActive(false);
             });
        });
    }
    public void ShowDoubleDiceThrowAttempt(int firstDiceValue, int secondDiceValue, string playerName, Action callback)
    {
        //Spawn.
        CG_Background.gameObject.SetActive(true);
        doubleDiceThrowPanel.gameObject.SetActive(true);

        doubleDiceThrowPanel.UpdateDisplay(playerName, firstDiceValue,secondDiceValue);

        LeanTween.alphaCanvas(CG_Background, 1f, 0.5f);
        LeanTween.scale(doubleDiceThrowPanel.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInOutSine)
             .setOnComplete(() => { OnDoubleDiceThrowPanelReachedCenterOfScreen(callback); });

        photonView.RPC(nameof(ShowDoubleDiceThrowToRemoteClients), RpcTarget.Others, playerName, firstDiceValue,secondDiceValue);
    }

    [PunRPC]
    private void ShowSingleDiceThrowToRemoteClients(string playerName,int randomDiceValue)
    {
        CG_Background.gameObject.SetActive(true);
        singleDiceThrowPanel.gameObject.SetActive(true);

        singleDiceThrowPanel.UpdateDisplay(playerName, randomDiceValue);

        LeanTween.alphaCanvas(CG_Background, 1f, 0.5f);
        LeanTween.scale(singleDiceThrowPanel.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInOutSine)
             .setOnComplete(() => OnSingleDiceThrowPanelReachedCenterOfScreen());
    }
    [PunRPC]
    private void ShowDoubleDiceThrowToRemoteClients(string playerName, int firstDiceValue, int secondDiceValue)
    {
        CG_Background.gameObject.SetActive(true);
        doubleDiceThrowPanel.gameObject.SetActive(true);

        doubleDiceThrowPanel.UpdateDisplay(playerName, firstDiceValue,secondDiceValue);

        LeanTween.alphaCanvas(CG_Background, 1f, 0.5f);
        LeanTween.scale(doubleDiceThrowPanel.gameObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeInOutSine)
             .setOnComplete(() => OnDoubleDiceThrowPanelReachedCenterOfScreen());

    }

    public int GiveRandomDiceValue()
    {
        return UnityEngine.Random.Range(1, 7);
    }
}