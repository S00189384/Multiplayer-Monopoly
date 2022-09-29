using UnityEngine;

[CreateAssetMenu(fileName = "ReceiveAmountFromBank", menuName = "CardData/ReceiveAmountFromBank")]
public class CardData_ReceiveAmountFromBank : CardData
{
    public int AmountToReceive;

    public override void Execute(string playerID)
    {
        Bank.Instance.AddMoneyToAccount(playerID, AmountToReceive);
    }
}