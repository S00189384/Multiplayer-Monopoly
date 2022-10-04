using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PayAmountToEachPlayer", menuName = "CardData/PayAmountToEachPlayer")]
public class CardData_PayAmountToEachPlayer : CardData
{
    public int AmountToPayEachPlayer;

    public override void Execute(string playerID)
    {
        //Get list of player ID's that have to pay the player.
        List<string> playerIDsToPayMoneyTo = GameManager.Instance.GetActivePlayersIgnoringID(playerID);

        //Make payment between players.
        for (int i = 0; i < playerIDsToPayMoneyTo.Count; i++)
        {
            Bank.Instance.MakePlayerPaymentExchange(playerID, playerIDsToPayMoneyTo[i], AmountToPayEachPlayer);
        }
    }
}