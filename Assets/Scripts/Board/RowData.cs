using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RowData", menuName = "RowData")]
public class RowData : ScriptableObject
{
    public TileData[] tileDataList = new TileData[GameDataSlinger.NUM_ROWS];
}
