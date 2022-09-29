using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager<T> 
{
    protected LinkedList<T> turns;
    protected LinkedListNode<T> currentTurn;
    public T CurrentTurn { get { return currentTurn.Value; } }
    public T GetFirst { get { return turns.First.Value; } }
    public int NumTurnsRemaining { get { return turns.Count; } }
    public bool OneRemainingTurn { get { return turns.Count == 1; } }
    public bool ContainsTurn(T turn) { return turns.Contains(turn); }
    public void RemoveTurn(T turn)
    {
        turns.Remove(turn);
        RemovedTurns.Add(turn);

        if (OneRemainingTurn)
            OneTurnRemainingEvent?.Invoke(GetFirst);
    }

    public List<T> RemovedTurns = new List<T>();

    public event Action<T> NewTurnEvent;
    public event Action<T> OneTurnRemainingEvent;

    public void Initialise(List<T> data)
    {
        turns = new LinkedList<T>(data);
        currentTurn = turns.First;
    }

    public void MoveToNextTurn()
    {
        currentTurn = currentTurn.Next != null ? currentTurn.Next : turns.First;
        NewTurnEvent?.Invoke(currentTurn.Value);
    }

    public void ClearData()
    {
        turns = null;
        currentTurn = null;
    }
}