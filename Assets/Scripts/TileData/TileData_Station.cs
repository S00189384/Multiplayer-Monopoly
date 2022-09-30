using UnityEngine;

[CreateAssetMenu(fileName = "TileData_Station", menuName = "TileData_Station")]
public class TileData_Station : TileData_Purchasable
{
    public int RentDefaultCost;
    public Sprite SPR_Station;

    //Get cost of rent (based of multiplier).
    //If you want rent cost of individual stations to not use the same multiplier system to calculate rent then change this and add int array.
    public int GetRentCost(int numStationsOwned)
    {
        int rentCost = RentDefaultCost;
        for (int i = 1; i < numStationsOwned; i++)
        {
            rentCost = rentCost * GameDataSlinger.STATION_OWNED_RENT_MULTIPLIER;
        }
        return rentCost;
    }
}