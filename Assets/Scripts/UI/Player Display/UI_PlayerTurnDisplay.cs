using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

//Small text above player displays on right of screen which show the player whos turn it is.

public class UI_PlayerTurnDisplay : MonoBehaviour,IOnEventCallback
{
    [SerializeField] private TextMeshProUGUI TMP_PlayerTurn;

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EventCodes.NewTurnEventCode)
        {
            string playerTurn = (string)photonEvent.CustomData;
            if (PhotonNetwork.LocalPlayer.UserId == playerTurn)
                TMP_PlayerTurn.text = $"Your turn";
            else
                TMP_PlayerTurn.text = $"{GameManager.Instance.GetPlayerNicknameFromID(playerTurn)}'s turn";
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
