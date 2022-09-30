using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTN_UnmortgageTile : BTN_Base/*, IOnEventCallback*/
{
    //private bool canUnmortgageATile;


    //public override void Awake()
    //{
    //    base.Awake();

    //    PhotonNetwork.AddCallbackTarget(this);
    //    PlayerTurnManager.PlayerFinishedTurnEvent += OnPlayerFinishedTheirTurn;
    //    GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;
    //}
    //private void OnAllPlayersSpawned()
    //{
    //    PlayerMortgageTileTracker localPlayerMortgageTileTracker = GameManager.Instance.GetLocalPlayerPiece().GetComponent<PlayerMortgageTileTracker>();
    //    localPlayerMortgageTileTracker.PlayerNowOwnsAMortgagedTileEvent += OnLocalPlayerNowOwnsAMortgagedTile;
    //    localPlayerMortgageTileTracker.PlayerNoLongerOwnsAMortgagedTileEvent += OnLocalPlayerNoLongerOwnsAMortgagedTile;
    //}

    ////Reacting to player turn.
    //private void OnPlayerFinishedTheirTurn() => SetButtonInteractable(false);
    //public void OnEvent(EventData photonEvent)
    //{
    //    if (photonEvent.Code == EventCodes.NewTurnEventCode)
    //    {
    //        string playerTurn = (string)photonEvent.CustomData;

    //        if (playerTurn == PhotonNetwork.LocalPlayer.UserId)
    //            SetButtonInteractable(canUnmortgageATile);
    //    }
    //}

    ////Reacting to player gaining / removing mortgaged tiles.
    //private void OnLocalPlayerNowOwnsAMortgagedTile() => canUnmortgageATile = true;
    //private void OnLocalPlayerNoLongerOwnsAMortgagedTile() => canUnmortgageATile = false;

    //private void OnDestroy()
    //{
    //    PhotonNetwork.RemoveCallbackTarget(this);
    //    PlayerTurnManager.PlayerFinishedTurnEvent -= OnPlayerFinishedTheirTurn;
    //    GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawned;
    //}
}
