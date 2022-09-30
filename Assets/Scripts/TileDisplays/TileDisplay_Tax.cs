using TMPro;
using UnityEngine;

public class TileDisplay_Tax : TileDisplay
{
    [SerializeField] private TextMeshPro tmpro_Name;
    [SerializeField] private TextMeshPro tmpro_AmountToPay;

    public override void UpdateDisplay(TileData tileData)
    {
        TileData_Tax taxData = (TileData_Tax)tileData;

        tmpro_Name.text = taxData.Name;
        tmpro_AmountToPay.text = $"Pay ${taxData.AmountToPay}";
    }
}