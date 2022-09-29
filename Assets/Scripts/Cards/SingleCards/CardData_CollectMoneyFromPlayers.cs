using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectMoneyFromPlayers", menuName = "CardData/CollectMoneyFromPlayers")]
public class CardData_CollectMoneyFromPlayers : CardData
{
    public int AmountToReceiveFromEachPlayer;

    public override void Execute(string playerID)
    {
        //Get list of player ID's that have to pay the player.
        List<string> playerIDsToReceiveMoneyFrom = GameManager.Instance.GetActivePlayersIgnoringID(playerID);

        //Make payment between players.
        for (int i = 0; i < playerIDsToReceiveMoneyFrom.Count; i++)
        {
            Bank.Instance.MakePlayerPaymentExchangeRPC(playerIDsToReceiveMoneyFrom[i], playerID, AmountToReceiveFromEachPlayer,RpcTarget.All);
        }
    }
}