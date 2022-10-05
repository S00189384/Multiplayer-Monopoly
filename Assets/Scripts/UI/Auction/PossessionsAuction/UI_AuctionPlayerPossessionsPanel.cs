using TMPro;
using UnityEngine;

//Panel that is on screen during an auction for a players possessions. 
//Shows the players what tiles the player owns and the combined value for all of the tiles.

public class UI_AuctionPlayerPossessionsPanel : MonoBehaviour
{
    [Header("Components of panel")]
    [SerializeField] private Transform T_OwnedPropertiesSpawnHolder;
    [SerializeField] private Transform T_OwnedOtherItems;
    [SerializeField] private TextMeshProUGUI TMP_TotalValueOfPossessions;

    [Header("Prefabs")]
    [SerializeField] private UI_PropertyDisplay propertyDisplayPrefab; 
    [SerializeField] private UI_StationDisplay stationDisplayPrefab; 
    [SerializeField] private UI_UtilityDisplay utilityDisplayPrefab;
    [SerializeField] private GameObject getOutOfJailFreeCardDisplayPrefab;

    public void InitialisePanel(OwnedPlayerTileTracker playerTileTracker,PlayerInventory playerInventory)
    {
        int totalPossessionsValue = 0;

        if(playerTileTracker.OwnsAProperty)
        {
            for (int i = 0; i < playerTileTracker.ownedPropertiesList.Count; i++)
            {
                UI_PropertyDisplay spawnedPropertyDisplay = Instantiate(propertyDisplayPrefab, T_OwnedPropertiesSpawnHolder);
                spawnedPropertyDisplay.UpdateDisplay(playerTileTracker.ownedPropertiesList[i].propertyData);
                totalPossessionsValue += playerTileTracker.ownedPropertiesList[i].PurchaseCost;
            }
        }

        if(playerTileTracker.OwnsAStation)
        {
            for (int i = 0; i < playerTileTracker.ownedStationsList.Count; i++)
            {
                UI_StationDisplay spawnedStationDisplay = Instantiate(stationDisplayPrefab, T_OwnedOtherItems);
                spawnedStationDisplay.UpdateDisplay(playerTileTracker.ownedStationsList[i].tileDataStation);
                totalPossessionsValue += playerTileTracker.ownedStationsList[i].PurchaseCost;
            }
        }

        if(playerTileTracker.OwnsAUtility)
        {
            for (int i = 0; i < playerTileTracker.ownedUtilitiesList.Count; i++)
            {
                UI_UtilityDisplay spawnedUtilityDisplay = Instantiate(utilityDisplayPrefab, T_OwnedOtherItems);
                spawnedUtilityDisplay.UpdateDisplay(playerTileTracker.ownedUtilitiesList[i].tileDataUtility);
                totalPossessionsValue += playerTileTracker.ownedUtilitiesList[i].PurchaseCost;
            }
        }

        if(playerInventory.HasAGetOutOfJailFreeCard)
        {
            for (int i = 0; i < playerInventory.NumGetOutOfJailFreeCards; i++)
            {
                Instantiate(getOutOfJailFreeCardDisplayPrefab, T_OwnedOtherItems);
                totalPossessionsValue += GameDataSlinger.GET_OUT_OF_JAIL_FREE_CARD_SELL_VALUE;
            }
        }

        TMP_TotalValueOfPossessions.text = $"${totalPossessionsValue}";
    }
}