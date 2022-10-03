using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PropertyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TMP_PropertyName;
    [SerializeField] private Image IMG_PropertyColour;

    public void UpdateDisplay(TileData_Property propertyData)
    {
        TMP_PropertyName.text = propertyData.Name;
        IMG_PropertyColour.color = propertyData.PropertyColourSet.propertyColour;
    }
}
