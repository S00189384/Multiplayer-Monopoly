using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PropertySellBuildingSelection : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private CanvasGroup CG_Background;
    [SerializeField] private TextMeshProUGUI TMP_Header;
    [SerializeField] private TextMeshProUGUI TMP_SellInfo;
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
    private bool UpdatedPropertySellInfoUIText = false;
    private bool MouseIsOverTileThatCanSellBuilding = false;

    private PlayerMoneyAccount moneyAccountOfLocalPlayer;

    public List<PropertyBuildingSetTracker> propertyBuildingSetTrackers = new List<PropertyBuildingSetTracker>();

    private void Awake()
    {
        GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;
    }

    private void OnAllPlayersSpawned()
    {
        GameManager.Instance.GetLocalPlayerPiece().GetComponent<OwnedPlayerTileTracker>().OwnsAllOfPropertyTypeEvent += OnLocalPlayerNowOwnsAllPropertiesBelongingToSet;
        //TileOwnershipManager.Instance.GetLocalPlayersOwnedTileTracker.OwnsAllOfPropertyTypeEvent += OnLocalPlayerNowOwnsAllPropertiesBelongingToSet;
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
        ReceiveTilesThatPlayerCanSellBuildingsOn();
        moneyAccountOfLocalPlayer = Bank.Instance.GetLocalPlayerMoneyAccount;

        FadeInUI();
        mainCanvas.sortingLayerName = "CanvasDuringPropertyBuildingSelling";
    }

    private void ReceiveTilesThatPlayerCanSellBuildingsOn()
    {
        if (propertyBuildingSetTrackers.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < propertyBuildingSetTrackers.Count; i++)
        {
            PropertyBuildingSetTracker propertyBuildingSet = propertyBuildingSetTrackers[i];

            ////Check if any prop is mortgaged for this set.
            //if (propertyBuildingSet.AnyPropertyIsMortgaged)
            //{
            //    print("At least one property is mortgaged. Ignoring this colour set");
            //    continue;
            //}

            CalculateIfCanSellBuildingsOnPropertiesOfAColourSet(propertyBuildingSet);
        }
    }

    private void CheckForMouseOverTile()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("CanSellBuilding")))
        {
            MouseIsOverTileThatCanSellBuilding = true;

            //Get tile instance once.
            if (propertyTile == null)
                hitInfo.collider.gameObject.TryGetComponent(out propertyTile);

            if (propertyTile)
            {
                //Update info about tile instance once.
                if (!UpdatedPropertySellInfoUIText)
                    UpdateSellInfoText();

                CheckIfPlayerClickedSellBuilding();
            }
        }
        else
        {
            if (MouseIsOverTileThatCanSellBuilding)
            {
                MouseIsOverTileThatCanSellBuilding = false;
                propertyTile = null;
                ResetSellInfoText();
            }
        }
    }

    private void CheckIfPlayerClickedSellBuilding()
    {
        //Check for click to sell building.
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (propertyTile.OwnerID == PhotonNetwork.LocalPlayer.UserId)
            {
                //Sell building.
                propertyTile.SellBuilding();
                OnSelectingToSellBuildingOnProperty();

                UpdateSellInfoText();
            }
        }
    }

    private void OnSelectingToSellBuildingOnProperty()
    {
        PropertyBuildingSetTracker propertyBuildingSetTracker = propertyBuildingSetTrackers.Find(prop => prop.propertyColour == propertyTile.propertyData.PropertyColourSet);
        CalculateIfCanSellBuildingsOnPropertiesOfAColourSet(propertyBuildingSetTracker);

        //Go through each property belonging to set that was clicked on.
        //Recalculate can build for each one.
    }

    private void CalculateIfCanSellBuildingsOnPropertiesOfAColourSet(PropertyBuildingSetTracker propertyBuildingSetTracker)
    {
        for (int i = 0; i < propertyBuildingSetTracker.propertyList.Count; i++)
        {
            TileInstance_Property propertyInstance = propertyBuildingSetTracker.propertyList[i];
            if (propertyBuildingSetTracker.CanSellBuildingOnProperty(propertyInstance))
            {
                propertyInstance.GetComponent<TileDisplay>().ChangeSortingLayerName("CanSellBuilding");
                propertyInstance.gameObject.layer = LayerMask.NameToLayer("CanSellBuilding");
            }
            else
            {
                propertyInstance.GetComponent<TileDisplay>().ChangeSortingLayerName("Owned");
                propertyInstance.gameObject.layer = LayerMask.NameToLayer("Owned");
            }
        }
    }

    private void UpdateSellInfoText()
    {
        int sellBuildingCost, playerBalanceAfterSelling, changeInBalance;

        if (propertyTile.HotelBuilt)
        {
            TMP_SellInfo.text = $"Sell a hotel on {propertyTile.propertyData.Name}";

            sellBuildingCost = propertyTile.propertyData.HotelSellValue;
            playerBalanceAfterSelling = moneyAccountOfLocalPlayer.GetBalanceWithPurchase(sellBuildingCost);
            changeInBalance = moneyAccountOfLocalPlayer.Balance - playerBalanceAfterSelling;

            TMP_BalanceChange.text = $"Balance change: ${moneyAccountOfLocalPlayer.Balance} to ${playerBalanceAfterSelling} (+${changeInBalance})";
        }
        else if (propertyTile.HouseBuilt)
        {
            TMP_SellInfo.text = $"Sell a house on {propertyTile.propertyData.Name}";

            sellBuildingCost = propertyTile.propertyData.HouseSellValue;
            playerBalanceAfterSelling = moneyAccountOfLocalPlayer.GetBalanceWithPurchase(sellBuildingCost);
            changeInBalance = moneyAccountOfLocalPlayer.Balance - playerBalanceAfterSelling;

            TMP_BalanceChange.text = $"Balance change: ${moneyAccountOfLocalPlayer.Balance} to ${playerBalanceAfterSelling} (+${changeInBalance})";
        }
    }

    private void ResetSellInfoText()
    {
        TMP_SellInfo.text = string.Empty;
        TMP_BalanceChange.text = string.Empty;
        UpdatedPropertySellInfoUIText = false;
        //TMP_BankruptWarning.gameObject.SetActive(false);
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
        mainCanvas.sortingLayerName = "Canvas";
    }
    private void OnDestroy()
    {
        //This could be null / throw an error? If player is destroyed.
        GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawned;
        TileOwnershipManager.Instance.GetLocalPlayersOwnedTileTracker.OwnsAllOfPropertyTypeEvent -= OnLocalPlayerNowOwnsAllPropertiesBelongingToSet;
    }
}