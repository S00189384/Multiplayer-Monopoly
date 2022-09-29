using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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