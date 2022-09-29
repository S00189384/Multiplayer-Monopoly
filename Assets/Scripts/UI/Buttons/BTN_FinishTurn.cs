using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Disable at start of turn
//Disable when dice roll is active / player piece is moving. 

//Enable when player piece has reached its destination.


public class BTN_FinishTurn : BTN_Base
{
    //[SerializeField] private PlayerTurnManager playerTurnManager;


    //public override void Awake()
    //{
    //    base.Awake();

    //    AddOnClickListener(FinishTurn);

    //    //PlayerTurnManager.NewTurnEvent += OnNewPlayerTurn;
    //    PieceMover.PlayerMovedEvent += OnPlayerStoppedMovingPiece;
    //}

    //private void Start()
    //{
    //    SetButtonInteractable(false);
    //}

    //private void OnPlayerStoppedMovingPiece(PlayerMove pieceMove)
    //{
    //    SetButtonInteractable(true);
    //}

    //private void OnNewPlayerTurn(string player)
    //{
    //    SetButtonInteractable(false);
    //}

    //public void FinishTurn()
    //{
    //    playerTurnManager.EndPlayerTurn();
    //    SetButtonInteractable(false);
    //}

    //private void OnDestroy()
    //{
    //    //PlayerTurnManager.NewTurnEvent -= OnNewPlayerTurn;
    //    PieceMover.PlayerMovedEvent -= OnPlayerStoppedMovingPiece;
    //}
}
