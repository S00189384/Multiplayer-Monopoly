using UnityEngine;

//Tile data for a chance tile.
//Chance tiles can have different colours of their question mark.

[CreateAssetMenu(fileName = "TileData_Chance", menuName = "TileData_Chance")]
public class TileData_Chance : TileData
{
    public Color questionMarkColour;
}
