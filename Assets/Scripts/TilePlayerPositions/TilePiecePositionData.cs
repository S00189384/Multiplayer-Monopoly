using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

//Make better system for holding player available positions. Atm it's just a queue which alternates the indexes of the available positions.

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
            //unavailablePlayerPositionIndexesQueue.Enqueue(i);
            availablePlayerPositionIndexesStack.Push(i);
        }
    }

    private void Start()
    {
        //if(tmpro_Debugger != null)
        //{
        //    for (int i = 0; i < availablePlayerPositionIndexesQueue.Count; i++)
        //    {
        //        tmpro_Debugger.text += availablePlayerPositionIndexesQueue.ElementAt(i) + " - ";
        //    }
        //}
    }

    //Notify all clients that a player landed - therefore update its position queue for other players to land.
    public void OnPlayerLanded()
    {
        //playerPiece.transform.position = CurrentPlayerMoveToPosition;
        photonView.RPC("Land", RpcTarget.All);
    }


    //public void PlayerLanded()
    //{
    //    if(photonView.IsMine)
    //        photonView.RPC("Land", RpcTarget.All);
    //}

    public void PlayerLeft()
    {
        photonView.RPC("Left", RpcTarget.All);
    }

    [PunRPC]
    public void Left()
    {
        //availablePlayerPositionIndexesQueue.Enqueue(availablePlayerPositionIndexesQueue.Dequeue());
        availablePlayerPositionIndexesStack.Push(unavailablePlayerPositionIndexesQueue.Dequeue());
    }

    [PunRPC]
    public void Land()
    {
        //playerPiece.transform.position = CurrentPlayerMoveToPosition;

        // availablePlayerPositionIndexesQueue.Enqueue(availablePlayerPositionIndexesQueue.Dequeue());

        //print($"moved piece to {CurrentPlayerMoveToPosition}");
        unavailablePlayerPositionIndexesQueue.Enqueue(availablePlayerPositionIndexesStack.Pop());
        //print($"new target transform is now {CurrentPlayerMoveToPosition}");

        //if (tmpro_Debugger != null)
        //    tmpro_Debugger.text = GetQueueString();
    }
}