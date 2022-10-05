using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

//Attached to each player and tracks their owned tiles.
//Keeps track of whether the player owns all properties belonging to a colour type as well.

[Serializable] public class PropertyTypeOwnerDictionary : SerializableDictionary<PropertyColourSet, int> { }

public class OwnedPlayerTileTracker : MonoBehaviourPun
{
    [SerializeField] public List<TileInstance_Property> ownedPropertiesList = new List<TileInstance_Property>();
    [SerializeField] public List<TileInstance_Station> ownedStationsList = new List<TileInstance_Station>();
    [SerializeField] public List<TileInstance_Utility> ownedUtilitiesList = new List<TileInstance_Utility>();

    //List of property types that the player currently owns all properties of.
    public List<PropertyColourSet> AllOwnedPropertiesTypeList = new List<PropertyColourSet>();

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
    
    public int OwnedPropertiesCount { get { return ownedPropertiesList.Count; } }
    public int OwnedStationsCount { get { return ownedStationsList.Count; } }
    public int OwnedUtilitiesCount { get { return ownedUtilitiesList.Count; } }

    public bool OwnsAProperty { get { return ownedPropertiesList.Count > 0; } }
    public bool OwnsAStation { get { return ownedStationsList.Count > 0; } }
    public bool OwnsAUtility { get { return ownedUtilitiesList.Count > 0; } }
    public bool OwnsAPurchasableTile { get { return OwnsAProperty || OwnsAStation || OwnsAUtility; } }

    public event Action<PropertyColourSet,List<TileInstance_Property>> OwnsAllOfPropertyTypeEvent;
    public event Action<PropertyColourSet, List<TileInstance_Property>> NoLongerOwnsAllOfAnyPropertyTypeEvent;

    //Dictionary to keep track of the number of owned properties this player owns of a certain type. Key - property type Value - int (no. of properties of that type owned).
    [SerializeField] private PropertyTypeOwnerDictionary propertyTypeOwnedCountDictionary = new PropertyTypeOwnerDictionary();
    public int GetNumberOfOwnedPropertyType(PropertyColourSet propertyColour) => propertyTypeOwnedCountDictionary[propertyColour];

    //Gaining tiles.
    public void GainProperty(TileInstance_Property propertyInstance)
    {
        ownedPropertiesList.Add(propertyInstance);

        PropertyColourSet colourSetOfProperty = propertyInstance.propertyData.PropertyColourSet;

        if (propertyTypeOwnedCountDictionary.ContainsKey(colourSetOfProperty))
        {
            propertyTypeOwnedCountDictionary[colourSetOfProperty]++;
        }
        else
        {
            propertyTypeOwnedCountDictionary.Add(propertyInstance.propertyData.PropertyColourSet, 1);
        }
    }
    public void GainStation(TileInstance_Station stationInstance)
    {
        ownedStationsList.Add(stationInstance);
    }
    public void GainUtility(TileInstance_Utility utilityInstance)
    {
        ownedUtilitiesList.Add(utilityInstance);
    }

    //Removing tiles.
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
}