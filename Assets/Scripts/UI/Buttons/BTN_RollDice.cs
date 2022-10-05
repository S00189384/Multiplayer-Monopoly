using UnityEngine;

//Button for rolling a dice to move forward on the board.
//Calls on ui dice throw manager to show the result of the dice throw to the player. 

public class BTN_RollDice : BTN_Base
{
    [SerializeField] private UI_DiceThrowManager diceThrowManager;

    public static int DiceRollValue;

    public override void Awake()
    {
        base.Awake();
        AddOnClickListener(StartRollDiceSequence);
    }

    private void StartRollDiceSequence()
    {
        SetButtonInteractable(false);

        int randomDiceValue = diceThrowManager.GetRandomDiceValue();
        DiceRollValue = randomDiceValue;
        diceThrowManager.ShowSingleDiceThrow(randomDiceValue, playerName: GameManager.Instance.LocalPlayerNickname, callback: () => PieceMover.Instance.MoveLocalPlayerPieceForwardOverTime(randomDiceValue));
    }
}