using System;

//Tile instance for a station tile.
//Rent is calculated by multiplying the default rent cost by a multiplier using the number of stations that the owner owns on the board.

public class TileInstance_Station : TileInstance_Purchasable, iTileDataRecievable, iPlayerProcessable
{
    //Data for this tile.
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

    public override void MortgageTile() => base.MortgageTile();
    public override void UnmortgageTile() => base.UnmortgageTile();

    public void RecieveTileData(TileData tileData)
    {
        tileDataStation = (TileData_Station)tileData;
    }
}