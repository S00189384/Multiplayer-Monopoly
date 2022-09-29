using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInstance_Station : TileInstance_Purchasable, iTileDataRecievable, iPlayerProcessable
{
    public TileData_Station tileDataStation;
    public override TileData_Purchasable GetPurchasableData => tileDataStation;

    public int CurrentRentCost 
    { 
        get
        {
            int numOwnedStationsOfOwner = TileOwnershipManager.Instance.GetOwnedPlayerTileTracker(OwnerID).OwnedStationsCount;
            return tileDataStation.GetRentCost(numOwnedStationsOfOwner);           
        } 
    }

    public static event Action<string, TileInstance_Station> PlayerLandedOnUnownedStationEvent;


    private void Start()
    {
        PurchaseCost = tileDataStation.PurchaseCost;
    }

    public void ProcessPlayer(string playerID)
    {
        print("Station processed player");

        if (!IsOwned)
        {
            PlayerLandedOnUnownedStationEvent?.Invoke(playerID, this);
        }
        else
        {
            if (!PlayerLandedIsOwner(playerID))
            {
                //Pay rent.
                Bank.Instance.ProcessRentPayment(playerID, OwnerID, CurrentRentCost);
            }
        }
    }

    public override void MortgageTile()
    {
        base.MortgageTile();
    }

    public override void UnmortgageTile()
    {
        base.UnmortgageTile();
    }

    public void RecieveTileData(TileData tileData)
    {
        tileDataStation = (TileData_Station)tileData;
    }
}
