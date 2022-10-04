using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//Manages the roll dice and finish turn buttons / UI.
//When it becomes the players turn this UI activates.
//When their turn ends it disables itself - player turn manager then assigns a new turn.


//Player is in red event:
//roll dice and finish turn buttons disable until player leaves red.


public class UI_ManageTurn : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private PlayerTurnManager playerTurnManager;
    [SerializeField] private GameObject buttonHolderParent;

    [SerializeField] private BTN_RollDice btnRollDice;
    [SerializeField] private BTN_Base btnFinishTurn;
    [SerializeField] private BTN_DeclareBankrupcy btnDeclareBankrupcy;

    [SerializeField] private GameObject GO_BankruptWarning;

    private bool IsMyTurn { get { return buttonHolderParent.activeSelf; } }
    private bool canRollDice;
    private bool canDeclareBankrupcy;

    private void Awake()
    {
        //Disable UI.
        buttonHolderParent.SetActive(false);

        //Subscribe to events.
        PhotonNetwork.AddCallbackTarget(this);
        GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;
        PlayerTurnManager.PlayerFinishedTurnEvent += OnPlayerFinishedTurn;
        PieceMover.PlayerMovedEvent += OnFinishedMove;
        Jailor.LocalPlayerJailedEvent += OnLocalPlayerJailed;
        Jailor.LocalPlayerLeftJailEvent += OnLocalPlayerLeftJail;
        Bank.PlayerDeclaredBankruptDueToBankPaymentEvent += OnPlayerDeclaredBankrupcy;
    }

    private void OnPlayerDeclaredBankrupcy(string playerID)
    {
        DisableUIElements();
    }

    private void OnAllPlayersSpawned()
    {
        PlayerMoneyAccount localPlayersMoneyAccount = GameManager.Instance.GetLocalPlayerPiece().GetComponent<PlayerMoneyAccount>();
        localPlayersMoneyAccount.EnteredBankruptsyEvent += OnPlayerEnteredBankrupcy;
        localPlayersMoneyAccount.LeftBankruptsyEvent += OnPlayerLeftBankrupcy;
    }

    private void OnPlayerUnsuccessfullyRolledADouble()
    {
        btnFinishTurn.SetButtonInteractable(true);
    }

    private void OnPlayerFinishedTurn()
    {
        DisableUIElements();
    }

    private void Start()
    {
        canRollDice = true;
    }

    private void OnPlayerEnteredBankrupcy(PlayerMoneyAccount playerMoneyAccount)
    {
        GO_BankruptWarning.SetActive(true);
        canDeclareBankrupcy = true;
        if (IsMyTurn)
            btnDeclareBankrupcy.SetButtonInteractable(canDeclareBankrupcy);
    }
    private void OnPlayerLeftBankrupcy(PlayerMoneyAccount playerMoneyAccount)
    {
        GO_BankruptWarning.SetActive(false);
        canDeclareBankrupcy = false;
        if (IsMyTurn)
            btnDeclareBankrupcy.SetButtonInteractable(canDeclareBankrupcy);
    }

    //When it becomes my turn - enable UI.
    private void OnStartOfMyTurn(string myPlayerID)
    {
        buttonHolderParent.SetActive(true);
        btnRollDice.SetButtonInteractable(canRollDice);
        btnDeclareBankrupcy.SetButtonInteractable(canDeclareBankrupcy);
    }

    //When I end my turn, tell turn manager and reset UI.
    public void OnFinishTurnButtonClicked()
    {
        playerTurnManager.EndPlayerTurn();
    }

    private void DisableUIElements()
    {
        btnFinishTurn.SetButtonInteractable(false);
        btnRollDice.SetButtonInteractable(false);
        btnDeclareBankrupcy.SetButtonInteractable(false);
        buttonHolderParent.SetActive(false);
    }

    //When my piece has finished moving.
    private void OnFinishedMove(PlayerMove playerMove)
    {
        if(playerMove.PlayerID == PhotonNetwork.LocalPlayer.UserId)
        {
            btnRollDice.SetButtonInteractable(false);
            btnFinishTurn.SetButtonInteractable(true);
        }
    }

    private void OnLocalPlayerJailed()
    {
        DisableUIElements();
        canRollDice = false;
        UI_DoubleDiceThrowPanel.UnsuccessfullyRolledADouble += OnPlayerUnsuccessfullyRolledADouble;
    }
    private void OnLocalPlayerLeftJail()
    {
        canRollDice = true;
        btnRollDice.SetButtonInteractable(false); //Left jail but dice roll is done automatically for them so disable.
        UI_DoubleDiceThrowPanel.UnsuccessfullyRolledADouble -= OnPlayerUnsuccessfullyRolledADouble;
    }

    //On new event call.
    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        //On new turn event.
        if (eventCode == EventCodes.NewTurnEventCode)
        {
            string newTurnID = (string)photonEvent.CustomData;
            if (newTurnID == PhotonNetwork.LocalPlayer.UserId)
                OnStartOfMyTurn(newTurnID);
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawned;
        PlayerTurnManager.PlayerFinishedTurnEvent -= OnPlayerFinishedTurn;
        PieceMover.PlayerMovedEvent -= OnFinishedMove;
        Jailor.LocalPlayerJailedEvent -= OnLocalPlayerJailed;
        Jailor.LocalPlayerLeftJailEvent -= OnLocalPlayerLeftJail;
        Bank.PlayerDeclaredBankruptDueToBankPaymentEvent -= OnPlayerDeclaredBankrupcy;
    }
}
