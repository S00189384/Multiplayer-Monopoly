using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

//TODO: 

public class BTN_BuildOnProperty : BTN_Base,IOnEventCallback
{
    [SerializeField] private UI_PropertyConstructBuildingSelection propertyBuildingSelection;

    public override void Awake()
    {
        base.Awake();
        AddOnClickListener(OnMyButtonClick);

        PhotonNetwork.AddCallbackTarget(this);
        PlayerTurnManager.PlayerFinishedTurnEvent += OnPlayerFinishedTheirTurn;
    }

    private void OnMyButtonClick()
    {

        propertyBuildingSelection.Initialise();
    }

    private void OnPlayerFinishedTheirTurn()
    {
        SetButtonInteractable(false);
    }

    private void Start()
    {
        SetButtonInteractable(false);
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == EventCodes.NewTurnEventCode)
        {
            string playerTurn = (string)photonEvent.CustomData;

            if(playerTurn == PhotonNetwork.LocalPlayer.UserId)
                SetButtonInteractable(true);
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        PlayerTurnManager.PlayerFinishedTurnEvent -= OnPlayerFinishedTheirTurn;
    }
}