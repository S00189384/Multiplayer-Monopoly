using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Class to spawn a row used by the tile generator script to spawn in tiles for a board prefab.

public static class RowSpawner 
{
    //Given row data and a starting transform - spawn in tiles for the row.
    public static List<TileDisplay> SpawnRow(RowData rowData, Transform rowTransform)
    {
        if(rowData == null)
        {
            Debug.LogWarning($"Could not generate row due to missing row data in inspector.");
            return null;
        }

        if(rowData.tileDataList.Any(tileData => tileData == null))
        {
            Debug.LogWarning($"{rowData.name} contains a tile which is missing tile data.");
            return null;
        }

        List<TileDisplay> spawnedTilesList = new List<TileDisplay>();
        Vector3 startingSpawnPosition = rowTransform.position;

        foreach (var tileData in rowData.tileDataList)
        {
            if(tileData.tilePrefab != null)
            {
                //Spawn tile.
                TileDisplay spawnedTile = GameObject.Instantiate(tileData.tilePrefab, startingSpawnPosition, Quaternion.identity);
                spawnedTilesList.Add(spawnedTile);

                //Update display of tile.
                spawnedTile.UpdateDisplay(tileData);

                //Can this tile receive data? If so give it its data.
                iTileDataRecievable tileInstance;
                spawnedTile.TryGetComponent(out tileInstance);
                if(tileInstance != null)
                    tileInstance.RecieveTileData(tileData);

                SpriteRenderer sr = spawnedTile.GetComponent<SpriteRenderer>();
                float tileWidth = sr.bounds.size.x; 

                spawnedTile.transform.SetParent(rowTransform);
                spawnedTile.transform.rotation = rowTransform.rotation;

                startingSpawnPosition -= (rowTransform.right * tileWidth);
            }
        }

        return spawnedTilesList;
    }

    //Destroy tiles in a row given the row parent.
    public static void ClearRow(Transform rowTransform)
    {
        for (int i = rowTransform.childCount; i > 0; --i)
            GameObject.DestroyImmediate(rowTransform.GetChild(0).gameObject);
    }
}