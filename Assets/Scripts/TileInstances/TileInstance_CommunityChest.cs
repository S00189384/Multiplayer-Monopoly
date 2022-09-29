using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInstance_CommunityChest : TileInstance, iTileDataRecievable, iPlayerProcessable
{
    [SerializeField] private TileData_CommunityChest tileDataCommunityChest;

    public void ProcessPlayer(string playerID)
    {
        UI_CardManager.Instance.ShowCommunityChestCard(playerID);
    }

    public void RecieveTileData(TileData tileData)
    {
        tileDataCommunityChest = (TileData_CommunityChest)tileData;
    }
}
