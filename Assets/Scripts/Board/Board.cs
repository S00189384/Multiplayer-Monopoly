using System;
using System.Collections.Generic;
using UnityEngine;

//A board which contains tiles.
//Tile generator script can spawn in tiles for a board prefab using a button in the inspector and this script receives the tiles that were spawned.
//When receiving the tiles, this script updates various properties about the board such as the total number of property's of each colour type.

public class Board : MonoBehaviour
{
    [Header("Tile displays of this board")]
    public List<TileDisplay> boardTiles = new List<TileDisplay>();

    [Header("Property Tile Info")]
    [Tooltip("Holds all the different types of property types that are found on the current board.")]
    [SerializeField] private List<PropertyColourSet> propertyColourSetsList = new List<PropertyColourSet>();

    [Tooltip("Dictionary for each property colour and the amount of tiles that are of that type on the current board")]
    public PropertyTypeInstanceCountDictionary propertyTypeInstanceCountDictionary = new PropertyTypeInstanceCountDictionary();

    [Header("Total count of various tile types")]
    public int TotalNumberOfPropertiesOnBoard = 0;
    public int TotalNumberOfUtilitiesOnBoard = 0;
    public int TotalNumberOfStationsOnBoard = 0;

    public void RecieveBoardTileData(List<TileDisplay> tiles)
    {
        boardTiles = tiles;

        //In case board has been generated twice - clear previous data.
        ResetBoardData();

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
                TotalNumberOfPropertiesOnBoard++;
                PropertyColourSet propertyColourType = propertyInstance.propertyData.PropertyColourSet;
                //If this property's colour type has not been registered yet, add it.
                if (!propertyTypeInstanceCountDictionary.ContainsKey(propertyColourType))
                {
                    propertyColourSetsList.Add(propertyColourType);
                    propertyTypeInstanceCountDictionary.Add(propertyColourType, 0);
                }

                propertyTypeInstanceCountDictionary[propertyColourType] += 1;
            }
            else //Not a property. Try utility
            {
                boardTiles[i].TryGetComponent(out utilityInstance);
                if (utilityInstance)
                {
                    TotalNumberOfUtilitiesOnBoard++;
                }
                else //Not a utility. Try station
                {
                    boardTiles[i].TryGetComponent(out stationInstance);
                    if (stationInstance)
                    {
                        TotalNumberOfStationsOnBoard++;
                    }
                }
            }
        }
    }

    private void ResetBoardData()
    {
        propertyColourSetsList.Clear();
        propertyTypeInstanceCountDictionary.Clear();
        TotalNumberOfPropertiesOnBoard = 0;
        TotalNumberOfUtilitiesOnBoard = 0;
        TotalNumberOfStationsOnBoard = 0;
    }
}

[Serializable] public class PropertyTypeInstanceCountDictionary : SerializableDictionary<PropertyColourSet, int> { }