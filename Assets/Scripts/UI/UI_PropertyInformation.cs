using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PropertyInformation : MonoBehaviour
{
    [Header("My Components")]
    [SerializeField] private Image imgPropertyColour;
    [SerializeField] private TextMeshProUGUI TMPRO_PropertyName;
    [SerializeField] private TextMeshProUGUI TMPRO_RentCost;
    [SerializeField] private TextMeshProUGUI TMPRO_House1RentCost;
    [SerializeField] private TextMeshProUGUI TMPRO_House2RentCost;
    [SerializeField] private TextMeshProUGUI TMPRO_House3RentCost;
    [SerializeField] private TextMeshProUGUI TMPRO_House4RentCost;
    [SerializeField] private TextMeshProUGUI TMPRO_HotelRentCost;
    [SerializeField] private TextMeshProUGUI TMPRO_MortgageValue;
    [SerializeField] private TextMeshProUGUI TMPRO_HousesPurchaseCost;
    [SerializeField] private TextMeshProUGUI TMPRO_HotelPurchaseCost;

    public void UpdateDisplay(TileData_Property tileData_Property)
    {
        imgPropertyColour.color = tileData_Property.PropertyColourSet.propertyColour;
        TMPRO_PropertyName.text = tileData_Property.Name;
        TMPRO_RentCost.text = $"RENT ${tileData_Property.RentDefaultCost}";

        TMPRO_House1RentCost.text = $"${tileData_Property.RentCostWithHouses[0]}";
        TMPRO_House2RentCost.text = $"${tileData_Property.RentCostWithHouses[1]}";
        TMPRO_House3RentCost.text = $"${tileData_Property.RentCostWithHouses[2]}";
        TMPRO_House4RentCost.text = $"${tileData_Property.RentCostWithHouses[3]}";

        TMPRO_HotelRentCost.text = $"${tileData_Property.RentCostWithHotel}";
        TMPRO_MortgageValue.text = $"Mortgage Value ${tileData_Property.MortgageValue}";
        TMPRO_HousesPurchaseCost.text = $"Houses cost ${tileData_Property.HousePurchaseCost} each";
        TMPRO_HotelPurchaseCost.text = $"Hotels, ${tileData_Property.HotelPurchaseCost} each plus 4 houses";
    }
}