using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Tile displays of this board")]
    public List<TileDisplay> boardTiles = new List<TileDisplay>();

    public List<TileInstance_Purchasable> purchasableTilesList = new List<TileInstance_Purchasable>(); //Remove later - prob not needed.

    [Header("Property Tile Info")]
    [Tooltip("Holds all the different types of property types that are found on the current board.")]
    [SerializeField] private List<PropertyColourSet> propertyColourSetsList = new List<PropertyColourSet>();

    [Tooltip("Dictionary for each property colour and the amount of tiles that are of that type on the current board")]
    public PropertyTypeInstanceCountDictionary propertyTypeInstanceCountDictionary = new PropertyTypeInstanceCountDictionary();

    //public Hashtable photonIDPropertyTypeHashTable = new Hashtable();

    [Header("Total count of various tile types")]
    public int TotalNumberOfPropertiesOnBoard = 0;
    public int TotalNumberOfUtilitiesOnBoard = 0;
    public int TotalNumberOfStationsOnBoard = 0;
    //public int PropertyColourTypesCount = 0;

    public void RecieveBoardTileData(List<TileDisplay> tiles)
    {
        boardTiles = tiles;

        //In case board has been generated twice - clear previous data.
        propertyColourSetsList.Clear();
        propertyTypeInstanceCountDictionary.Clear();
        purchasableTilesList.Clear();
        TotalNumberOfPropertiesOnBoard = 0;
        TotalNumberOfUtilitiesOnBoard = 0;
        TotalNumberOfStationsOnBoard = 0;

        UpdateBoardProperties();
    }

    //Determine the type of each tile and save data.
    private void UpdateBoardProperties()
    {
        //Go through each tile in the board and check its type.
        TileInstance_Property propertyInstance;
        TileInstance_Utility utilityInstance;
        TileInstance_Station stationInstance;

        for (int i = 0; i < boardTiles.Count; i++)
        {
            //Find property colour types information.
            boardTiles[i].TryGetComponent(out propertyInstance);
            if (propertyInstance)
            {
                purchasableTilesList.Add(propertyInstance);

                TotalNumberOfPropertiesOnBoard++;
                PropertyColourSet propertyColourType = propertyInstance.propertyData.PropertyColourSet;
                //If this property's colour type has not been registered yet, add it.
                if (!propertyTypeInstanceCountDictionary.ContainsKey(propertyColourType))
                {
                    propertyColourSetsList.Add(propertyColourType);
                    propertyTypeInstanceCountDictionary.Add(propertyColourType, 0);
                    //PropertyColourTypesCount++;
                }

                propertyTypeInstanceCountDictionary[propertyColourType] += 1;
            }
            else //Not a property. Try utility
            {
                boardTiles[i].TryGetComponent(out utilityInstance);
                if (utilityInstance)
                {
                    purchasableTilesList.Add(utilityInstance);
                    TotalNumberOfUtilitiesOnBoard++;
                }
                else //Not a utility. Try station
                {
                    boardTiles[i].TryGetComponent(out stationInstance);
                    if (stationInstance)
                    {
                        purchasableTilesList.Add(stationInstance);
                        TotalNumberOfStationsOnBoard++;
                    }
                }
            }
        }
    }
}

[Serializable] public class PropertyTypeInstanceCountDictionary : SerializableDictionary<PropertyColourSet, int> { }