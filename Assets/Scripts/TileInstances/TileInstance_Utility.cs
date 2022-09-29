using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInstance_Utility : TileInstance_Purchasable, iTileDataRecievable, iPlayerProcessable
{
    public TileData_Utility tileDataUtility;
    public override TileData_Purchasable GetPurchasableData => tileDataUtility;


    public int CurrentRentCost 
    { 
        get 
        {
            int numOwnedUtilitiesOfOwner = TileOwnershipManager.Instance.GetOwnedPlayerTileTracker(OwnerID).OwnedUtilitiesCount;
            return BTN_RollDice.DiceRollValue * GameDataSlinger.UTILITY_RENT_DICE_MULTIPLIERS[numOwnedUtilitiesOfOwner];
        } 
    }


    public static event Action<string, TileInstance_Utility> PlayerLandedOnUnownedUtilityEvent;

    private void Start()
    {
        PurchaseCost = tileDataUtility.PurchaseCost;
    }

    public override void MortgageTile()
    {
        base.MortgageTile();
    }

    public override void UnmortgageTile()
    {
        base.UnmortgageTile();
    }

    public void ProcessPlayer(string playerID)
    {
        print("Utility processed player ");
        if (!IsOwned)
        {
            PlayerLandedOnUnownedUtilityEvent?.Invoke(playerID, this);
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

    public void RecieveTileData(TileData tileData)
    {
        tileDataUtility = (TileData_Utility)tileData;
    }
}