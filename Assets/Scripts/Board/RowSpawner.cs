using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RowSpawner 
{
    public static List<TileDisplay> SpawnRow(RowData rowData, Transform rowTransform)
    {
        if(rowData == null)
        {
            Debug.LogWarning($"Could not generate row due to missing row data in inspector.");
            return null;
        }

        if(rowData.tileDataList.Any(tileData => tileData == null))
        {
            Debug.LogWarning($"{rowData.name} contains missing tile data.");
            return null;
        }

        List<TileDisplay> spawnedTilesList = new List<TileDisplay>();
        Vector3 startingSpawnPosition = rowTransform.position;

        foreach (var tileData in rowData.tileDataList)
        {
            if(tileData.tilePrefab != null)
            {
                TileDisplay spawnedTile = GameObject.Instantiate(tileData.tilePrefab, startingSpawnPosition, Quaternion.identity);
                spawnedTilesList.Add(spawnedTile);

                //Update display of tile.
                spawnedTile.UpdateDisplay(tileData);

                //Give tile instance its tile data.
                iTileDataRecievable tileInstance;
                spawnedTile.TryGetComponent(out tileInstance);
                if(tileInstance != null)
                    tileInstance.RecieveTileData(tileData);

                SpriteRenderer sr = spawnedTile.GetComponent<SpriteRenderer>();
                float tileWidth = sr.bounds.size.x; //This depends on prefab.

                spawnedTile.transform.SetParent(rowTransform);
                spawnedTile.transform.rotation = rowTransform.rotation;

                startingSpawnPosition -= (rowTransform.right * tileWidth);
            }
        }

        return spawnedTilesList;
    }

    //public static TileDisplay SpawnTile(TileData tileData,Vector3 spawnPosition)
    //{
    //    TileDisplay spawnedTile = GameObject.Instantiate(tileData.tilePrefab, spawnPosition, Quaternion.identity);
    //    spawnedTile.UpdateDisplay(tileData);


    //    return null;
    //}

    public static void ClearRow(Transform rowTransform)
    {
        for (int i = rowTransform.childCount; i > 0; --i)
            GameObject.DestroyImmediate(rowTransform.GetChild(0).gameObject);
    }
}


