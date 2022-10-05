using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//UI for a single dice throw.
//Enabled by dice throw manager.
//TODO: Remove try catch block when finished.

public class UI_SingleDiceThrowPanel : UI_DiceThrowPanel
{
    [SerializeField] private TextMeshProUGUI TMP_DiceThrowValue;
    [SerializeField] private Image IMG_Dice;

    public void UpdateDisplay(string playerName, int diceValueToShow)
    {
        TMP_Header.text = $"{playerName} throws a dice";

        IMG_Dice.sprite = diceSideSprites[diceValueToShow - 1];
    }
}