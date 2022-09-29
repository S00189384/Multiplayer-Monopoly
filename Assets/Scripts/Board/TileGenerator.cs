using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: Better way of giving other classes tile data like piece mover, instance manager etc.
//Don't allow spawn tiles when the board has already been fully made.

public class TileGenerator : MonoBehaviour
{
    [SerializeField] private BoardData boardData;
    [SerializeField] private Transform[] rowStartingPositions = new Transform[GameDataSlinger.NUM_ROWS];

    [SerializeField] private List<TileDisplay> cornerTiles = new List<TileDisplay>();
    private List<TileDisplay> spawnedTilesList = new List<TileDisplay>(); 

    public void SpawnTiles()
    {
        if(boardData == null)
        {
            Debug.LogWarning("Could not generate tiles - missing board data in inspector.");
            return;
        }

        int tileIndex = 0;
        for (int i = 0; i < GameDataSlinger.NUM_ROWS; i++)
        {
            var spawnedTilesForThisRow = RowSpawner.SpawnRow(boardData.rows[i], rowStartingPositions[i]);

            if(spawnedTilesForThisRow != null)
            {
                //spawnedTilesForThisRow.Insert(0, cornerTiles[i]);

                cornerTiles[i].GetComponent<TileInstance>().TileBoardIndex = tileIndex;
                tileIndex++;

                //Set tile index of each tile in this row.
                for (int j = 0; j < spawnedTilesForThisRow.Count; j++)
                {
                    spawnedTilesForThisRow[j].GetComponent<TileInstance>().TileBoardIndex = tileIndex;
                    tileIndex++;
                }

                spawnedTilesList.Add(cornerTiles[i]);
                spawnedTilesList.AddRange(spawnedTilesForThisRow);
            }
        }

        print("Spawned tiles on board");

        //Rework? Maybe have scripts below listen to board generated event.

        Board board = GetComponent<Board>();
        board.RecieveBoardTileData(spawnedTilesList);

        PieceMover pieceMover = FindObjectOfType<PieceMover>();
        pieceMover.RecieveTilePositonData(spawnedTilesList);

        TileOwnershipManager tileOwnershipManager = FindObjectOfType<TileOwnershipManager>();
        tileOwnershipManager.RecieveBoard(board);
    }

    public void ClearTiles()
    {
        for (int i = 0; i < GameDataSlinger.NUM_ROWS; i++)
        {
            RowSpawner.ClearRow(rowStartingPositions[i]);
        }

        spawnedTilesList.Clear();

        print("Tiles cleared.");
    }
}

