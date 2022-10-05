using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//Attached to each tile, this script handles the positions that players stay in when landing on a tile.
//When landing on a tile, this script marks the position the player lands in as being "unavailable" by adding it to an "unavailable" positions queue.
//When leaving a tile, the script removes this unavailable position from the queue and adds it to the stack of available positions a player can use to stay in on this tile.
//The current position for a player to be in on a tile is the first element at the top of the available positions stack.

//TODO:
//If a player disconnects, the tile position they filled up will remain marked as being unavailable. Not a big deal but should be fixed.

public class TilePiecePositionData : MonoBehaviourPun
{
    [SerializeField] protected List<Transform> playerMoveToTransforms = new List<Transform>();
    protected Queue<int> unavailablePlayerPositionIndexesQueue = new Queue<int>();
    protected Stack<int> availablePlayerPositionIndexesStack = new Stack<int>();
    public Vector3 CurrentPlayerMoveToPosition { get { return playerMoveToTransforms[availablePlayerPositionIndexesStack.Peek()].position; } }

    private void Awake()
    {
        for (int i = 0; i < playerMoveToTransforms.Count; i++)
        {
            availablePlayerPositionIndexesStack.Push(i);
        }
    }

    //Notify all clients that a player landed - therefore update its position queue for other players to land.
    public void OnPlayerLanded()
    {
        photonView.RPC(nameof(Land), RpcTarget.All);
    }

    public void PlayerLeft()
    {
        photonView.RPC(nameof(Left), RpcTarget.All);
    }

    [PunRPC]
    private void Left()
    {
        availablePlayerPositionIndexesStack.Push(unavailablePlayerPositionIndexesQueue.Dequeue());
    }

    [PunRPC]
    private void Land()
    {
        unavailablePlayerPositionIndexesQueue.Enqueue(availablePlayerPositionIndexesStack.Pop());
    }
}