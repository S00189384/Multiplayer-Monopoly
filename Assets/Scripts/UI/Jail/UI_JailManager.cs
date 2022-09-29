using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_JailManager : MonoBehaviour,IOnEventCallback
{
    [SerializeField] private UI_DiceThrowManager UI_diceThrowManager;
    private PlayerInventory localPlayerInventory;

    [SerializeField] private UI_InJailOptionsPanel jailOptionsPanelPrefab;
    [SerializeField] private CanvasGroup CG_Background;

    [SerializeField] private Transform T_panelSpawnTransform;

    private int remainingTurnsInJail = GameDataSlinger.MAX_ROLL_DOUBLE_ATTEMPTS_TO_LEAVE_JAIL;

    private GameObject GO_spawnedJailPanel;

    private void Awake()
    {
        GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;
        PhotonNetwork.AddCallbackTarget(this);
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

        Player_Piece localPlayerPiece = GameManager.Instance.GetLocalPlayerPiece();

        HidePanel(callbackOnEnd: () =>
        UI_NotificationManager.Instance.RPC_ShowNotificationWithLocalCallback(notificationMessage, callback: () =>
        {
            int randomDiceThrow = UI_diceThrowManager.GiveRandomDiceValue();
            UI_diceThrowManager.ShowSingleDiceThrow(randomDiceThrow,playerName,() => PieceMover.Instance.MovePieceForwardOverTime(localPlayerPiece,randomDiceThrow));         
        })); 
    }

    private void OnRollDoubleButtonClicked()
    {
        string playerName = GameManager.Instance.GetPlayerNicknameFromID(PhotonNetwork.LocalPlayer.UserId);

        int firstDiceValue, secondDiceValue;
        firstDiceValue = UI_diceThrowManager.GiveRandomDiceValue();
        secondDiceValue = UI_diceThrowManager.GiveRandomDiceValue();

        HidePanel(callbackOnEnd: () => UI_diceThrowManager.ShowDoubleDiceThrowAttempt(firstDiceValue,secondDiceValue, playerName, callback: () => 
        {
            if(firstDiceValue == secondDiceValue)
            {
                Jailor.Instance.RemovePlayerFromJail(PhotonNetwork.LocalPlayer.UserId);

                Player_Piece localPlayerPiece = GameManager.Instance.GetLocalPlayerPiece();
                int randomDiceThrow = UI_diceThrowManager.GiveRandomDiceValue();
                UI_diceThrowManager.ShowSingleDiceThrow(randomDiceThrow, playerName, () => PieceMover.Instance.MovePieceForwardOverTime(localPlayerPiece, randomDiceThrow));
            }
            else
            {
                
            }

        }));
    }

    private void OnUseGetOutOfJailFreeCard()
    {
        string playerName = GameManager.Instance.GetPlayerNicknameFromID(PhotonNetwork.LocalPlayer.UserId);
        string notificationMessage = $"{playerName} used a get out of jail free card to leave jail";

        localPlayerInventory.UseGetOutOfJailFreeCard();
        Jailor.Instance.RemovePlayerFromJail(PhotonNetwork.LocalPlayer.UserId);

        Player_Piece localPlayerPiece = GameManager.Instance.GetLocalPlayerPiece();

        HidePanel(callbackOnEnd: () =>
        UI_NotificationManager.Instance.RPC_ShowNotificationWithLocalCallback(notificationMessage, callback: () =>
        {
            int randomDiceThrow = UI_diceThrowManager.GiveRandomDiceValue();
            UI_diceThrowManager.ShowSingleDiceThrow(randomDiceThrow, playerName, () => PieceMover.Instance.MovePieceForwardOverTime(localPlayerPiece, randomDiceThrow));
        }));
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCodes.NewTurnEventCode)
        {
            string playerTurn = (string)photonEvent.CustomData;

            if (playerTurn == PhotonNetwork.LocalPlayer.UserId && Jailor.Instance.LocalPlayerIsInJail)
            {          
                if(remainingTurnsInJail <= 0)
                {
                    string playerName = GameManager.Instance.GetPlayerNicknameFromID(PhotonNetwork.LocalPlayer.UserId);
                    string notificationMessage = $"{playerName} stayed in jail too long and had to pay ${GameDataSlinger.LEAVE_JAIL_FEE_COST} to leave jail.";

                    Jailor.Instance.RemovePlayerFromJail(PhotonNetwork.LocalPlayer.UserId);

                    Player_Piece localPlayerPiece = GameManager.Instance.GetLocalPlayerPiece();

                    UI_NotificationManager.Instance.RPC_ShowNotificationWithLocalCallback(notificationMessage, callback: () =>
                    {
                        int randomDiceThrow = UI_diceThrowManager.GiveRandomDiceValue();
                        UI_diceThrowManager.ShowSingleDiceThrow(randomDiceThrow, playerName, () => PieceMover.Instance.MovePieceForwardOverTime(localPlayerPiece, randomDiceThrow));
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
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
