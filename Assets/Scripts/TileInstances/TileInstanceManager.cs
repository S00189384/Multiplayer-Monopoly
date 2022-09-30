using System.Collections.Generic;
using UnityEngine;

//Script which processes the player on a tile that they land on.
//When a player moves - a player move event is triggered and this script listens for this.
//When a player has finished their move this script calls on the tile that they landed on to process the player.

public class TileInstanceManager : MonoBehaviour
{
    [SerializeField] private List<iPlayerProcessable> playerProcessableList = new List<iPlayerProcessable>();

    private void Awake()
    {
        PieceMover.PlayerMovedEvent += OnPlayerFinishedMove;
    }

    private void Start()
    {
        Board gameBoard = FindObjectOfType<Board>();

        //For each tile on the board - if the tile can process the player, keep track of the tile.
        for (int i = 0; i < gameBoard.boardTiles.Count; i++)
        {
            iPlayerProcessable iPlayerProcessable;
            gameBoard.boardTiles[i].TryGetComponent(out iPlayerProcessable);
            if (iPlayerProcessable != null)
                playerProcessableList.Add(iPlayerProcessable);
        }
    }

    private void OnPlayerFinishedMove(PlayerMove pieceMove)
    {
        playerProcessableList[pieceMove.TileIndexMovedTo].ProcessPlayer(pieceMove.PlayerID);
    }

    public void RecieveTileInstanceData(List<iPlayerProcessable> tileInstances)
    {
        playerProcessableList = tileInstances;
    }

    private void OnDestroy()
    {
        PieceMover.PlayerMovedEvent -= OnPlayerFinishedMove;
    }
}
