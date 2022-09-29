using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_GameOverPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TMP_PlayerThatWon;
    [SerializeField] private Button BTN_Quit;
    [SerializeField] private Button BTN_ReturnToMainMenu;

    private void Awake()
    {
        BTN_Quit.onClick.AddListener(OnQuitButtonClicked);
        BTN_ReturnToMainMenu.onClick.AddListener(OnReturnToMainMenuButtonClicked);
    }

    public void UpdateDisplay(string playerIDThatWon)
    {
       TMP_PlayerThatWon.text = $"{GameManager.Instance.GetPlayerNicknameFromID(playerIDThatWon)} won the game!";
    }

    private void OnQuitButtonClicked()
    {      
        Application.Quit(0);
    }

    private void OnReturnToMainMenuButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(1);
    }
}
