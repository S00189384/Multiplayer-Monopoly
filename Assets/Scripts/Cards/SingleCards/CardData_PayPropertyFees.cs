using UnityEngine;

[CreateAssetMenu(fileName = "PayPropertyFees", menuName = "CardData/PayPropertyFees")]
public class CardData_PayPropertyFees : CardData
{
    public int AmountToPayForEachHouse;
    public int AmountToPayForEachHotel;

    public override void Execute(string playerID)
    {
        //OwnedPlayerTileTracker ownedPlayerTileTracker = TileOwnershipManager.Instance.GetOwnedPlayerTileTracker(playerID);
        //ownedPlayerTileTracker.count
    }
}