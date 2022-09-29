using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardData", menuName = "BoardData")]
public class BoardData : ScriptableObject
{
    public RowData[] rows = new RowData[GameDataSlinger.NUM_ROWS];
}
