using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

//Class for handling the player turns in the game. 
//When one player is left, event is raised which signals gamemanager to trigger game over sequence.

public class PlayerTurnManager : MonoBehaviourPunCallbacks
{
    public static PlayerTurnManager Instance;

    private TurnManager<string> playerTurnManager;

    public static event Action PlayerFinishedTurnEvent;
    public string GetCurrentTurn { get { return playerTurnManager.CurrentTurn; } }

    public static event Action<string, List<string>> OneRemainingPlayerEvent; //Last player, list of removed players.

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;
        Bank.PlayerDeclaredBankruptDueToBankPaymentEvent += OnPlayerDeclaredBankrupcy;
        Bank.PlayerDeclaredBankruptDueToPlayerPaymentEvent += OnPlayerBankruptDueToPlayer;
    }


    private void OnOnePlayerRemaining(string lastPlayerLeft)
    {
        OneRemainingPlayerEvent?.Invoke(lastPlayerLeft, playerTurnManager.RemovedTurns);
    }

    private void OnPlayerBankruptDueToPlayer(string playerIDBankrupt, string playerIDThatCausedBankrupcy)
    {
        OnPlayerDeclaredBankrupt(playerIDBankrupt);
    }
    private void OnPlayerDeclaredBankrupcy(string playerID) 
    {
        OnPlayerDeclaredBankrupt(playerID);
    }
    private void OnPlayerDeclaredBankrupt(string playerID)
    {
        if (playerTurnManager.CurrentTurn == playerID)
            EndPlayerTurn();

        playerTurnManager.RemoveTurn(playerID);
        photonView.RPC(nameof(RemoveTurnRemoteClients), RpcTarget.Others, playerID);
    }

    private void TriggerNewTurnEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(EventCodes.NewTurnEventCode, playerTurnManager.CurrentTurn, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnAllPlayersSpawned()
    {
        playerTurnManager = new TurnManager<string>();
        playerTurnManager.Initialise(GameManager.Instance.ActivePlayersIDList);
        playerTurnManager.OneTurnRemainingEvent += OnOnePlayerRemaining;

        TriggerNewTurnEvent();
    }

    [PunRPC]
    private void MoveToNextTurnLocalClient()
    {
        playerTurnManager.MoveToNextTurn();
    }

    public void EndPlayerTurn()
    {
        photonView.RPC(nameof(MoveToNextTurnLocalClient), RpcTarget.All);

        PlayerFinishedTurnEvent?.Invoke();

        TriggerNewTurnEvent();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playerTurnManager.CurrentTurn == otherPlayer.UserId)
        {
            MoveToNextTurnLocalClient();
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
                TriggerNewTurnEvent();
        }

        if (playerTurnManager.ContainsTurn(otherPlayer.UserId))
        {
            playerTurnManager.RemoveTurn(otherPlayer.UserId);
        }
    }

    [PunRPC] 
    private void RemoveTurnRemoteClients(string playerID)
    {
        playerTurnManager.RemoveTurn(playerID);
    }

    private void OnDestroy()
    {
        GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawned;
        Bank.PlayerDeclaredBankruptDueToBankPaymentEvent -= OnPlayerDeclaredBankrupcy;
    }
}