using UnityEngine;

//Scriptable object for a row.
//Each row contains tile data.

[CreateAssetMenu(fileName = "RowData", menuName = "RowData")]
public class RowData : ScriptableObject
{
    public TileData[] tileDataList = new TileData[GameDataSlinger.NUM_ROWS];
}
