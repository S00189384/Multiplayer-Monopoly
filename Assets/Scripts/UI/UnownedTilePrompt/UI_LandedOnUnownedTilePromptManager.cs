using UnityEngine;

//UI manager which spawns prompts for player action when they land on an unowned tile.

public class UI_LandedOnUnownedTilePromptManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private UI_LandedOnUnownedTilePrompt landedOnUnownedTilePromptPrefab;
    [SerializeField] private UI_LandedOnUnownedPropertyPrompt landedOnUnownedPropertyPromptPrefab;
    [SerializeField] private UI_LandedOnUnownedStationPrompt landedOnUnownedStationPromptPrefab;
    [SerializeField] private UI_LandedOnUnownedUtilityPrompt landedOnUnownedUtiltyPromptPrefab;

    [SerializeField] private Transform T_promptSpawn;
    [SerializeField] private CanvasGroup CG_Background;

    [Header("Movement On/Off Screen Settings")]
    [SerializeField] private float timeToMoveToPosition;
    [SerializeField] private LeanTweenType tweenType;

    private UI_LandedOnUnownedTilePrompt spawnedLandedOnTilePropmt;

    private void Awake()
    {
        TileInstance_Property.PlayerLandedOnUnownedPropertyEvent += OnPlayerLandedOnUnownedProperty;
        TileInstance_Station.PlayerLandedOnUnownedStationEvent += OnPlayerLandedOnUnownedStation;
        TileInstance_Utility.PlayerLandedOnUnownedUtilityEvent += OnPlayerLandedOnUnknownUtility;
    }

    private void OnPlayerLandedOnUnownedProperty(string playerID, TileInstance_Property propertyInstanceLandedOn)
    {
        UI_LandedOnUnownedPropertyPrompt spawnedUnownedPropertyPrompt = Instantiate(landedOnUnownedPropertyPromptPrefab,T_promptSpawn.position,Quaternion.identity, this.transform);
        spawnedUnownedPropertyPrompt.InitialiseDisplay(playerID, propertyInstanceLandedOn);
        spawnedLandedOnTilePropmt = spawnedUnownedPropertyPrompt;

        FixDisplayOfPrompt();
        SetUpButtonsOnPrompt();
        MovePromptOnScreen();
    }

    private void OnPurchaseTileButtonClickedOnPrompt()
    {
        LeanTween.alphaCanvas(CG_Background, 0f, timeToMoveToPosition);
        LeanTween.move(spawnedLandedOnTilePropmt.gameObject, T_promptSpawn.position, timeToMoveToPosition).setEase(tweenType)
            .setOnComplete(() =>
            {
                spawnedLandedOnTilePropmt.PurchaseMyTile();
                CG_Background.gameObject.SetActive(false);
                Destroy(spawnedLandedOnTilePropmt.gameObject);
            });
    }
    private void OnStartAuctionButtonClickedOnPrompt()
    {
        LeanTween.alphaCanvas(CG_Background, 0f, timeToMoveToPosition);

        LeanTween.alphaCanvas(CG_Background, 0f, timeToMoveToPosition);
        LeanTween.move(spawnedLandedOnTilePropmt.gameObject, T_promptSpawn.position, timeToMoveToPosition).setEase(tweenType)
            .setOnComplete(() =>
            {
                spawnedLandedOnTilePropmt.OnStartAuctionButtonPressed();
                CG_Background.gameObject.SetActive(false);
                Destroy(spawnedLandedOnTilePropmt.gameObject);
            });
    }


    private void OnPlayerLandedOnUnownedStation(string playerID, TileInstance_Station stationInstanceLandedOn)
    {
        UI_LandedOnUnownedStationPrompt spawnedUnownedStationPrompt = Instantiate(landedOnUnownedStationPromptPrefab, T_promptSpawn.position, Quaternion.identity, this.transform);
        spawnedUnownedStationPrompt.InitialiseDisplay(playerID, stationInstanceLandedOn);
        spawnedLandedOnTilePropmt = spawnedUnownedStationPrompt;

        FixDisplayOfPrompt();
        SetUpButtonsOnPrompt();
        MovePromptOnScreen();
    }
    private void OnPlayerLandedOnUnknownUtility(string playerID, TileInstance_Utility utilityInstanceLandedOn)
    {
        UI_LandedOnUnownedUtilityPrompt spawnedUnownedUtilityPrompt = Instantiate(landedOnUnownedUtiltyPromptPrefab, T_promptSpawn.position, Quaternion.identity, this.transform);
        spawnedUnownedUtilityPrompt.InitialiseDisplay(playerID, utilityInstanceLandedOn);
        spawnedLandedOnTilePropmt = spawnedUnownedUtilityPrompt;

        FixDisplayOfPrompt();
        SetUpButtonsOnPrompt();
        MovePromptOnScreen();
    }

    private void FixDisplayOfPrompt()
    {
        spawnedLandedOnTilePropmt.transform.localEulerAngles = Vector3.zero;
    }
    private void SetUpButtonsOnPrompt()
    {
        spawnedLandedOnTilePropmt.AddOnClickListenerToPurchaseTileButton(OnPurchaseTileButtonClickedOnPrompt);
        spawnedLandedOnTilePropmt.AddOnClickListenerToStartAuctionButton(OnStartAuctionButtonClickedOnPrompt);
    }
    private void MovePromptOnScreen()
    {
        CG_Background.gameObject.SetActive(true);
        LeanTween.alphaCanvas(CG_Background, 1f, timeToMoveToPosition);
        LeanTween.move(spawnedLandedOnTilePropmt.gameObject, transform.position, timeToMoveToPosition).setEase(tweenType);
    }

    private void OnDestroy()
    {
        TileInstance_Property.PlayerLandedOnUnownedPropertyEvent -= OnPlayerLandedOnUnownedProperty;
        TileInstance_Station.PlayerLandedOnUnownedStationEvent -= OnPlayerLandedOnUnownedStation;
        TileInstance_Utility.PlayerLandedOnUnownedUtilityEvent -= OnPlayerLandedOnUnknownUtility;
    }
}