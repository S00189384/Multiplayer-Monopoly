using System.Collections.Generic;
using UnityEngine;

//Script attached to a board which can spawn in or clear the tiles for this board.
//Corner tiles are treated seperately to normal tiles in a row for now.
//When tiles are spawned, other scripts such as tile ownership manager receive the new tile data from the board.

public class BoardGenerator : MonoBehaviour
{
    [SerializeField] private BoardData boardData;
    [SerializeField] private Transform[] rowStartingPositions = new Transform[GameDataSlinger.NUM_ROWS];

    [SerializeField] private List<TileDisplay> cornerTiles = new List<TileDisplay>();
    private List<TileDisplay> spawnedTilesList = new List<TileDisplay>(); 

    public void SpawnTilesOnBoard()
    {
        if(spawnedTilesList.Count >= GameDataSlinger.NUM_TILES)
        {
            Debug.LogWarning("Did not generate tiles for board as board has already been generated.");
            return;
        }

        if(boardData == null)
        {
            Debug.LogWarning("Could not generate tiles for board - missing board data in inspector.");
            return;
        }

        int tileIndex = 0;
        for (int i = 0; i < GameDataSlinger.NUM_ROWS; i++)
        {
            var spawnedTilesForThisRow = RowSpawner.SpawnRow(boardData.rows[i], rowStartingPositions[i]);

            if(spawnedTilesForThisRow != null)
            {
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

        Debug.Log("Tiles for board spawned.");

        //Give scripts the board tile data that was just spawned.
        Board board = GetComponent<Board>();
        board.RecieveBoardTileData(spawnedTilesList);

        PieceMover pieceMover = FindObjectOfType<PieceMover>();
        pieceMover.RecieveTilePositonData(spawnedTilesList);

        TileOwnershipManager tileOwnershipManager = FindObjectOfType<TileOwnershipManager>();
        tileOwnershipManager.RecieveBoard(board);
    }

    public void ClearTilesOnBoard()
    {
        for (int i = 0; i < GameDataSlinger.NUM_ROWS; i++)
        {
            RowSpawner.ClearRow(rowStartingPositions[i]);
        }

        spawnedTilesList.Clear();

        Debug.Log("Board cleared.");
    }
}

