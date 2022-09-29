using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_InJailOptionsPanel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BTN_Base BTN_PayFee;
    [SerializeField] private BTN_Base BTN_RollDouble;
    [SerializeField] private BTN_Base BTN_UseCard;
    [SerializeField] private TextMeshProUGUI TMP_MaxNumTurnsLeftInJail;

    public void UpdateDisplay(int maxNumTurnsLeftInJail,bool hasGetOutOfJailCard,Action payFeeCallback,Action rollDoubleCallback,Action useCardCallback = null)
    {
        BTN_PayFee.SetButtonText($"Pay ${GameDataSlinger.LEAVE_JAIL_FEE_COST}");
        TMP_MaxNumTurnsLeftInJail.text = $"Max number of turns left in jail: {maxNumTurnsLeftInJail}";
        BTN_UseCard.gameObject.SetActive(hasGetOutOfJailCard);

        BTN_PayFee.AddOnClickListener(() => payFeeCallback());
        BTN_RollDouble.AddOnClickListener(() => rollDoubleCallback());

        if (hasGetOutOfJailCard)
            BTN_UseCard.AddOnClickListener(() => useCardCallback());
    }
}