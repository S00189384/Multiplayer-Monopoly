using UnityEngine;

[CreateAssetMenu(fileName = "MoveInDirection", menuName = "CardData/MoveInADirectionByNumberOfTiles")]
public class CardData_MoveInADirectionByNumberOfTiles : CardData
{
    public int NumTilesToMove;
    public MoveDirectionType moveDirection;

    public override void Execute(string playerID)
    {
        PieceMover.Instance.MovePieceInDirectionOverTime(GameManager.Instance.GetPlayerPieceByID(playerID), NumTilesToMove,moveDirection);
    }
}
