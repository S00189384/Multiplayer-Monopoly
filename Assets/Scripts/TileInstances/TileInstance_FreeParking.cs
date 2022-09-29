using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInstance_FreeParking : TileInstance, iPlayerProcessable
{
    public void ProcessPlayer(string playerID)
    {
        print("Free parking processed player ");
    }
}
