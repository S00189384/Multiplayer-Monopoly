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

    private bool CanBuildOnProperty;

    public override void Awake()
    {
        base.Awake();

        PhotonNetwork.AddCallbackTarget(this);

        PlayerTurnManager.PlayerFinishedTurnEvent += OnPlayerFinishedTheirTurn;
        //GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;
        AddOnClickListener(OnMyButtonClick);
        //Player built hotel event.
    }

    private void OnMyButtonClick()
    {

        propertyBuildingSelection.Initialise();
    }

    //private void OnAllPlayersSpawned()
    //{
    //    OwnedPlayerTileTracker localPlayerOwnedTileTracker = GameManager.Instance.SpawnedPlayersDictionary[PhotonNetwork.LocalPlayer.UserId].GetComponent<OwnedPlayerTileTracker>();
    //    localPlayerOwnedTileTracker.OwnsAllOfPropertyTypeEvent += OnLocalPlayerNowOwnsAllOfAPropertyType;
    //    localPlayerOwnedTileTracker.NoLongerOwnsAllOfAnyPropertyTypeEvent += OnPlayerNoLongerOwnsAllOfAnyPropertyType;
    //}

    private void OnPlayerNoLongerOwnsAllOfAnyPropertyType() => CanBuildOnProperty = false;
    private void OnLocalPlayerNowOwnsAllOfAPropertyType() => CanBuildOnProperty = true;

    private void OnPlayerFinishedTheirTurn()
    {
        SetButtonInteractable(false);
    }

    private void Start()
    {
        CanBuildOnProperty = false;
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
       // GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawned;
    }
}