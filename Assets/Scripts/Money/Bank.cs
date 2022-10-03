using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

//Bank is responsible for holding players money accounts.
//Bank processes payments between players and between players and the bank.
//Bank also can process a tile purchase.
//When a tile is purchased, the bank notifies tileownershipmanager to also process the tile purchase.

public class Bank : MonoBehaviourPun
{
    public static Bank Instance;

    //Player account management.
    private Dictionary<string, PlayerMoneyAccount> playerMoneyAccountDictionary = new Dictionary<string, PlayerMoneyAccount>();
    public PlayerMoneyAccount GetPlayerMoneyAccountByID(string playerID) => playerMoneyAccountDictionary[playerID];
    public PlayerMoneyAccount GetLocalPlayerMoneyAccount => playerMoneyAccountDictionary[PhotonNetwork.LocalPlayer.UserId];

    //Events.
    public static event Action<PlayerPaymentExchange> PlayerPaymentExchangeEvent;
    public static event Action<string, string, int> RentPaymentMadeBetweenPlayersEvent;
    public static event Action<string> PlayerDeclaredBankruptDueToBankPaymentEvent; //Player is confirmed as bankrupt due to paying the bank a value.

    public static event Action<string, string> PlayerDeclaredBankruptDueToPlayerPaymentEvent; //player bankrupt, player caused. Player is confirmed as bankrupt caused by paying another player.
    public Dictionary<string, string> bankrupcyBetweenPlayersDictionary = new Dictionary<string, string>();//player bankrupt, player caused.

    private List<string> playersDeclaredBankruptList = new List<string>();
    public bool PlayerIsBankrupt(string playerID) => playersDeclaredBankruptList.Contains(playerID);

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawnedEvent;
    }
    private void OnAllPlayersSpawnedEvent()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            string playerID = players[i].UserId;
            PlayerMoneyAccount playerMoneyAccount = GameManager.Instance.GetPlayerPieceByID(playerID).GetComponent<PlayerMoneyAccount>();
            playerMoneyAccount.InitialiseAccount(playerID);
            playerMoneyAccountDictionary.Add(playerID, playerMoneyAccount);
        }
    }

    //Tile purchasing.
    public void ProcessPlayerTilePurchase(PlayerMoneyAccount playerMoneyAccount, TileInstance_Purchasable purchasableTile)
    {
        RemoveMoneyFromAccount(playerMoneyAccount.PlayerID, purchasableTile.PurchaseCost);
        TileOwnershipManager.Instance.ProcessPlayerTilePurchaseAllClients(playerMoneyAccount.PlayerID, purchasableTile.photonView.ViewID);
    }
    public void ProcessPlayerTilePurchase(string playerID, int tilePhotonID,int tilePurchaseCost)
    {
        PlayerMoneyAccount playerMoneyAccount = playerMoneyAccountDictionary[playerID];
        RemoveMoneyFromAccount(playerID, tilePurchaseCost);
        TileOwnershipManager.Instance.ProcessPlayerTilePurchaseAllClients(playerID, tilePhotonID);
    }

    //Adding money to player accounts.
    public void AddMoneyToAccount(string playerID,int amount)
    {
        GetPlayerMoneyAccountByID(playerID).AddToBalance(amount);
    }
    public void AddMoneyToLocalPlayerAccount(int amount)
    {
        GetLocalPlayerMoneyAccount.AddToBalance(amount);
    }

    //Removing money from player accounts.
    public void RemoveMoneyFromAccount(string playerID,int amount)
    {
        GetPlayerMoneyAccountByID(playerID).SubtractFromBalance(amount);
    }
    public void RemoveMoneyFromLocalPlayerAccount(int amount)
    {
        GetLocalPlayerMoneyAccount.SubtractFromBalance(amount);
    }

    //Bankrupcy / Local.
    public void BankruptPlayer(string playerIDToBankrupt)
    {
        photonView.RPC(nameof(FlagPlayerAsBankrupt), RpcTarget.All, playerIDToBankrupt);
      
        string playerNameBankrupt = GameManager.Instance.GetPlayerNicknameFromID(PhotonNetwork.LocalPlayer.UserId);

        if (bankrupcyBetweenPlayersDictionary.ContainsKey(playerIDToBankrupt))
        {
            string playerNameCausedBankrupcy = bankrupcyBetweenPlayersDictionary[playerIDToBankrupt];

            UI_NotificationManager.Instance.RPC_ShowNotificationWithLocalCallback($"{playerNameBankrupt} declared bankrupcy caused by {playerNameCausedBankrupcy}!",
                callback: () =>
                {
                    PlayerDeclaredBankruptDueToPlayerPaymentEvent?.Invoke(playerIDToBankrupt,bankrupcyBetweenPlayersDictionary[playerIDToBankrupt]);
                });

            bankrupcyBetweenPlayersDictionary.Remove(playerIDToBankrupt);
        }
        else
        {
            PlayerDeclaredBankruptDueToBankPaymentEvent?.Invoke(playerIDToBankrupt);
            //UI_NotificationManager.Instance.RPC_ShowNotificationWithLocalCallback($"{playerNameBankrupt} declared bankrupcy caused by a bank payment!",
            //    callback: () =>
            //    {
            //        PlayerDeclaredBankruptDueToBankPaymentEvent?.Invoke(playerIDToBankrupt);
            //    });
        }          
    }

    [PunRPC]
    private void FlagPlayerAsBankrupt(string playerID) => playersDeclaredBankruptList.Add(playerID);

    //Rent between players.
    public void ProcessRentPayment(string playerIDOfRentPayer,string playerIDOfRentReciever,int rentCost)
    {
        RentPaymentMadeBetweenPlayersEvent?.Invoke(playerIDOfRentPayer, playerIDOfRentReciever, rentCost);

        PlayerMoneyAccount accountOfRentPayer = playerMoneyAccountDictionary[playerIDOfRentPayer];
        PlayerMoneyAccount accountOfRentReciever = playerMoneyAccountDictionary[playerIDOfRentReciever];

        string rentPayerName, rentRecieverName;
        rentPayerName = GameManager.Instance.GetPlayerNicknameFromID(playerIDOfRentPayer);
        rentRecieverName = GameManager.Instance.GetPlayerNicknameFromID(playerIDOfRentReciever);

        UI_NotificationManager.Instance.RPC_ShowNotification($"{rentPayerName} paid {rentRecieverName} ${rentCost} in rent.",RpcTarget.All);

        MakePlayerPaymentExchange(accountOfRentPayer, accountOfRentReciever, rentCost);
    }

    //Payment exchange between players.
    public void MakePlayerPaymentExchangeRPC(string playerIDFrom, string playerIDTo, int amount,RpcTarget rpcTarget)
    {
        photonView.RPC(nameof(MakePlayerPaymentExchange), rpcTarget, playerIDFrom, playerIDTo, amount);
    }

    [PunRPC]
    public void MakePlayerPaymentExchange(string playerIDFrom, string playerIDTo, int amount)
    {
        PlayerMoneyAccount playerAccountFrom = GetPlayerMoneyAccountByID(playerIDFrom);
        PlayerMoneyAccount playerAccountTo = GetPlayerMoneyAccountByID(playerIDTo);

        MakePlayerPaymentExchange(playerAccountFrom, playerAccountTo, amount);
    }

    public void MakePlayerPaymentExchange(PlayerMoneyAccount playerFrom,PlayerMoneyAccount playerTo,int amount)
    {
        if (playerFrom.WouldGoBankrupt(amount))
        {
            print($"added to bankrupt between players dict : {GameManager.Instance.GetPlayerNicknameFromID(playerFrom.PlayerID)} bankrupted by {GameManager.Instance.GetPlayerNicknameFromID(playerTo.PlayerID)}");
            bankrupcyBetweenPlayersDictionary.Add(playerFrom.PlayerID, playerTo.PlayerID);
        }

        playerFrom.SubtractFromBalance(amount);
        playerTo.AddToBalance(amount);

        PlayerPaymentExchange paymentExchange = new PlayerPaymentExchange(amount, playerFrom, playerTo);
        PlayerPaymentExchangeEvent?.Invoke(paymentExchange);

        print($"Player payment exchange of ${amount} made between {GameManager.Instance.GetPlayerNicknameFromID(playerFrom.PlayerID)} and {GameManager.Instance.GetPlayerNicknameFromID(playerTo.PlayerID)}.");
    }

    private void OnDestroy()
    {
        GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawnedEvent;
    }
}

public struct PlayerPaymentExchange
{
    public int Amount;
    public PlayerMoneyAccount playerAccountFrom;
    public PlayerMoneyAccount playerAccountTo;

    public PlayerPaymentExchange(int amount, PlayerMoneyAccount playerAccountFrom, PlayerMoneyAccount playerAccountTo)
    {
        this.Amount = amount;
        this.playerAccountFrom = playerAccountFrom;
        this.playerAccountTo = playerAccountTo;
    }
}
