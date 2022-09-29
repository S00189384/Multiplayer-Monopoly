using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInstance_GoToJail : TileInstance, iPlayerProcessable
{
    public void ProcessPlayer(string playerID)
    {
        print("go to jail processed player");
        Jailor.Instance.JailPlayer(playerID);
    }
}