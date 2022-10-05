using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Slider for setting the value of an auction bid.
//Max slider bet amount is the players current balance.
//Min slider bet amount is the current highest bid + the amount to add after a bid ($1).
//When a player bids, each players min slider amount is set to that bid.

public class UI_AuctionBidSlider : MonoBehaviourPun
{
    [Header("UI Components")]
    [SerializeField] private Slider bidSlider;
    [SerializeField] private TextMeshProUGUI TMP_CurrentBidValue;
    [SerializeField] private TextMeshProUGUI TMP_MaxBidValue; //Players money balance. Can change if they choose to mortgage.

    public int PlayerBid { get { return (int)bidSlider.value; } }

    private void Start()
    {
        if (GameManager.Instance.LocalPlayerIsAnActivePlayer)
        {
            BTN_AuctionBid.PlayerBiddedAtAuction += OnPlayerBiddedAtAuction;
            SetMinSliderValue(GameDataSlinger.MIN_AUCTION_BET);
            SetMaxSliderValue(Bank.Instance.GetLocalPlayerMoneyAccount.Balance);
            UpdateSliderValueText(GameDataSlinger.MIN_AUCTION_BET);
        }
        else
            gameObject.SetActive(false);
    }

    private void OnPlayerBiddedAtAuction(string playerID, int bidAmount)
    {
        if (bidAmount >= bidSlider.maxValue)
            bidSlider.gameObject.SetActive(false);

        photonView.RPC(nameof(OnPlayerBiddedAtAuctionRPC), RpcTarget.All, bidAmount);
    }

    [PunRPC]
    private void OnPlayerBiddedAtAuctionRPC(int bidAmount)
    {
        SetMinSliderValue(bidAmount + GameDataSlinger.MIN_AUCTION_BET_ADDITION);
        SetSliderValue(bidAmount + GameDataSlinger.MIN_AUCTION_BET_ADDITION);       
    }

    public void SetMaxSliderValue(int value)
    {
        bidSlider.maxValue = value;
        TMP_MaxBidValue.text = $"${(int)value}";
    }

    public void SetMinSliderValue(int value) => bidSlider.minValue = value;
    public void SetSliderValue(int value)
    {
        bidSlider.value = value;
        UpdateSliderValueText(value);
    }

    public void UpdateSliderValueText(float newValue)
    {
        TMP_CurrentBidValue.text = $"${(int)newValue}";
    }

    private void OnDestroy()
    {
        BTN_AuctionBid.PlayerBiddedAtAuction -= OnPlayerBiddedAtAuction;
    }
}