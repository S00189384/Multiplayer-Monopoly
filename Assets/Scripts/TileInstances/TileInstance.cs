using Photon.Pun;
using UnityEngine;

//A tile instance is a spawned tile that is on the board and processes players as they land on them.
//A tile instance uses tile data to receive the values of its tile such as rent cost etc. 

public abstract class TileInstance : MonoBehaviourPun
{
    [SerializeField] private int tileBoardIndex;

    public int TileBoardIndex { get => tileBoardIndex; set => tileBoardIndex = value; }

    //public abstract void ProcessPlayer(string playerID);
    //public abstract void RecieveTileData(TileData tileData);
}

public interface iPlayerProcessable
{
    public void ProcessPlayer(string playerID);
}

public interface iTileDataRecievable
{
    public void RecieveTileData(TileData tileData);
}