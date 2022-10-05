using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//UI for constructing a building on a property.


public class UI_PropertyConstructBuildingSelection : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private CanvasGroup CG_Background;
    [SerializeField] private TextMeshProUGUI TMP_Header;
    [SerializeField] private TextMeshProUGUI TMP_ConstructInfo;
    [SerializeField] private TextMeshProUGUI TMP_BalanceChange;
    [SerializeField] private Button BTN_Return;

    [Header("UI Fading Settings")]
    [SerializeField] private float timeToFadeUI;
    [SerializeField] private LeanTweenType fadeTweenType;

    [Header("Raycasting")]
    private Ray ray;
    private RaycastHit hitInfo;
    [SerializeField] private TileInstance_Property propertyTile;

    private bool IsActive = false;
    private bool UpdatedPropertyConstructionInfoUIText = false;
    private bool MouseIsOverTileThatCanBuildOn = false;

    private PlayerMoneyAccount moneyAccountOfLocalPlayer;

    public List<PropertyBuildingSetTracker> propertyBuildingSetTrackers = new List<PropertyBuildingSetTracker>();

    private void Awake()
    {
        GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;
    }

    private void OnAllPlayersSpawned()
    {
        GameManager.Instance.GetLocalPlayerPiece().GetComponent<OwnedPlayerTileTracker>().OwnsAllOfPropertyTypeEvent += OnLocalPlayerNowOwnsAllPropertiesBelongingToSet;
    }

    private void OnLocalPlayerNowOwnsAllPropertiesBelongingToSet(PropertyColourSet colourSet, List<TileInstance_Property> propertyList)
    {
        PropertyBuildingSetTracker propertyBuildingSetTracker = new PropertyBuildingSetTracker(colourSet, propertyList);
        propertyBuildingSetTrackers.Add(propertyBuildingSetTracker);
    }

    private void Update()
    {
        if (!IsActive)
            return;

        CheckForMouseOverTile();
    }

    public void Initialise()
    {
        ReceiveTilesThatPlayerCanConstructPropertyOn();
        moneyAccountOfLocalPlayer = Bank.Instance.GetLocalPlayerMoneyAccount;

        FadeInUI();
        mainCanvas.sortingLayerName = CustomLayerMasks.canvasConstructBuildingSortingLayerName;
    }

    private void ReceiveTilesThatPlayerCanConstructPropertyOn()
    {
        if(propertyBuildingSetTrackers.Count <= 0)
        {    
            return;
        }

        for (int i = 0; i < propertyBuildingSetTrackers.Count; i++)
        {
            PropertyBuildingSetTracker propertyBuildingSet = propertyBuildingSetTrackers[i];

            //Check if any prop is mortgaged for this set.
            if(propertyBuildingSet.AnyPropertyIsMortgaged)
            {
                continue;
            }

            CalculateIfPropertiesOfAColourTypeCanBeBuiltOn(propertyBuildingSet);
        }    
    }

    private void CheckForMouseOverTile()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer(CustomLayerMasks.canConstructBuildingLayerName)))
        {
            MouseIsOverTileThatCanBuildOn = true;

            //Get tile instance once.
            if (propertyTile == null)
                hitInfo.collider.gameObject.TryGetComponent(out propertyTile);

            if (propertyTile)
            {
                //Update info about tile instance once.
                if (!UpdatedPropertyConstructionInfoUIText)
                    UpdateConstructInfoText();

                CheckIfPlayerClickedConstructBuilding();
            }
        }
        else
        {
            if (MouseIsOverTileThatCanBuildOn)
            {
                MouseIsOverTileThatCanBuildOn = false;
                propertyTile = null;
                ResetBuildInfoText();
            }
        }
    }

    private void CheckIfPlayerClickedConstructBuilding()
    {
        //Check for click to construct building.
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (propertyTile.OwnerID == PhotonNetwork.LocalPlayer.UserId)
            {
                propertyTile.ConstructBuilding();
                OnSelectingToConstructOnProperty();

                UpdateConstructInfoText();
            }
        }
    }

    private void OnSelectingToConstructOnProperty()
    {
        PropertyBuildingSetTracker propertyBuildingSetTracker = propertyBuildingSetTrackers.Find(prop => prop.propertyColour == propertyTile.propertyData.PropertyColourSet);
        CalculateIfPropertiesOfAColourTypeCanBeBuiltOn(propertyBuildingSetTracker);

        //Go through each property belonging to set that was clicked on.
        //Recalculate can build for each one.
    }

    private void CalculateIfPropertiesOfAColourTypeCanBeBuiltOn(PropertyBuildingSetTracker propertyBuildingSetTracker)
    {
        for (int i = 0; i < propertyBuildingSetTracker.propertyList.Count; i++)
        {
            TileInstance_Property propertyInstance = propertyBuildingSetTracker.propertyList[i];
            if (propertyBuildingSetTracker.CanConstructOnProperty(propertyInstance))
            {
                propertyInstance.GetComponent<TileDisplay>().ChangeSortingLayerName(CustomLayerMasks.canConstructBuildingLayerName);
                propertyInstance.gameObject.layer = LayerMask.NameToLayer(CustomLayerMasks.canConstructBuildingLayerName);
            }
            else
            {
                propertyInstance.GetComponent<TileDisplay>().ChangeSortingLayerName(CustomLayerMasks.ownedLayerMaskName);
                propertyInstance.gameObject.layer = LayerMask.NameToLayer(CustomLayerMasks.ownedLayerMaskName);
            }
        }
    }

    private void UpdateConstructInfoText()
    {
        int buildHouseCost, playerBalanceAfterBuilding, changeInBalance;

        if (propertyTile.CanBuildHouse)
        {
            TMP_ConstructInfo.text = $"Build a house on {propertyTile.propertyData.Name}";

            buildHouseCost = propertyTile.propertyData.HousePurchaseCost;
            playerBalanceAfterBuilding = moneyAccountOfLocalPlayer.GetBalanceWithPurchase(buildHouseCost);
            changeInBalance = moneyAccountOfLocalPlayer.Balance - playerBalanceAfterBuilding;

            TMP_BalanceChange.text = $"Balance change: ${moneyAccountOfLocalPlayer.Balance} to ${playerBalanceAfterBuilding} (-${changeInBalance})";
        }
        else if(propertyTile.CanBuildHotel)
        {
            TMP_ConstructInfo.text = $"Build a hotel on {propertyTile.propertyData.Name}";

            buildHouseCost = propertyTile.propertyData.HousePurchaseCost;
            playerBalanceAfterBuilding = moneyAccountOfLocalPlayer.GetBalanceWithPurchase(buildHouseCost);
            changeInBalance = moneyAccountOfLocalPlayer.Balance - playerBalanceAfterBuilding;

            TMP_BalanceChange.text = $"Balance change: ${moneyAccountOfLocalPlayer.Balance} to ${playerBalanceAfterBuilding} (-${changeInBalance})";
        }
    }

    private void ResetBuildInfoText()
    {
        TMP_ConstructInfo.text = string.Empty;
        TMP_BalanceChange.text = string.Empty;
        UpdatedPropertyConstructionInfoUIText = false;
    }
    public void OnReturnButtonClicked()
    {
        FadeOutUI();
    }

    private void ResetLayersOfTiles()
    {
        for (int i = 0; i < propertyBuildingSetTrackers.Count; i++)
        {
            propertyBuildingSetTrackers[i].propertyList.ForEach(prop =>
            {
                if(prop.NumConstructedBuildings <= 0)
                {
                    //Can mortgage it.
                    prop.GetComponent<TileDisplay>().ChangeSortingLayerName(CustomLayerMasks.mortgageableLayerName);
                    prop.gameObject.layer = LayerMask.NameToLayer(CustomLayerMasks.mortgageableLayerName);
                }
                else
                {
                    prop.GetComponent<TileDisplay>().ChangeSortingLayerName(CustomLayerMasks.unmortgageableLayerName);
                    prop.gameObject.layer = LayerMask.NameToLayer(CustomLayerMasks.unmortgageableLayerName);
                }
            });
        }
    }

    private void FadeInUI()
    {
        CG_Background.gameObject.SetActive(true);
        LeanTween.alphaCanvas(CG_Background, 1f, timeToFadeUI).setEase(fadeTweenType).setOnComplete(() => IsActive = true);
    }
    private void FadeOutUI()
    {
        IsActive = false;
        LeanTween.alphaCanvas(CG_Background, 0, timeToFadeUI).setEase(fadeTweenType)
            .setOnComplete(() => 
            { 
                CG_Background.gameObject.SetActive(false);
                ResetLayersOfTiles();
            });
    }

    private void OnDisable()
    {
        mainCanvas.sortingLayerName = CustomLayerMasks.canvasDefaultSortingLayerName;
    }
    private void OnDestroy()
    {
        //This could be null / throw an error? If player is destroyed.
        GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawned;
        TileOwnershipManager.Instance.GetLocalPlayersOwnedTileTracker.OwnsAllOfPropertyTypeEvent -= OnLocalPlayerNowOwnsAllPropertiesBelongingToSet;
    }
}

//Created when player owns all of a property colour set.
//Allows UI to keep track of whether the player can mortgage or sell / construct buildings on property.
[Serializable]
public class PropertyBuildingSetTracker
{
    public PropertyColourSet propertyColour;
    public List<TileInstance_Property> propertyList;

    //The number of the tile with the highest number of constructed buildings.
    public int MaxNumConstructedBuildingsOfAnyProperty;
    public int NumMortgagedProperties;
    public bool AnyPropertyIsMortgaged { get { return NumMortgagedProperties > 0; } }
    public bool AllPropertiesHaveSameNumberOfConstructedBuildings { get { return propertyList.TrueForAll(prop => prop.NumConstructedBuildings == MaxNumConstructedBuildingsOfAnyProperty); } }

    public PropertyBuildingSetTracker(PropertyColourSet propertyColour, List<TileInstance_Property> propertyList)
    {
        this.propertyColour = propertyColour;
        this.propertyList = propertyList;

        NumMortgagedProperties = propertyList.Count(prop => prop.IsMortgaged);
        MaxNumConstructedBuildingsOfAnyProperty = propertyList.Max(prop => prop.NumConstructedBuildings);

        propertyList.ForEach(prop => 
        { 
            prop.BuiltHouseEvent += OnPropertyBuiltHouse;
            prop.BuiltHotelEvent += OnPropertyBuiltHotel;
            prop.SoldHouseEvent += OnPropertySoldHouse;
            prop.SoldHotelEvent += OnPropertySoldHotel;
            prop.MortgagedEvent += OnPropertyMortgaged;
            prop.UnmortgagedEvent += OnPropertyUnmortgaged;
        });
    }

    private void OnPropertySoldHotel(TileInstance_Property property)
    {
        if (propertyList.TrueForAll(prop => prop.NumConstructedBuildings == property.NumConstructedBuildings))
        {
            MaxNumConstructedBuildingsOfAnyProperty = property.NumConstructedBuildings;
        }
    }

    private void OnPropertySoldHouse(TileInstance_Property property)
    {
        if (propertyList.TrueForAll(prop => prop.NumConstructedBuildings == property.NumConstructedBuildings))
        {
            MaxNumConstructedBuildingsOfAnyProperty = property.NumConstructedBuildings;
        }
    }

    public bool CanConstructOnProperty(TileInstance_Property propertyInstance)
    {
        // Can construct a house or hotel on property if hotel isn't built
        // Num buildings on property is 0 or all properties have the same number of constructed buildings.
        return (propertyInstance.NumConstructedBuildings <= 0 || propertyInstance.NumConstructedBuildings < MaxNumConstructedBuildingsOfAnyProperty || AllPropertiesHaveSameNumberOfConstructedBuildings) && !propertyInstance.HotelBuilt && !AnyPropertyIsMortgaged;
    }

    public bool CanSellBuildingOnProperty(TileInstance_Property propertyInstance)
    {
        return propertyInstance.NumConstructedBuildings > 0 && propertyInstance.NumConstructedBuildings == MaxNumConstructedBuildingsOfAnyProperty;
    }

    private void OnPropertyMortgaged(TileInstance_Purchasable tileInstance_Purchasable)
    {
        NumMortgagedProperties++;
    }
    private void OnPropertyUnmortgaged(TileInstance_Purchasable tileInstance_Purchasable)
    {
        NumMortgagedProperties--;
    }
    private void OnPropertyBuiltHouse(TileInstance_Property property)
    {
        MaxNumConstructedBuildingsOfAnyProperty = property.NumConstructedBuildings;
    }
    private void OnPropertyBuiltHotel(TileInstance_Property property)
    {
        MaxNumConstructedBuildingsOfAnyProperty = property.NumConstructedBuildings;
    }
}