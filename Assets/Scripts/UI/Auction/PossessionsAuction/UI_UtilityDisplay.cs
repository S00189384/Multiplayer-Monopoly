using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Tile display for a utility that can be shown on the UI. 
//Used by player possessions auction panel to show any utilities the player owns.

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