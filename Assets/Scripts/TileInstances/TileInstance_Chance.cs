using System;
using UnityEngine;

//Tile instance for a chance card.
//Triggers event when player lands on a chance card tile which UI_CardManager listens for. 

public class TileInstance_Chance : TileInstance, iTileDataRecievable, iPlayerProcessable
{
    [SerializeField] private TileData_Chance tileDataChance;

    public static event Action<string> PlayerLandedOnChanceCardEvent;

    public void ProcessPlayer(string playerID)
    {
        PlayerLandedOnChanceCardEvent?.Invoke(playerID);
    }

    public void RecieveTileData(TileData tileData)
    {
        tileDataChance = (TileData_Chance)tileData;
    }
}
