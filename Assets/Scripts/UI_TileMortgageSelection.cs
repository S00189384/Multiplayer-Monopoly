using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Activates when player clicks on button to mortgage / unmortgage a tile.
//If player hovers over a tile they can mortgage / unmortgage the UI shows them how much money they will gain / lose.
//Clicking on a tile makes the tile become mortgaged or unmortgaged.
//Player can't unmortgage a tile if they would go bankrupt with unmortgaging it or are already in the red.

public class UI_TileMortgageSelection : MonoBehaviour
{
    private const string canvasSortingLayerNameDuringSelection = "CanvasDuringMortgageSelection";
    private const string defaultcanvasSortingLayerName = "Canvas";

    [Header("Components")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private CanvasGroup CG_Background;
    [SerializeField] private TextMeshProUGUI TMP_MortgageInfo;
    [SerializeField] private TextMeshProUGUI TMP_BalanceChange;
    [SerializeField] private TextMeshProUGUI TMP_BankruptWarning;
    [SerializeField] private Button BTN_Return;

    [Header("UI Fading Settings")]
    [SerializeField] private float timeToFadeUI;
    [SerializeField] private LeanTweenType fadeTweenType;

    [Header("Raycasting")]
    private Ray ray;
    private RaycastHit hitInfo;
    private TileInstance_Purchasable purchasableTile;

    private bool UpdatedMortgageInfoUIText = false;
    private bool MouseIsOverOwnedTile = false;

    private PlayerMoneyAccount moneyAccountOfLocalPlayer;

    private void Update()
    {
        CheckForMouseOverTile();
    }

    private void CheckForMouseOverTile()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer(CustomLayerMasks.mortgageableLayerName)))
        {
            MouseIsOverOwnedTile = true;

            //Get tile instance once.
            if (purchasableTile == null)
                hitInfo.collider.gameObject.TryGetComponent(out purchasableTile);

            if (purchasableTile)
            {
                //Update info about tile instance once.
                if (!UpdatedMortgageInfoUIText)
                    UpdateMortgageText();

                CheckIfPlayerClickedToMortgageTile();
            }
        }
        else
        {
            if (MouseIsOverOwnedTile)
            {
                MouseIsOverOwnedTile = false;
                purchasableTile = null;
                ResetMortgageText();
            }
        }
    }

    private void CheckIfPlayerClickedToMortgageTile()
    {
        //Check for click to mortgage / unmortgage.
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (purchasableTile.OwnerID == PhotonNetwork.LocalPlayer.UserId)
            {
                if (purchasableTile.IsMortgaged && moneyAccountOfLocalPlayer.CanAffordPurchase(purchasableTile.GetPurchasableData.UnmortgageCost))
                {
                    purchasableTile.UnmortgageTile();
                    UpdateMortgageText();
                }
                else if(!purchasableTile.IsMortgaged)
                {
                    purchasableTile.MortgageTile();
                    UpdateMortgageText();
                }
            }
        }
    }

    private void UpdateMortgageText()
    {
        int changeInBalance, playerBalanceAfterSelection;

        if (purchasableTile.IsMortgaged) //Unmortgage info.
        {
            int unmortgageCost = purchasableTile.GetPurchasableData.UnmortgageCost;
            playerBalanceAfterSelection = moneyAccountOfLocalPlayer.GetBalanceWithPurchase(purchasableTile.GetPurchasableData.UnmortgageCost);
            changeInBalance = moneyAccountOfLocalPlayer.Balance - playerBalanceAfterSelection;

            TMP_MortgageInfo.text = $"Unmortgage {purchasableTile.GetPurchasableData.Name}";
            TMP_BalanceChange.text = $"Balance change: ${moneyAccountOfLocalPlayer.Balance} to ${playerBalanceAfterSelection} (-${changeInBalance})";
            TMP_BankruptWarning.gameObject.SetActive(!moneyAccountOfLocalPlayer.CanAffordPurchase(unmortgageCost));
        }
        else //Mortgage info.
        {
            playerBalanceAfterSelection = moneyAccountOfLocalPlayer.GetBalanceWithMoneyGain(purchasableTile.GetPurchasableData.MortgageValue);
            changeInBalance = playerBalanceAfterSelection - moneyAccountOfLocalPlayer.Balance;

            TMP_BankruptWarning.gameObject.SetActive(false);
            TMP_MortgageInfo.text = $"Mortgage {purchasableTile.GetPurchasableData.Name}";
            TMP_BalanceChange.text = $"Balance change: ${moneyAccountOfLocalPlayer.Balance} to ${playerBalanceAfterSelection} (+${changeInBalance})"; ;
        }

        UpdatedMortgageInfoUIText = true;
    }

    private void ResetMortgageText()
    {
        TMP_MortgageInfo.text = string.Empty;
        TMP_BalanceChange.text = string.Empty;
        TMP_BankruptWarning.gameObject.SetActive(false);
        UpdatedMortgageInfoUIText = false;
    }
    public void OnReturnButtonClicked()
    {
        FadeOutUI();
    }

    private void OnEnable()
    {
        FadeInUI();
        moneyAccountOfLocalPlayer = Bank.Instance.GetLocalPlayerMoneyAccount;
        mainCanvas.sortingLayerName = canvasSortingLayerNameDuringSelection;
    }

    private void OnDisable()
    {
        mainCanvas.sortingLayerName = defaultcanvasSortingLayerName;
    }

    private void FadeInUI()
    {
        LeanTween.alphaCanvas(CG_Background, 1f, timeToFadeUI).setEase(fadeTweenType);
    }
    private void FadeOutUI()
    {
        LeanTween.alphaCanvas(CG_Background, 0, timeToFadeUI).setEase(fadeTweenType).setOnComplete(() => gameObject.SetActive(false));
    }
}
