using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_BankrupcyConfirmationPrompt : MonoBehaviour
{
    [SerializeField] private Button BTN_No;
    [SerializeField] private Button BTN_Yes;

    public void UpdateDisplay(Action noButtonAction, Action yesButtonAction)
    {
        BTN_No.onClick.AddListener(()=>noButtonAction());
        BTN_Yes.onClick.AddListener(()=>yesButtonAction());
    }
}
