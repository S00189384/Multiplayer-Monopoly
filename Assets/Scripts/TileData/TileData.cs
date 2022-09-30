using UnityEngine;

//Scriptable object which holds the data for a tile.
//This data can then be used by a tile instance and tile display.

[CreateAssetMenu(fileName = "TileData", menuName = "TileData")]
public abstract class TileData : ScriptableObject
{
    public string Name;
    public TileDisplay tilePrefab;
    public TileType tileType;
}

public enum TileType
{
    Property,
    Station,
    Utility,
    Tax,
    CommunityChest,
    Chance,
    Start,
    FreeParking,
    GoToJail,
    VisitingJail
}