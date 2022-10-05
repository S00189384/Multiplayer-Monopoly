using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script which moves a piece on the board.
//Uses each tiles piece position data on the board to determine the position a player should land on when moving to a tile.
//Various move types available - forward, backward, instant, over time, x num tiles in a direction, to a tile index etc.

public class PieceMover : MonoBehaviour
{
    public static PieceMover Instance;

    [SerializeField] public List<TilePiecePositionData> tilePiecePositonDataList = new List<TilePiecePositionData>();
    private int totalNumTiles;

    [Header("Movement Settings")]
    [SerializeField] private float delayBetweenMovements;
    [SerializeField] private float movementSpeed;

    //Event broadcasted to local client only.
    public static event Action<PlayerMove> PlayerMovedEvent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start() => totalNumTiles = tilePiecePositonDataList.Count;

    public void RecieveTilePositonData(List<TileDisplay> tileDisplays)
    {
        tilePiecePositonDataList.Clear();
        tileDisplays.ForEach(td => tilePiecePositonDataList.Add(td.GetComponent<TilePiecePositionData>()));
    }

    //Move piece instantly to tile.
    public void MovePieceInDirectionInstant(Player_Piece playerPiece, int numTilesToMove,MoveDirectionType moveDirection)
    {
        if (moveDirection == MoveDirectionType.Forward)
            MovePieceForwardInstant(playerPiece, numTilesToMove);
        else if (moveDirection == MoveDirectionType.Backward)
            MovePieceBackwardsInstant(playerPiece, numTilesToMove);

        MovePieceToTileIndexInstant(playerPiece, GetTileIndexForMove(numTilesToMove, playerPiece.TileIndex));
    }
    public void MovePieceForwardInstant(Player_Piece playerPiece, int numTilesToMove)
    {
        MovePieceToTileIndexInstant(playerPiece, GetTileIndexForMove(numTilesToMove, playerPiece.TileIndex));
    }
    public void MovePieceBackwardsInstant(Player_Piece playerPiece, int numTilesToMove)
    {
        MovePieceToTileIndexInstant(playerPiece, GetTileIndexForMove(-numTilesToMove, playerPiece.TileIndex));
    }
    public void MovePieceToTileIndexInstant(Player_Piece playerPiece, int targetTileIndex,bool raiseEvent = true)
    {
        int startingIndex = playerPiece.TileIndex;

        tilePiecePositonDataList[startingIndex].PlayerLeft();
        playerPiece.transform.position = tilePiecePositonDataList[targetTileIndex].CurrentPlayerMoveToPosition;
        playerPiece.SetTileIndex(targetTileIndex);
        tilePiecePositonDataList[targetTileIndex].OnPlayerLanded();

        if(raiseEvent)
            PlayerMovedEvent?.Invoke(new PlayerMove(playerPiece.Player.UserId,startingIndex,targetTileIndex));
    }

    //Move piece over time to tile.
    public void MovePieceInDirectionOverTime(Player_Piece playerPiece, int numTilesToMove,MoveDirectionType moveDirection, Action callback = null)
    {
        if (moveDirection == MoveDirectionType.Forward)
            MovePieceForwardOverTime(playerPiece, numTilesToMove, callback);
        else if (moveDirection == MoveDirectionType.Backward)
            MovePieceBackwardsOverTime(playerPiece, numTilesToMove, callback);
    }
    public void MovePieceForwardOverTime(Player_Piece playerPiece, int numTilesToMove, Action callback = null)
    {
        StartCoroutine(Ien_MovePieceOverTime(playerPiece, GetTileIndexForMove(numTilesToMove, playerPiece.TileIndex), IncrementIndex, callback));
    }
    public void MovePieceBackwardsOverTime(Player_Piece playerPiece, int numTilesToMove, Action callback = null)
    {
        StartCoroutine(Ien_MovePieceOverTime(playerPiece, GetTileIndexForMove(-numTilesToMove, playerPiece.TileIndex), DecrementIndex, callback));
    }

    //Move local piece over time.
    public void MoveLocalPlayerPieceInDirectionOverTime(int numTilesToMove, MoveDirectionType moveDirection, Action callback = null)
    {
        Player_Piece playerPiece = GameManager.Instance.GetLocalPlayerPiece();
        MovePieceInDirectionOverTime(playerPiece, numTilesToMove, moveDirection, callback);
    }
    public void MoveLocalPlayerPieceForwardOverTime(int numTilesToMove, Action callback = null)
    {
        Player_Piece playerPiece = GameManager.Instance.GetLocalPlayerPiece();
        MovePieceForwardOverTime(playerPiece, numTilesToMove, callback);
    }
    public void MoveLocalPlayerPieceBackwardOverTime(int numTilesToMove, Action callback = null)
    {
        Player_Piece playerPiece = GameManager.Instance.GetLocalPlayerPiece();
        MovePieceBackwardsOverTime(playerPiece, numTilesToMove, callback);
    }
    
    //What is the target index for a move if given the starting index and the number of tiles to move.
    //Works for both forward and backwards direction by giving numTilesToMove a pos or neg value.
    private int GetTileIndexForMove(int numTilesToMove, int startIndex) => ((startIndex + numTilesToMove) + totalNumTiles) % totalNumTiles;
    private IEnumerator Ien_MovePieceOverTime(Player_Piece playerPiece, int targetTileIndex,Func<int,int> tileIndexFunc, Action callback = null)
    {
        tilePiecePositonDataList[playerPiece.TileIndex].PlayerLeft();

        int startingIndex = playerPiece.TileIndex;
        int index = startingIndex;

        while (index != targetTileIndex) //Move until piece reaches target tile.
        {
            index = tileIndexFunc(index); //Inc or Dec the current index based on direction to move in.
            index = GetTileIndexForMove(index, totalNumTiles);

            Vector3 target = tilePiecePositonDataList[index].CurrentPlayerMoveToPosition;
            Vector3 startingPiecePosition = playerPiece.transform.position;
            float percentageCompleteLerp = 0f;

            while(percentageCompleteLerp < 1f)
            {
                percentageCompleteLerp += Time.deltaTime * movementSpeed;
                playerPiece.transform.position = Vector3.Lerp(startingPiecePosition, target, percentageCompleteLerp);
                yield return null;
            }

            yield return new WaitForSeconds(delayBetweenMovements);
        }

        playerPiece.SetTileIndex(index);
        tilePiecePositonDataList[targetTileIndex].OnPlayerLanded();

        PlayerMovedEvent?.Invoke(new PlayerMove(playerPiece.Player.UserId,startingIndex,targetTileIndex));
        callback?.Invoke();
    }

    //Start of game.
    public void SetPlayerPositionsStartOfGame(List<GameObject> playerPieces)
    {
        for (int i = 0; i < playerPieces.Count; i++)
        {
            playerPieces[i].transform.position = tilePiecePositonDataList[GameDataSlinger.PLAYER_START_TILE_INDEX].CurrentPlayerMoveToPosition;
            tilePiecePositonDataList[GameDataSlinger.PLAYER_START_TILE_INDEX].OnPlayerLanded();
        }
    }
    public void SetPlayerPositionStartOfGame(GameObject playerPiece)
    {
         playerPiece.transform.position = tilePiecePositonDataList[GameDataSlinger.PLAYER_START_TILE_INDEX].CurrentPlayerMoveToPosition;
         tilePiecePositonDataList[GameDataSlinger.PLAYER_START_TILE_INDEX].OnPlayerLanded();
    }

    //private int GetNextTileIndexForward(int index, int totalNumTiles) => (index + 1) % totalNumTiles;
    private int IncrementIndex(int index) => index + 1;
    private int DecrementIndex(int index) => index - 1;
}

public enum MoveDirectionType
{
    Auto,
    Forward,
    Backward
}

public struct PlayerMove
{
    public string PlayerID;
    public int TileIndexMovedFrom;
    public int TileIndexMovedTo;

    public static event Action<PlayerMove> MovePassedGoEvent;
    
    //Find better way of checking if passed go?
    private int NumTilesMoved { get { return Math.Abs(((GameDataSlinger.NUM_TILES - TileIndexMovedFrom) + TileIndexMovedTo) - GameDataSlinger.NUM_TILES); } }
    private bool PassedGo { get { return TileIndexMovedFrom > TileIndexMovedTo && TileIndexMovedFrom + NumTilesMoved >= GameDataSlinger.NUM_TILES; } }

    public PlayerMove(string playerID,int tileIndexMovedFrom, int tileIndexMovedTo)
    {
        PlayerID = playerID;
        TileIndexMovedFrom = tileIndexMovedFrom;
        TileIndexMovedTo = tileIndexMovedTo;

        if (PassedGo)
            MovePassedGoEvent?.Invoke(this);
    }
}