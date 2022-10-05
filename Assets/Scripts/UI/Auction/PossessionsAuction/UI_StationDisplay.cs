using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Tile display for a station that can be shown on the UI. 
//Used by player possessions auction panel to show any stations the player owns.


public class UI_StationDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TMP_StationName;
    [SerializeField] private Image IMG_StationImage;

    public void UpdateDisplay(TileData_Station stationData)
    {
        TMP_StationName.text = stationData.Name;
        IMG_StationImage.sprite = stationData.SPR_Station;
    }
}