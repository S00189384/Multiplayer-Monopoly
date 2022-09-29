using UnityEngine;

[CreateAssetMenu(fileName = "AdvanceToTile", menuName = "CardData/AdvanceToTile")]
public class CardData_AdvanceToTile : CardData
{
    //Tile data instead? Then search for board tiles containing this data to find the index of the tile??
    //public TileData tileDataToMoveTo;
    public int TileIndexToMoveTo;

    public override void Execute(string playerID)
    {
        //Change this to auto direction?

        PieceMover.Instance.MovePieceForwardOverTime(GameManager.Instance.GetPlayerPieceByID(playerID), TileIndexToMoveTo);
        //PieceMover.Instance.MovePieceToTileIndexInstant(GameManager.Instance.GetPlayerPieceByID(playerID), TileIndexToMoveTo);
    }
}

