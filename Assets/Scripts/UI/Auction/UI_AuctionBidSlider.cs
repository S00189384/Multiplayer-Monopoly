using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Needs to recieve player max money in account.
//Needs to react to players max money in account changing (locally).
// 

public class UI_AuctionBidSlider : MonoBehaviourPun
{
    [SerializeField] private Slider bidSlider;
    [SerializeField] private TextMeshProUGUI TMP_CurrentBidValue;
    [SerializeField] private TextMeshProUGUI TMP_MaxBidValue; //Players money balance. Can change if they choose to mortgage.

    public int PlayerBid { get { return (int)bidSlider.value; } }

    //Start.
    private void Awake() => BTN_AuctionBid.PlayerBiddedAtAuction += OnPlayerBiddedAtAuction;
    private void Start() => bidSlider.minValue = GameDataSlinger.MIN_AUCTION_BET;


    private void OnPlayerBiddedAtAuction(string playerID, int bidAmount)
    {
        photonView.RPC(nameof(OnPlayerBiddedAtAuctionRPC), RpcTarget.All, bidAmount);
    }

    [PunRPC]
    private void OnPlayerBiddedAtAuctionRPC(int bidAmount)
    {
        SetMinSliderValue(bidAmount + GameDataSlinger.MIN_AUCTION_BET_ADDITION);
        SetSliderValue(bidAmount + GameDataSlinger.MIN_AUCTION_BET_ADDITION);
    }

    public void SetMaxSliderValue(int value) => bidSlider.maxValue = value;
    public void SetMinSliderValue(int value) => bidSlider.minValue = value;
    public void SetSliderValue(int value) => bidSlider.value = value;

    public void OnBidSliderValueChanged(float newValue)
    {
        TMP_CurrentBidValue.text = $"${(int)newValue}";
    }

    private void OnDestroy()
    {
        BTN_AuctionBid.PlayerBiddedAtAuction -= OnPlayerBiddedAtAuction;
    }
}