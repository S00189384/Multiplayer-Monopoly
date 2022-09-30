//Tile instance for go to jail tile.
//Jailor jails the player when they land on this tile.
//Jailor and jailor UI scripts process the player as they are in jail.

public class TileInstance_GoToJail : TileInstance, iPlayerProcessable
{
    public void ProcessPlayer(string playerID)
    {
        Jailor.Instance.JailPlayer(playerID);
    }
}