using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO: Remove try catch block when finished.

public class UI_SingleDiceThrowPanel : UI_DiceThrowPanel
{
    [SerializeField] private TextMeshProUGUI TMP_DiceThrowValue;
    [SerializeField] private Image IMG_Dice;

    public void UpdateDisplay(string playerName, int diceValueToShow)
    {
        TMP_Header.text = $"{playerName} throws a dice";
        //TMP_DiceThrowValue.text = diceValueToShow.ToString();

        try
        {
            IMG_Dice.sprite = diceSideSprites[diceValueToShow - 1];
        }
        catch (IndexOutOfRangeException exception)
        {

        }
    }
}