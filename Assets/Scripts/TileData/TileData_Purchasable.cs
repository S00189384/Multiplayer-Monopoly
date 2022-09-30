using UnityEngine;

[CreateAssetMenu(fileName = "TileData_Purchasable", menuName = "TileData_Purchasable")]
public abstract class TileData_Purchasable : TileData
{
    public int PurchaseCost;
    public int MortgageValue;
    public int UnmortgageCost { get { return MortgageValue + (int)(MortgageValue * GameDataSlinger.UNMORTGAGE_INTEREST_COST); } }
}
