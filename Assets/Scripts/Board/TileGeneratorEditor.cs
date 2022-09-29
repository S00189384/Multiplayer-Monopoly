using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(TileGenerator))]
public class TileGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileGenerator tileGenerator = (TileGenerator)target;
        if(GUILayout.Button("Build Tiles"))
        {
            tileGenerator.SpawnTiles();
        }

        if(GUILayout.Button("Clear Tiles"))
        {
            tileGenerator.ClearTiles();
        }
    }
}

#endif