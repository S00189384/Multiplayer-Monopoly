using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Button for selling houses / hotels on a property.
//Enables / Disables based on player turn and whether they have property buildings constructed.

public class BTN_Sell : BTN_Base,IOnEventCallback
{
    [SerializeField] private UI_PropertySellBuildingSelection propertySellBuildingSelection;

    public override void Awake()
    {
        base.Awake();

        PlayerTurnManager.PlayerFinishedTurnEvent += OnPlayerFinishedTheirTurn;
        PhotonNetwork.AddCallbackTarget(this);
        AddOnClickListener(OnMyButtonClick);
        //GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;
    }


    private void OnMyButtonClick()
    {
        propertySellBuildingSelection.Initialise();
    }

    private void OnAllPlayersSpawned()
    {
        //PlayerPropertyBuildingTracker localPlayersPropertyBuildingTracker = GameManager.Instance.GetLocalPlayerPiece().GetComponent<PlayerPropertyBuildingTracker>();
        //localPlayersPropertyBuildingTracker.PlayerGainedAbilityToSellPropertyBuildings += OnLocalPlayerGainedAbilityToSellPropertyBuildings;
        //localPlayersPropertyBuildingTracker.PlayerLostAbilityToSellPropertyBuildings += OnLocalPlayerLostAbilityToSellPropertyBuildings;
    }

    //Turn changing button reaction.
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
                SetButtonInteractable(true);
        }
    }

    //Player building / losing property building/s button reaction.
    private void OnLocalPlayerGainedAbilityToSellPropertyBuildings()
    {
        //CanSell = true;
        SetButtonInteractable(true);
    }
    private void OnLocalPlayerLostAbilityToSellPropertyBuildings()
    {
        //CanSell = false;
        SetButtonInteractable(false);
    }


    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        PlayerTurnManager.PlayerFinishedTurnEvent -= OnPlayerFinishedTheirTurn;
    }
}