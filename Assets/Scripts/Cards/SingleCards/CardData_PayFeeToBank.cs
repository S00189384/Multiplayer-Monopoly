using UnityEngine;

[CreateAssetMenu(fileName = "PayFeeToBank", menuName = "CardData/PayFeeToBank")]
public class CardData_PayFeeToBank : CardData
{
    public int FeeToPayBank;

    public override void Execute(string playerID)
    {
        Bank.Instance.RemoveMoneyFromAccount(playerID, FeeToPayBank);
    }
}