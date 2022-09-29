using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInstance_Jail : TileInstance, iPlayerProcessable
{
    public void ProcessPlayer(string playerID)
    {
        print("Jail processed player ");
    }
}
