using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

//Build on property button. Enabled when its the players turn.
//Enables UI for constructing a building on a property when clicked.

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