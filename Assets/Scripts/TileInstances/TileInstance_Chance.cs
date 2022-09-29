using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInstance_Chance : TileInstance, iTileDataRecievable, iPlayerProcessable
{
    [SerializeField] private TileData_Chance tileDataChance;

    public void ProcessPlayer(string playerID)
    {
        UI_CardManager.Instance.ShowChanceCard(playerID);
    }

    public void RecieveTileData(TileData tileData)
    {
        tileDataChance = (TileData_Chance)tileData;
    }
}
