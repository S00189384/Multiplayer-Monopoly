using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can mortgage -
// purchased a purchasable tile
// Num owned purchasable tile over 1




public class BTN_MortgageTile : BTN_Base, IOnEventCallback
{
    private bool canMortgageATile;

    public override void Awake()
    {
        base.Awake();

        PhotonNetwork.AddCallbackTarget(this);
        PlayerTurnManager.PlayerFinishedTurnEvent += OnPlayerFinishedTheirTurn;
        GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;
    }

    private void OnAllPlayersSpawned()
    {
        PlayerMortgageTileTracker localPlayerMortgageTileTracker = GameManager.Instance.GetLocalPlayerPiece().GetComponent<PlayerMortgageTileTracker>();
        //localPlayerMortgageTileTracker.PlayerNowOwnsAMortgagedTileEvent += OnLocalPlayerNowOwnsAMortgagedTile;
        //localPlayerMortgageTileTracker.PlayerNoLongerOwnsAMortgagedTileEvent += OnLocalPlayerNoLongerOwnsAMortgagedTile;
    }

    private void OnLocalPlayerNoLongerOwnsAMortgagedTile()
    {
        //canMortgageATile = false;
    }

    private void OnLocalPlayerNowOwnsAMortgagedTile()
    {
        //canMortgageATile = true;

    }

    private void OnPlayerFinishedTheirTurn()
    {
        SetButtonInteractable(false);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCodes.NewTurnEventCode)
        {
            string playerTurn = (string)photonEvent.CustomData;

            if (playerTurn == PhotonNetwork.LocalPlayer.UserId)
                SetButtonInteractable(canMortgageATile);
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        PlayerTurnManager.PlayerFinishedTurnEvent -= OnPlayerFinishedTheirTurn;
        GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawned;
    }
}
