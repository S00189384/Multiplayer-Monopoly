using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class TileInstance_Purchasable : TileInstance
{
    public abstract TileData_Purchasable GetPurchasableData { get; }

    [Header("Purchasable Info")]
    public int PurchaseCost;
    public string OwnerID;
    public bool IsMortgaged;

    //[SerializeField] private bool canBeMortgaged;
    //public bool CanBeMortgaged 
    //{ 
    //    get => canBeMortgaged;
    //    set 
    //    {
    //        canBeMortgaged = value;
    //        if (canBeMortgaged)
    //            BecameMortgageableEvent?.Invoke();
    //        else
    //            BecameUnmortgageableEvent?.Invoke();
    //    }
    //}

    public virtual bool CanBeMortgagedd { get { return IsOwned; } }

    public bool IsOwned { get { return OwnerID != string.Empty && OwnerID != null; } }
    protected bool PlayerLandedIsOwner(string playerIDLanded) => playerIDLanded == OwnerID;

    //public static event Action<string, int> PlayerLandedOnUnownedPurchasableTileEvent;
    public event Action<string> NewOwnerEvent;

    public event Action<TileInstance_Purchasable> MortgagedEvent;
    public event Action<TileInstance_Purchasable> UnmortgagedEvent;

    public event Action BecameMortgageableEvent;
    public event Action BecameUnmortgageableEvent;

    //protected void RaisePlayerLandedOnUnownedPurchasableTileEvent(string playerIDLanded,int photonViewID)
    //{
    //    PlayerLandedOnUnownedPurchasableTileEvent?.Invoke(playerIDLanded, photonViewID);
    //}

    public virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(CustomLayerMasks.unownedLayerMaskName);
    }

    private void Start()
    {
        
    }

    public virtual void MortgageTile()
    {
        Bank.Instance.AddMoneyToAccount(OwnerID, GetPurchasableData.MortgageValue);
        MortgagedEvent?.Invoke(this);
        photonView.RPC(nameof(MortgageTileRPC), RpcTarget.All);
    }

    [PunRPC]
    public void MortgageTileRPC()
    {
        IsMortgaged = true;
    }

    public virtual void UnmortgageTile()
    {
        Bank.Instance.RemoveMoneyFromAccount(OwnerID, GetPurchasableData.UnmortgageCost);
        UnmortgagedEvent?.Invoke(this);
        photonView.RPC(nameof(UnmortgageTileRPC), RpcTarget.All);
    }

    [PunRPC]
    public void UnmortgageTileRPC()
    {
        IsMortgaged = false;
    }

    public void GiveOwnershipToPlayer(string playerID)
    {
        if(playerID == PhotonNetwork.LocalPlayer.UserId)
            gameObject.layer = LayerMask.NameToLayer(CustomLayerMasks.mortgageableLayerName); // Change layer mask for client that owns property only.

        photonView.RPC(nameof(GiveOwnershipToPlayerRPC), RpcTarget.All, playerID);
    }

    [PunRPC]
    public void GiveOwnershipToPlayerRPC(string playerID)
    {
        OwnerID = playerID;
        NewOwnerEvent?.Invoke(OwnerID);
    }

    public void RemovePlayerOwnership()
    {
        gameObject.layer = LayerMask.NameToLayer(CustomLayerMasks.unownedLayerMaskName);
        photonView.RPC(nameof(RemovePlayerOwnershipRPC), RpcTarget.All);
    }
    [PunRPC]
    public void RemovePlayerOwnershipRPC()
    {
        OwnerID = null;
    }
}