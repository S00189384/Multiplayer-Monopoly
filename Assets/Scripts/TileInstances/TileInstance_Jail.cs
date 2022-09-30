//Tile instance for jail tile.
//Landing on this tile does nothing to the player.

public class TileInstance_Jail : TileInstance, iPlayerProcessable
{
    public void ProcessPlayer(string playerID) { }
}
