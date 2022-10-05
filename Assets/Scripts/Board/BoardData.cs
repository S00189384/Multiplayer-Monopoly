using UnityEngine;

//Scriptable object for a board.
//Contains 4 rows of tiles, each row containing tile data as a scriptable object.

[CreateAssetMenu(fileName = "BoardData", menuName = "BoardData")]
public class BoardData : ScriptableObject
{
    public RowData[] rows = new RowData[GameDataSlinger.NUM_ROWS];
}
