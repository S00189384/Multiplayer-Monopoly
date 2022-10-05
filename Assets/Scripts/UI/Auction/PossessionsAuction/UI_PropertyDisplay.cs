using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Tile display for a property that can be shown on the UI. 
//Used by player possessions auction panel to show any properties the player owns.

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