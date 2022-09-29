using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Simple script for main menu.

//Future improvements:
//Create / join lobby.
//Show available lobbies the player can join.
//Ready up system so host can only start if all the players are ready.
//Message to show the player feedback on joining / creating a room or searching / joining a lobby.


public class MainMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField IF_RoomName;
    [SerializeField] private Button BTN_PlayGame;
    [SerializeField] private Button BTN_LeaveRoom;
    [SerializeField] private TextMeshProUGUI TMP_PlayersInRoom;

    private bool CanStartGame { get { return PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= GameDataSlinger.NUM_MIN_PLAYERS; } }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 20;

        IF_RoomName.text = "g";

        IF_RoomName.characterLimit = GameDataSlinger.ROOM_NAME_CHARACTER_LIMIT;
    }

    public void CreateRoom()
    {
        if(!string.IsNullOrEmpty(IF_RoomName.text))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = GameDataSlinger.NUM_MAX_PLAYERS;
            roomOptions.PublishUserId = true;
            roomOptions.CleanupCacheOnLeave = false;

            PhotonNetwork.CreateRoom(IF_RoomName.text, roomOptions);
        }
    }

    public void ConnectToRoom()
    {
        PhotonNetwork.JoinRoom(IF_RoomName.text);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        newPlayer.NickName = $"Player {PhotonNetwork.CurrentRoom.PlayerCount}";
        UpdateListOfPlayersDisplay();

        if(CanStartGame)
            BTN_PlayGame.gameObject.SetActive(true);
        else
            BTN_PlayGame.gameObject.SetActive(false);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!CanStartGame)
            BTN_PlayGame.gameObject.SetActive(false);

        UpdateListOfPlayersDisplay();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(CanStartGame)
        {
            BTN_PlayGame.gameObject.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.NickName = $"Player {PhotonNetwork.CurrentRoom.PlayerCount}";

        BTN_LeaveRoom.gameObject.SetActive(true);

        UpdateListOfPlayersDisplay();
    }

    private void UpdateListOfPlayersDisplay()
    {
        Player[] players = PhotonNetwork.PlayerList;
        string text = string.Empty;

        for (int i = 0; i < players.Length; i++)
        {
            string line = string.Empty;
            line = $"{players[i].NickName}";

            if (players[i].UserId == PhotonNetwork.LocalPlayer.UserId)
                line += $" (Me)";

            if (players[i].IsMasterClient)
                line += $" (Host)";

            //text += $"{players[i].NickName} \n";
            text += line +  "\n";
        }

        TMP_PlayersInRoom.text = text;
    }

    private void ClearListOfPlayersDisplay()
    {
        TMP_PlayersInRoom.text = string.Empty;
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("Play Scene");
    }

    public void LeaveRoom()
    {
        if (CanStartGame)
            BTN_PlayGame.gameObject.SetActive(false);

        PhotonNetwork.LeaveRoom();
        ClearListOfPlayersDisplay();
        BTN_LeaveRoom.gameObject.SetActive(false);
    }
}
