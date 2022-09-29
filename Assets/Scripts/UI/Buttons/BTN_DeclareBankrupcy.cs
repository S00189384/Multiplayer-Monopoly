using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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