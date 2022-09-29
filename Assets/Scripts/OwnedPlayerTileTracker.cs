using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable] public class PropertyTypeOwnerDictionary : SerializableDictionary<PropertyColourSet, int> { }

public class OwnedPlayerTileTracker : MonoBehaviourPun
{
    [SerializeField] public List<TileInstance_Property> ownedPropertiesList = new List<TileInstance_Property>();
    [SerializeField] public List<TileInstance_Station> ownedStationsList = new List<TileInstance_Station>();
    [SerializeField] public List<TileInstance_Utility> ownedUtilitiesList = new List<TileInstance_Utility>();

    //List of property types that the player currently owns all properties of.
    public List<PropertyColourSet> AllOwnedPropertiesTypeList = new List<PropertyColourSet>();
    public bool OwnsAllOfAPropertyType { get { return AllOwnedPropertiesTypeList.Count > 0; } }

    private Dictionary<PropertyColourSet, List<TileInstance_Property>> propertiesOwnsAllOfDictionary = new Dictionary<PropertyColourSet, List<TileInstance_Property>>();
    public List<TileInstance_Property> GetPropertiesOwnsAllOf(PropertyColourSet propertyColourSet) => propertiesOwnsAllOfDictionary[propertyColourSet];

    public void AddToAllOwnedPropertyOfTypeList(PropertyColourSet propertyColour)
    {
        OwnsAllOfPropertyTypeEvent?.Invoke(propertyColour,ownedPropertiesList.FindAll(prop => prop.propertyData.PropertyColourSet == propertyColour));

        AllOwnedPropertiesTypeList.Add(propertyColour);
        propertiesOwnsAllOfDictionary.Add(propertyColour, ownedPropertiesList.FindAll(prop => prop.propertyData.PropertyColourSet == propertyColour));
    }
    public void RemoveFromAllOwnedPropertyOfTypeList(PropertyColourSet propertyColour)
    {
        AllOwnedPropertiesTypeList.Remove(propertyColour);
        if (AllOwnedPropertiesTypeList.Count <= 0)
            NoLongerOwnsAllOfAnyPropertyTypeEvent?.Invoke(propertyColour, ownedPropertiesList.FindAll(prop => prop.propertyData.PropertyColourSet == propertyColour));

        propertiesOwnsAllOfDictionary.Remove(propertyColour);
    }
    

    ////Or..
    //[SerializeField] private List<int> ownedPropertiesIndexList = new List<int>();
    //[SerializeField] private List<int> ownedStationsIndexList = new List<int>();
    //[SerializeField] private List<int> ownedUtilitiesIndexList = new List<int>();

    public int OwnedPropertiesCount { get { return ownedPropertiesList.Count; } }
    public int OwnedStationsCount { get { return ownedStationsList.Count; } }
    public int OwnedUtilitiesCount { get { return ownedUtilitiesList.Count; } }

    public bool OwnsAProperty { get { return ownedPropertiesList.Count > 0; } }
    public bool OwnsAStation { get { return ownedStationsList.Count > 0; } }
    public bool OwnsAUtility { get { return ownedUtilitiesList.Count > 0; } }
    public bool OwnsAPurchasableTile { get { return OwnsAProperty || OwnsAStation || OwnsAUtility; } }

    //Player data on their owned properties.
    public bool HasMortgagedProperties { get { return ownedPropertiesList.Any(prop => prop.IsMortgaged); } }
    //public bool HasAHouseBuilt { get { return ownedPropertiesList.Any(prop => prop.HasHouseBuilt); } }
    //public bool HasAHotelBuilt { get { return ownedPropertiesList.Any(prop => prop.HotelBuilt); } }

    private int numOwnedPurchasableTiles { get { return ownedPropertiesList.Count + ownedStationsList.Count + ownedUtilitiesList.Count; } }

    //private bool canMortgageATile;
    //public bool CanMortgageATile 
    //{
    //    get => canMortgageATile;
    //    set
    //    {
    //        canMortgageATile = value;
    //        if (canMortgageATile)
    //            GainedAbilityToMortgageATileEvent?.Invoke();
    //        else
    //            LostAbilityToMortgageATileEvent?.Invoke();
    //    } 
    //}

    public event Action<TileInstance_Purchasable> GainedTileEvent;
    public event Action<TileInstance_Purchasable> LostTileEvent;

    //public event Action Gained

    public event Action<PropertyColourSet,List<TileInstance_Property>> OwnsAllOfPropertyTypeEvent;
    public event Action<PropertyColourSet, List<TileInstance_Property>> NoLongerOwnsAllOfAnyPropertyTypeEvent;

    public event Action GainedAbilityToMortgageATileEvent;
    public event Action LostAbilityToMortgageATileEvent;

    //Dictionary to keep track of the number of owned properties this player owns of a certain type. Key - property type Value - int (no. of properties of that type owned).
    [SerializeField] private PropertyTypeOwnerDictionary propertyTypeOwnedCountDictionary = new PropertyTypeOwnerDictionary();
    public int GetNumberOfOwnedPropertyType(PropertyColourSet propertyColour) => propertyTypeOwnedCountDictionary[propertyColour];

    public void GainProperty(TileInstance_Property propertyInstance)
    {
        ownedPropertiesList.Add(propertyInstance);
        GainedTileEvent?.Invoke(propertyInstance);

        PropertyColourSet colourSetOfProperty = propertyInstance.propertyData.PropertyColourSet;

        if (propertyTypeOwnedCountDictionary.ContainsKey(colourSetOfProperty))
        {
            //testDictionary[colourSetOfProperty].Add(propertyInstance);
            propertyTypeOwnedCountDictionary[colourSetOfProperty]++;
        }
        else
        {
            //testDictionary.Add(propertyInstance.propertyData.PropertyColourSet, new List<TileInstance_Property>() { propertyInstance });
            propertyTypeOwnedCountDictionary.Add(propertyInstance.propertyData.PropertyColourSet, 1);
        }

        //int photonViewIDOfProperty = propertyInstance.photonView.ViewID;
        //photonView.RPC(nameof(GainPropertyRPC), RpcTarget.All, photonViewIDOfProperty);
    }
    public void GainStation(TileInstance_Station stationInstance)
    {
        ownedStationsList.Add(stationInstance);

        GainedTileEvent?.Invoke(stationInstance);
        //int photonViewIDOfStation = stationInstance.photonView.ViewID;
        //photonView.RPC(nameof(GainStationRPC), RpcTarget.All, photonViewIDOfStation);
    }
    public void GainUtility(TileInstance_Utility utilityInstance)
    {
        ownedUtilitiesList.Add(utilityInstance);
        GainedTileEvent?.Invoke(utilityInstance);
        //int photonViewIDOfUtility = utilityInstance.photonView.ViewID;
        //photonView.RPC(nameof(GainUtilityRPC), RpcTarget.All, photonViewIDOfUtility);
    }

    public void RemoveProperty(TileInstance_Property propertyInstance)
    {
        ownedPropertiesList.Remove(propertyInstance);

        PropertyColourSet colourTypeOfProperty = propertyInstance.propertyData.PropertyColourSet;

        propertyTypeOwnedCountDictionary[colourTypeOfProperty]--;
        if (propertyTypeOwnedCountDictionary[colourTypeOfProperty] == 0)
            propertyTypeOwnedCountDictionary.Remove(colourTypeOfProperty);
    }

    public void RemoveStation(TileInstance_Station stationInstance)
    {
        ownedStationsList.Remove(stationInstance);
    }

    public void RemoveUtility(TileInstance_Utility utilityInstance)
    {
        ownedUtilitiesList.Remove(utilityInstance);
    }


    [PunRPC]
    private void GainPropertyRPC(int propertyViewID) => ownedPropertiesList.Add(PhotonNetwork.GetPhotonView(propertyViewID).GetComponent<TileInstance_Property>());
    [PunRPC]
    private void GainStationRPC(int stationViewID) => ownedStationsList.Add(PhotonNetwork.GetPhotonView(stationViewID).GetComponent<TileInstance_Station>());
    [PunRPC]
    private void GainUtilityRPC(int utilityViewID) => ownedUtilitiesList.Add(PhotonNetwork.GetPhotonView(utilityViewID).GetComponent<TileInstance_Utility>());
}