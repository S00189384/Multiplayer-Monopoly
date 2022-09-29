using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTN_RollDice : BTN_Base
{
    [SerializeField] private UI_DiceThrowManager diceThrowManager;

    private string playerTurn;
    public static int DiceRollValue;

    [SerializeField] private int diceRollValue;

    public override void Awake()
    {
        base.Awake();
        AddOnClickListener(StartRollDiceSequence);
    }

    private void StartRollDiceSequence()
    {
        SetButtonInteractable(false);

        //int randomDiceValue = 1;
        int randomDiceValue = diceThrowManager.GiveRandomDiceValue();
        DiceRollValue = randomDiceValue;
        diceThrowManager.ShowSingleDiceThrow(randomDiceValue, playerName: GameManager.Instance.LocalPlayerNickname, callback: () => PieceMover.Instance.MoveLocalPlayerPieceForwardOverTime(randomDiceValue));
    }
}
