using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DoubleDiceThrowPanel : UI_DiceThrowPanel
{
    [SerializeField] private TextMeshProUGUI TMP_FirstDiceThrowValue;
    [SerializeField] private TextMeshProUGUI TMP_SecondDiceThrowValue;

    [SerializeField] private Image IMG_FirstDice;
    [SerializeField] private Image IMG_SecondDice;

    public static event Action SuccessfullyRolledADouble;
    public static event Action UnsuccessfullyRolledADouble;

    public void UpdateDisplay(string playerName, int firstDiceValue,int secondDiceValue)
    {
        string headerText;
        if(firstDiceValue == secondDiceValue)
        {
            headerText = $"{playerName} successfully rolled a double and left jail!";
            SuccessfullyRolledADouble?.Invoke();
        }
        else
        {
            headerText = $"{playerName} failed to roll a double and stays in jail";
            UnsuccessfullyRolledADouble?.Invoke();
        }

        TMP_Header.text = headerText;

        IMG_FirstDice.sprite = diceSideSprites[firstDiceValue - 1];
        IMG_SecondDice.sprite = diceSideSprites[secondDiceValue - 1];
    }
}
