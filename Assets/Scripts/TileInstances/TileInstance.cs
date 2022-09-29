using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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