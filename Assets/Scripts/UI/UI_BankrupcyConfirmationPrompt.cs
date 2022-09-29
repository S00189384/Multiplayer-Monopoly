using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BankrupcyConfirmationPrompt : MonoBehaviour
{
    private const string hasAbilityToLeaveBankrupcyMessage = "You are able to leave bankrupcy by selling your current possessions!";
    private const string doesNotHaveAbilityToLeaveBankrupcyMessage = "You are not able to leave bankrupcy based on the money you can currently earn from selling / mortgaging your possessions.";


    [SerializeField] private TextMeshProUGUI TMP_AbilityToLeaveBankrupcy;
    [SerializeField] private Button BTN_No;
    [SerializeField] private Button BTN_Yes;

    public void UpdateDisplay(Action noButtonAction, Action yesButtonAction,bool canLeaveBankrupcy)
    {
        BTN_No.onClick.AddListener(()=>noButtonAction());
        BTN_Yes.onClick.AddListener(()=>yesButtonAction());

        if (canLeaveBankrupcy)
            TMP_AbilityToLeaveBankrupcy.text = hasAbilityToLeaveBankrupcyMessage;
        else
            TMP_AbilityToLeaveBankrupcy.text = doesNotHaveAbilityToLeaveBankrupcyMessage;
    }
}
