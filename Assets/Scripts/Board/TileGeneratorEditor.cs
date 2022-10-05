using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(BoardGenerator))]
public class TileGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoardGenerator tileGenerator = (BoardGenerator)target;
        if(GUILayout.Button("Build Tiles"))
        {
            tileGenerator.SpawnTilesOnBoard();
        }

        if(GUILayout.Button("Clear Tiles"))
        {
            tileGenerator.ClearTilesOnBoard();
        }
    }
}

#endif