using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AuctionPromptSingleProperty : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI TMP_Header;
    [SerializeField] private UI_PropertyInformation propertyInformation;
    [SerializeField] private UI_AuctionTurnDisplayManager turnDisplayManager;

    public void InitialisePrompt(string playerIDThatStartedAuction,TileInstance_Property propertyInstance)
    {
        TMP_Header.text = $"{GameManager.Instance.GetPlayerNicknameFromID(playerIDThatStartedAuction)} started an auction!";
        propertyInformation.UpdateDisplay(propertyInstance.propertyData);
        turnDisplayManager.SpawnActivePlayersDisplay(GameManager.Instance.ActivePlayersIDList);

        //AuctionTurnManager.Instance.Initialise();
    }
}