using System;

//Utility tile instance that is active / spawned on the board.

public class TileInstance_Utility : TileInstance_Purchasable, iTileDataRecievable, iPlayerProcessable
{
    //Data for this tile.
    public TileData_Utility tileDataUtility;
    public override TileData_Purchasable GetPurchasableData => tileDataUtility;

    //Events.
    public static event Action<string, TileInstance_Utility> PlayerLandedOnUnownedUtilityEvent;

    //Utility rent is the number of owned utilities of the owner multiplied by the dice roll value.
    public int CurrentRentCost 
    { 
        get 
        {
            int numOwnedUtilitiesOfOwner = TileOwnershipManager.Instance.GetOwnedPlayerTileTracker(OwnerID).OwnedUtilitiesCount;
            return BTN_RollDice.DiceRollValue * GameDataSlinger.UTILITY_RENT_DICE_MULTIPLIERS[numOwnedUtilitiesOfOwner - 1];
        } 
    }

    //Start.
    private void Start() => PurchaseCost = tileDataUtility.PurchaseCost;

    public override void MortgageTile() => base.MortgageTile();
    public override void UnmortgageTile() => base.UnmortgageTile();

    public void ProcessPlayer(string playerID)
    {
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