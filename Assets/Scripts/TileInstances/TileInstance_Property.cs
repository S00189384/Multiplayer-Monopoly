using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInstance_Property : TileInstance_Purchasable, iTileDataRecievable,iPlayerProcessable
{
    [Header("Property Info")]
    public TileData_Property propertyData;
    public override TileData_Purchasable GetPurchasableData => propertyData;

    private PropertyTileBuildingConstructor propertyBuildingConstructor;

    [SerializeField] public int CurrentRentRequired;

    public static event Action<string,TileInstance_Property> PlayerLandedOnUnownedPropertyEvent;
    public event Action<TileInstance_Property> BuiltHouseEvent;
    public event Action<TileInstance_Property> BuiltHotelEvent;
    public event Action<TileInstance_Property> SoldHouseEvent;
    public event Action<TileInstance_Property> SoldHotelEvent;

    public override bool CanBeMortgagedd => base.CanBeMortgagedd && propertyBuildingConstructor.PropertyContainsAnyBuildings;
    public int NumConstructedBuildings { get { return propertyBuildingConstructor.NumConstructedBuildings;} }
    public bool CanBuildHouse { get { return propertyBuildingConstructor.NumHousesBuilt < GameDataSlinger.NUM_MAX_HOUSES_PER_PROPERTY; } }
    public bool CanBuildHotel { get { return propertyBuildingConstructor.NumHousesBuilt == GameDataSlinger.NUM_MAX_HOUSES_PER_PROPERTY; } }
    public bool HotelBuilt { get { return propertyBuildingConstructor.HotelOnProperty; } }
    public bool HouseBuilt { get { return propertyBuildingConstructor.HouseOnProperty; } }


    public override void Awake()
    {
        base.Awake();
        propertyBuildingConstructor = GetComponent<PropertyTileBuildingConstructor>();
    }

    private void Start()
    {
        CurrentRentRequired = propertyData.RentDefaultCost;
        PurchaseCost = propertyData.PurchaseCost;
    }

    public override void MortgageTile()
    {
        base.MortgageTile();
    }

    public override void UnmortgageTile()
    {
        base.UnmortgageTile();
    }

    public void ConstructBuilding()
    {
        if (CanBuildHouse)
            BuildHouse();
        else if (CanBuildHotel)
            BuildHotel();
    }

    public void SellBuilding()
    {
        if (HotelBuilt)
            SellHotel();
        else if (HouseBuilt)
            SellHouse();
    }


    public void BuildHouse()
    {
        propertyBuildingConstructor.SpawnHouse();
        Bank.Instance.RemoveMoneyFromAccount(OwnerID, propertyData.HousePurchaseCost);

        BuiltHouseEvent?.Invoke(this);

        CurrentRentRequired = propertyData.RentCostWithHouses[propertyBuildingConstructor.NumHousesBuilt - 1];
    }
    public void BuildHotel()
    {
        propertyBuildingConstructor.SpawnHotel();
        Bank.Instance.RemoveMoneyFromAccount(OwnerID, propertyData.HotelPurchaseCost);

        BuiltHotelEvent?.Invoke(this);

        CurrentRentRequired = propertyData.RentCostWithHotel;
    }
    public void SellHouse()
    {
        propertyBuildingConstructor.RemoveHouse();
        Bank.Instance.AddMoneyToAccount(OwnerID, propertyData.HouseSellValue);
        SoldHouseEvent?.Invoke(this);

        if (propertyBuildingConstructor.HouseOnProperty)
            CurrentRentRequired = propertyData.RentCostWithHouses[propertyBuildingConstructor.NumHousesBuilt - 1];
        else
            CurrentRentRequired = propertyData.RentDefaultCost;
    }
    public void SellHotel()
    {
        propertyBuildingConstructor.RemoveHotel();
        Bank.Instance.AddMoneyToAccount(OwnerID, propertyData.HotelSellValue);
        SoldHotelEvent?.Invoke(this);

        CurrentRentRequired = propertyData.RentCostWithHouses[propertyBuildingConstructor.NumHousesBuilt - 1];
    }

    public void ProcessPlayer(string playerID)
    {
        if (!IsOwned)
        {
            //RaisePlayerLandedOnUnownedPurchasableTileEvent(playerID, photonView.ViewID);
            PlayerLandedOnUnownedPropertyEvent?.Invoke(playerID,this);
        }
        else
        {
            if (!PlayerLandedIsOwner(playerID) && !IsMortgaged)
            {
                //Pay rent.
                Bank.Instance.ProcessRentPayment(playerID, OwnerID, CurrentRentRequired);
            }
        }
    }

    public void RecieveTileData(TileData tileData)
    {
        propertyData = (TileData_Property)tileData;
    }
}