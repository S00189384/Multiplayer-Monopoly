using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Prompt that appears on screen when the game is over. Spawned in and moved on screen by UI_GameOver object.
//Informs the player on who won and contains buttons to send the player back to the main menu or to quit the game.
//Future improvements would be to also show a run down of what happened to each player during the game like a leaderboard.
//Could maybe show the time they declared bankrupcy / disconnected, what caused them to enter bankrupcy etc. 
//Also could have an option for play again with the same people in the game with a voting system.

public class UI_GameOverPrompt : MonoBehaviour
{
    [Header("UI Components")]
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
        Time.timeScale = 1;
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(1);
    }
}
