using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StationInformation : MonoBehaviour
{
    [Header("My UI Components")]
    [SerializeField] private Image IMG_Station;
    [SerializeField] private TextMeshProUGUI TMP_StationName;
    [SerializeField] private TextMeshProUGUI TMP_Rent;
    [SerializeField] private TextMeshProUGUI TMP_Rent2Owned;
    [SerializeField] private TextMeshProUGUI TMP_Rent3Owned;
    [SerializeField] private TextMeshProUGUI TMP_Rent4Owned;
    [SerializeField] private TextMeshProUGUI TMP_MortgageValue;

    public void UpdateDisplay(TileData_Station stationData)
    {
        IMG_Station.sprite = stationData.SPR_Station;
        TMP_StationName.text = stationData.Name;
        TMP_Rent.text = $"${stationData.RentDefaultCost}";
        TMP_Rent2Owned.text = $"${stationData.GetRentCost(2)}";
        TMP_Rent3Owned.text = $"${stationData.GetRentCost(3)}";
        TMP_Rent4Owned.text = $"${stationData.GetRentCost(4)}";
        TMP_MortgageValue.text = $"Mortgage Value ${stationData.MortgageValue}";
    }
}
