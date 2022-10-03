using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
