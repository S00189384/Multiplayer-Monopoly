using System;
using UnityEngine;

//Tile instance for a CommunityChest card.
//Triggers event when player lands on a CommunityChest card tile which UI_CardManager listens for. 

public class TileInstance_CommunityChest : TileInstance, iTileDataRecievable, iPlayerProcessable
{
    [SerializeField] private TileData_CommunityChest tileDataCommunityChest;

    public static event Action<string> PlayerLandedOnCommunityChestEvent;

    public void ProcessPlayer(string playerID)
    {
        PlayerLandedOnCommunityChestEvent?.Invoke(playerID);
    }

    public void RecieveTileData(TileData tileData)
    {
        tileDataCommunityChest = (TileData_CommunityChest)tileData;
    }
}