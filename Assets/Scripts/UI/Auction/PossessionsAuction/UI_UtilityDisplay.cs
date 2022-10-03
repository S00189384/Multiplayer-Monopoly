using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UtilityDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TMP_UtilityName;
    [SerializeField] private Image IMG_UtilityImage;

    public void UpdateDisplay(TileData_Utility utilityData)
    {
        TMP_UtilityName.text = utilityData.Name;
        IMG_UtilityImage.sprite = utilityData.SPR_Utility;
    }
}