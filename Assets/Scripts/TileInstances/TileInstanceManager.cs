using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInstanceManager : MonoBehaviour
{
    public static TileInstanceManager Instance;

    [SerializeField] private List<iPlayerProcessable> playerProcessableList = new List<iPlayerProcessable>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        PieceMover.PlayerMovedEvent += OnPlayerFinishedMove;
    }

    private void Start()
    {
        Board gameBoard = FindObjectOfType<Board>();

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
