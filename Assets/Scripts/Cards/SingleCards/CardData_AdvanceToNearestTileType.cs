using UnityEngine;

//Not used in project yet.

[CreateAssetMenu(fileName = "AdvanceToNearestTileType", menuName = "CardData/AdvanceToNearestTileType")]
public class CardData_AdvanceToNearestTileType : CardData
{
    public TileType tileTypeToMoveTo;
    public int diceMultiplierPayToOwner; //If owned, throw dice and pay owner a total ten times amount thrown.

    public override void Execute(string playerID)
    {
        
    }
}