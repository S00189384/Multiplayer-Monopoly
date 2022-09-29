using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerDisplay : MonoBehaviourPun
{
    [SerializeField] private Outline displayOutline;
    private Color defaultOutlineColour;

    [SerializeField] private TextMeshProUGUI TMP_PlayerName;
    [SerializeField] private TextMeshProUGUI TMP_PlayerBalance;
    [SerializeField] private Image IMG_PlayerIcon;

    [SerializeField] private GameObject playerNoLongerPartOfGameDisplay;
    [SerializeField] private TextMeshProUGUI TMP_PlayerNameOutOfGame;
    [SerializeField] private TextMeshProUGUI TMP_PlayerNameOutOfGameDescription;

    private void Awake()
    {
        defaultOutlineColour = displayOutline.effectColor;
    }

    public void UpdateDisplay(string playerName,string playerBalance,Sprite playerSprite)
    {
        TMP_PlayerName.text = playerName;
        TMP_PlayerBalance.text = playerBalance;
        IMG_PlayerIcon.sprite = playerSprite;

        TMP_PlayerNameOutOfGame.text = playerName;
    }

    public void ChangeBalance(PlayerMoneyAccount account)
    {
        TMP_PlayerBalance.text = $"${account.Balance}";
    }


    public void ChangeBalance(int newBalance)
    {
        TMP_PlayerBalance.text = $"${newBalance}";
    }

    [PunRPC]
    public void ChangeBalanceRemote(int newBalance)
    {
        TMP_PlayerBalance.text = $"${newBalance}";
    }

    public void ChangeOutlineColour(Color colour)
    {
        displayOutline.effectColor = colour;
    }

    public void ChangeDisplayToDisconnected()
    {
        playerNoLongerPartOfGameDisplay.SetActive(true);
        TMP_PlayerNameOutOfGameDescription.text = "Disconnected";
    }

    public void ChangeDisplayToBankrupt(RpcTarget rpcTarget)
    {
        photonView.RPC(nameof(ChangeDisplayToBankruptRPC), rpcTarget);
    }

    [PunRPC]
    private void ChangeDisplayToBankruptRPC()
    {
        playerNoLongerPartOfGameDisplay.SetActive(true);
        TMP_PlayerNameOutOfGameDescription.text = "Bankrupt";
    }
}
