using UnityEngine;

//Declare bankrupcy button. Can only be clicked on when the player is in the red / bankrupt.
//Enables UI for confirming bankrupcy when clicked.

public class BTN_DeclareBankrupcy : BTN_Base
{
    [SerializeField] private UI_BankrupcyConfirmation bankrupcyConfirmation;

    public override void Awake()
    {
        base.Awake();
        AddOnClickListener(OnMyButtonClicked);
    }

    private void OnMyButtonClicked()
    {
        bankrupcyConfirmation.ShowConfirmationPrompt();
    }
}