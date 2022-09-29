using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoneyAccount : MonoBehaviourPun
{
    public string PlayerID;
    [SerializeField] private int balance;
    public int Balance 
    { 
        get => balance; 
        set 
        {
            int previousBalance = balance;

            balance = value;
            BalanceChangedEvent?.Invoke(balance);

            if (previousBalance < 0 && balance > 0)
            {
                print("Player left bankrupcy");
                LeftBankruptsyEvent?.Invoke(this);
            }

            if (balance < 0)
            {
                print("Player entered bankrupcy");
                EnteredBankruptsyEvent?.Invoke(this);
            }
        } 
    }

    public event Action<PlayerMoneyAccount> EnteredBankruptsyEvent;
    public event Action<PlayerMoneyAccount> LeftBankruptsyEvent;
    public event Action<int> BalanceChangedEvent;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && photonView.IsMine)
        {
            SetBalance(1);
        }
    }


    public void InitialiseAccount(string playerID)
    {
        PlayerID = playerID;
        Balance = GameDataSlinger.PLAYER_STARTING_MONEY;
    }
    public int GetBalanceWithPurchase(int purchaseCost) => Balance - purchaseCost;
    public int GetBalanceWithMoneyGain(int gainAmount) => Balance + gainAmount;
    public bool WouldGoBankruptWithPurchase(int purchaseCost) => Balance - purchaseCost < 0;

    public void AddToBalance(int amount)
    {
        Balance += amount;
        photonView.RPC(nameof(AddToBalanceRemoteClients), RpcTarget.Others, amount);
    }

    [PunRPC]
    private void AddToBalanceRemoteClients(int amount)
    {
        Balance += amount;

    }
    public void SubtractFromBalance(int amount)
    {
        Balance -= amount;
        photonView.RPC(nameof(SubtractFromBalanceRemoteClients), RpcTarget.Others, amount);
    }

    [PunRPC]
    private void SubtractFromBalanceRemoteClients(int amount)
    {
        Balance -= amount;
    }

    public void SetBalance(int amount)
    {
        Balance = amount;
        photonView.RPC(nameof(SetBalanceRemoteClients), RpcTarget.Others, amount);
    }

    [PunRPC]
    private void SetBalanceRemoteClients(int amount)
    {
        Balance = amount;
    }

    public bool CanAffordPurchase(int purchaseCost) => (Balance - purchaseCost) > 0;
}