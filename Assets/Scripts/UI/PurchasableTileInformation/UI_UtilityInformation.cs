using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UtilityInformation : MonoBehaviour
{
    [Header("My UI Components")]
    [SerializeField] private Image IMG_Utility;
    [SerializeField] private TextMeshProUGUI TMP_UtilityName;
    [SerializeField] private TextMeshProUGUI TMP_UtilityRentOneOwned;
    [SerializeField] private TextMeshProUGUI TMP_UtilityRentBothOwned;
    [SerializeField] private TextMeshProUGUI TMP_MortgageValue;

    public void UpdateDisplay(TileData_Utility utilityData)
    {
        IMG_Utility.sprite = utilityData.SPR_Utility;
        TMP_UtilityName.text = utilityData.Name;
        TMP_UtilityRentOneOwned.text = $"If one \"Utility\" is owned, rent is {GameDataSlinger.UTILITY_RENT_DICE_MULTIPLIERS[0]} times amount shown on dice.";
        TMP_UtilityRentBothOwned.text = $"If both \"Utilities\" are owned, rent is {GameDataSlinger.UTILITY_RENT_DICE_MULTIPLIERS[1]} times amount shown on dice.";
        TMP_MortgageValue.text = $"Mortgage Value ${utilityData.MortgageValue}";
    }
}