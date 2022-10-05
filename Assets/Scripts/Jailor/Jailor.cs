using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jailor : MonoBehaviourPun
{
    public static Jailor Instance;

    private TileInstance_Jail jailTileInstance;

    private bool localPlayerIsInJail;
    public bool LocalPlayerIsInJail 
    {
        get => localPlayerIsInJail;
        set 
        {
            localPlayerIsInJail = value;
            if (localPlayerIsInJail)
                LocalPlayerJailedEvent?.Invoke();
            else
                LocalPlayerLeftJailEvent?.Invoke();
        } 
    }

    [SerializeField] private List<string> jailedPlayersList = new List<string>();
    public int NumJailedPlayers { get { return jailedPlayersList.Count; } }

    public static event Action<string> PlayerJailedEvent;
    public static event Action LocalPlayerJailedEvent;
    public static event Action LocalPlayerLeftJailEvent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        if (jailTileInstance == null)
            jailTileInstance = FindObjectOfType<TileInstance_Jail>();
    }

    public void JailPlayer(string playerID)
    {
        if (playerID == PhotonNetwork.LocalPlayer.UserId)
            LocalPlayerIsInJail = true;

        jailedPlayersList.Add(playerID);

        Player_Piece playersPiece = GameManager.Instance.GetPlayerPieceByID(playerID);
        PieceMover.Instance.MovePieceToTileIndexInstant(playersPiece, jailTileInstance.TileBoardIndex,raiseEvent: false);
        UI_NotificationManager.Instance.RPC_ShowNotificationWithLocalCallback($"{GameManager.Instance.GetPlayerNicknameFromID(playerID)} has been sent to jail!",PlayerTurnManager.Instance.EndPlayerTurn);

        PlayerJailedEvent?.Invoke(playerID);

        photonView.RPC(nameof(JailPlayerRemote), RpcTarget.Others, playerID);
    }

    public void RemovePlayerFromJail(string playerID)
    {
        if (playerID == PhotonNetwork.LocalPlayer.UserId)
            LocalPlayerIsInJail = false;

        jailedPlayersList.Remove(playerID);

        photonView.RPC(nameof(RemovePlayerFromJailRemote), RpcTarget.Others, playerID);
    }

    [PunRPC]
    public void JailPlayerRemote(string playerID)
    {
        jailedPlayersList.Add(playerID);
    }

    [PunRPC]
    public void RemovePlayerFromJailRemote(string playerID)
    {
        jailedPlayersList.Remove(playerID);
    }
}