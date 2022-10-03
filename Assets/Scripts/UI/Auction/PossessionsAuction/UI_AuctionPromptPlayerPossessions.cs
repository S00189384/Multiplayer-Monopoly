using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AuctionPromptPlayerPossessions : MonoBehaviour
{
    [SerializeField] private UI_AuctionTurnDisplayManager turnDisplayManager;


    public void InitialisePrompt(string playerIDPossessionsUpForAuction)
    {


        turnDisplayManager.SpawnActivePlayersDisplay(GameManager.Instance.ActivePlayersIDList);
    }
}
