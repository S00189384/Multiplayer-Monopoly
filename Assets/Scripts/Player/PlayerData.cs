using System;

[Serializable]
public class PlayerData 
{
    public string ID;
    public string Name;
    //public Player_Piece PiecePrefab;

    public PlayerData(string name/*, Player_Piece piecePrefab*/)
    {
        ID = Guid.NewGuid().ToString();
        Name = name;
        //PiecePrefab = piecePrefab;
    }
}