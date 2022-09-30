using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_JailManager : MonoBehaviour,IOnEventCallback
{
    [Header("In jail UI options Prefab")]
    [SerializeField] private UI_InJailOptionsPanel jailOptionsPanelPrefab;
    private GameObject GO_spawnedJailPanel;

    [Header("UI Components")]
    [SerializeField] private CanvasGroup CG_Background;
    [SerializeField] private Transform T_panelSpawnTransform;

    [Header("UI Script for showing dice throws while in jail")]
    [SerializeField] private UI_DiceThrowManager UI_diceThrowManager;

    //For accessing players get out of jail free cards if any.
    private PlayerInventory localPlayerInventory;

    //How many turns does the player have left to stay in jail.
    [SerializeField] private int remainingTurnsInJail;
    private void ResetRemainingTurnsInJail() => remainingTurnsInJail = GameDataSlinger.MAX_ROLL_DOUBLE_ATTEMPTS_TO_LEAVE_JAIL;

    private void Awake()
    {
        GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;
        Jailor.LocalPlayerJailedEvent += OnLocalPlayerJailed;
        Jailor.LocalPlayerLeftJailEvent += OnLocalPlayerLeftJail;
    }
    private void Start()
    {
        ResetRemainingTurnsInJail();
    }

    private void OnAllPlayersSpawned()
    {
        localPlayerInventory = GameManager.Instance.GetLocalPlayerPiece().GetComponent<PlayerInventory>();
    }

    public void ShowPanel()
    {
        UI_InJailOptionsPanel spawnedJailPanel = Instantiate(jailOptionsPanelPrefab, T_panelSpawnTransform.position,Quaternion.identity,this.transform);
        spawnedJailPanel.transform.localEulerAngles = Vector3.zero;
        GO_spawnedJailPanel = spawnedJailPanel.gameObject;
        spawnedJailPanel.UpdateDisplay(remainingTurnsInJail, localPlayerInventory.HasAGetOutOfJailFreeCard, OnPayFeeButtonClicked,OnRollDoubleButtonClicked,OnUseGetOutOfJailFreeCard);

        CG_Background.gameObject.SetActive(true);

        LeanTween.alphaCanvas(CG_Background, 1f, 0.5f);
        LeanTween.move(spawnedJailPanel.gameObject, transform.position, 0.5f).setEase(LeanTweenType.easeInOutSine);
    }

    public void HidePanel(Action callbackOnEnd = null)
    {
        LeanTween.alphaCanvas(CG_Background, 0f, 0.5f);
        LeanTween.move(GO_spawnedJailPanel, T_panelSpawnTransform.position, 0.5f).setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                CG_Background.gameObject.SetActive(false);
                Destroy(GO_spawnedJailPanel);
                callbackOnEnd?.Invoke();
            });
    }

    private void OnPayFeeButtonClicked()
    {
        string playerName = GameManager.Instance.GetPlayerNicknameFromID(PhotonNetwork.LocalPlayer.UserId);
        string notificationMessage = $"{playerName} paid ${GameDataSlinger.LEAVE_JAIL_FEE_COST} to leave jail.";

        Jailor.Instance.RemovePlayerFromJail(PhotonNetwork.LocalPlayer.UserId);
        ResetRemainingTurnsInJail();
        Bank.Instance.RemoveMoneyFromLocalPlayerAccount(GameDataSlinger.LEAVE_JAIL_FEE_COST);

        HidePanel(callbackOnEnd: () =>
        UI_NotificationManager.Instance.RPC_ShowNotificationWithLocalCallback(notificationMessage, callback: () =>
        {
            int randomDiceThrow = UI_diceThrowManager.GetRandomDiceValue();
            UI_diceThrowManager.ShowSingleDiceThrow(randomDiceThrow,playerName,() => PieceMover.Instance.MoveLocalPlayerPieceForwardOverTime(randomDiceThrow));         
        })); 
    }

    private void OnRollDoubleButtonClicked()
    {
        string playerName = GameManager.Instance.GetPlayerNicknameFromID(PhotonNetwork.LocalPlayer.UserId);

        int firstDiceValue, secondDiceValue;
        firstDiceValue = UI_diceThrowManager.GetRandomDiceValue();
        secondDiceValue = UI_diceThrowManager.GetRandomDiceValue();

        HidePanel(callbackOnEnd: () => UI_diceThrowManager.ShowDoubleDiceThrowAttempt(firstDiceValue,secondDiceValue, playerName, callback: () => 
        {
            if(firstDiceValue == secondDiceValue)
            {
                Jailor.Instance.RemovePlayerFromJail(PhotonNetwork.LocalPlayer.UserId);
                ResetRemainingTurnsInJail();

                int randomDiceThrow = UI_diceThrowManager.GetRandomDiceValue();
                UI_diceThrowManager.ShowSingleDiceThrow(randomDiceThrow, playerName, () => PieceMover.Instance.MoveLocalPlayerPieceForwardOverTime(randomDiceThrow));
            }
        }));
    }

    private void OnUseGetOutOfJailFreeCard()
    {
        string playerName = GameManager.Instance.GetPlayerNicknameFromID(PhotonNetwork.LocalPlayer.UserId);
        string notificationMessage = $"{playerName} used a get out of jail free card to leave jail";

        localPlayerInventory.UseGetOutOfJailFreeCard();
        Jailor.Instance.RemovePlayerFromJail(PhotonNetwork.LocalPlayer.UserId);
        ResetRemainingTurnsInJail();

        HidePanel(callbackOnEnd: () =>
        UI_NotificationManager.Instance.RPC_ShowNotificationWithLocalCallback(notificationMessage, callback: () =>
        {
            int randomDiceThrow = UI_diceThrowManager.GetRandomDiceValue();
            UI_diceThrowManager.ShowSingleDiceThrow(randomDiceThrow, playerName,callback: () => PieceMover.Instance.MoveLocalPlayerPieceForwardOverTime(randomDiceThrow));
        }));
    }

    //When this clients player is jailed, listen for their turn and process them being in jail.
    private void OnLocalPlayerJailed() => PhotonNetwork.AddCallbackTarget(this);
    private void OnLocalPlayerLeftJail() => PhotonNetwork.RemoveCallbackTarget(this);
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCodes.NewTurnEventCode)
        {
            string playerTurn = (string)photonEvent.CustomData;

            if (playerTurn == PhotonNetwork.LocalPlayer.UserId && Jailor.Instance.LocalPlayerIsInJail)
            {          
                if(remainingTurnsInJail <= 0) //Player stayed in jail too long.
                {
                    string playerName = GameManager.Instance.GetPlayerNicknameFromID(PhotonNetwork.LocalPlayer.UserId);
                    string notificationMessage = $"{playerName} stayed in jail too long and had to pay ${GameDataSlinger.LEAVE_JAIL_FEE_COST} to leave jail.";

                    Jailor.Instance.RemovePlayerFromJail(PhotonNetwork.LocalPlayer.UserId);
                    ResetRemainingTurnsInJail();
                    Bank.Instance.RemoveMoneyFromLocalPlayerAccount(GameDataSlinger.LEAVE_JAIL_FEE_COST);

                    UI_NotificationManager.Instance.RPC_ShowNotificationWithLocalCallback(notificationMessage, callback: () =>
                    {
                        int randomDiceThrow = UI_diceThrowManager.GetRandomDiceValue();
                        UI_diceThrowManager.ShowSingleDiceThrow(randomDiceThrow, playerName,callback: () => PieceMover.Instance.MoveLocalPlayerPieceForwardOverTime(randomDiceThrow));
                    });
                }
                else
                {
                    ShowPanel();
                    remainingTurnsInJail--;
                }
            }
        }
    }
    private void OnDestroy()
    {
        GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawned;
        Jailor.LocalPlayerJailedEvent -= OnLocalPlayerJailed;
        Jailor.LocalPlayerLeftJailEvent -= OnLocalPlayerLeftJail; 
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}