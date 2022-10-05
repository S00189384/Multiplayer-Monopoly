using UnityEngine;

[CreateAssetMenu(fileName = "AdvanceToTile", menuName = "CardData/AdvanceToTile")]
public class CardData_AdvanceToTile : CardData
{
    //Maybe use tile data instead? Then search for board tiles containing this data to find the index of the tile?
    public int TileIndexToMoveTo;

    public override void Execute(string playerID)
    {
        PieceMover.Instance.MovePieceForwardOverTime(GameManager.Instance.GetPlayerPieceByID(playerID), TileIndexToMoveTo);
    }
}

