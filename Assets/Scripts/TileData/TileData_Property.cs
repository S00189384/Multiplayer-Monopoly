using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: What if you want to change the number of houses that can be built on a property? C

[CreateAssetMenu(fileName = "PropertyData", menuName = "PropertyData")]
public class TileData_Property : TileData_Purchasable
{
    public int HousePurchaseCost;
    public int HotelPurchaseCost;

    public int HouseSellValue { get { return (int) (HousePurchaseCost * GameDataSlinger.PROPERTY_BUILDING_SELL_VALUE_PERCENTAGE_OF_PURCHASE_COST); } }
    public int HotelSellValue { get { return (int) (HotelPurchaseCost * GameDataSlinger.PROPERTY_BUILDING_SELL_VALUE_PERCENTAGE_OF_PURCHASE_COST); } }

    public int RentDefaultCost;
    public int[] RentCostWithHouses = new int[GameDataSlinger.NUM_MAX_HOUSES_PER_PROPERTY];
    public int RentCostWithHotel;

    public PropertyColourSet PropertyColourSet;
}
