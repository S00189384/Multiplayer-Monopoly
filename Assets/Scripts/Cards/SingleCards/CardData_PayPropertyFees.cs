using UnityEngine;

//Not used in project yet.

[CreateAssetMenu(fileName = "PayPropertyFees", menuName = "CardData/PayPropertyFees")]
public class CardData_PayPropertyFees : CardData
{
    public int AmountToPayForEachHouse;
    public int AmountToPayForEachHotel;

    public override void Execute(string playerID) { }
}