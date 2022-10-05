using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//Button to enable the selection of mortgaging a tile.

public class BTN_TileMortgagerSelection : BTN_Base, IOnEventCallback
{
    [SerializeField] private UI_TileMortgageSelection uiTileMortgagerSelection;

    public override void Awake()
    {
        base.Awake();
        PhotonNetwork.AddCallbackTarget(this);
        PlayerTurnManager.PlayerFinishedTurnEvent += OnPlayerFinishedTheirTurn;
        AddOnClickListener(OnMyButtonClick);
    }

    private void Start()
    {
        SetButtonInteractable(false);
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
                SetButtonInteractable(true);
        }
    }

    private void OnMyButtonClick()
    {
        uiTileMortgagerSelection.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        PlayerTurnManager.PlayerFinishedTurnEvent -= OnPlayerFinishedTheirTurn;
    }
}