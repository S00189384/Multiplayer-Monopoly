using Photon.Pun;
using System;
using UnityEngine;

//Tile instance script for a purchasable tile - properties, utilities and stations.
//A purchasable tile can be mortgaged / unmortgaged and owned by a player.

[RequireComponent(typeof(Collider))]
public abstract class TileInstance_Purchasable : TileInstance
{
    public abstract TileData_Purchasable GetPurchasableData { get; }

    [Header("Purchasable Info")]
    public int PurchaseCost;
    public string OwnerID;
    public bool IsMortgaged;

    public virtual bool CanBeMortgaged { get { return IsOwned; } }
    public bool IsOwned { get { return OwnerID != string.Empty && OwnerID != null; } }
    protected bool PlayerLandedIsOwner(string playerIDLanded) => playerIDLanded == OwnerID;

    //Events.
    public event Action<string> NewOwnerEvent;
    public event Action LostOwnershipEvent;
    public event Action<TileInstance_Purchasable> MortgagedEvent;
    public event Action<TileInstance_Purchasable> UnmortgagedEvent;

    //Start.
    public virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(CustomLayerMasks.unownedLayerMaskName);
    }

    //Mortgaging / Unmortgaging.
    public virtual void MortgageTile()
    {
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
        UnmortgagedEvent?.Invoke(this);
        photonView.RPC(nameof(UnmortgageTileRPC), RpcTarget.All);
    }

    public void UnmortgageTileOnTileReset()
    {
        UnmortgagedEvent?.Invoke(this);
        IsMortgaged = false;
    }

    [PunRPC]
    public void UnmortgageTileRPC()
    {
        IsMortgaged = false;
    }

    //Ownership.
    public void GiveOwnershipToPlayer(string playerID)
    {
        // Change layer mask for client that owns property only.
        if (playerID == PhotonNetwork.LocalPlayer.UserId)
            gameObject.layer = LayerMask.NameToLayer(CustomLayerMasks.mortgageableLayerName);

        OwnerID = playerID;
        NewOwnerEvent?.Invoke(OwnerID);
    }

    [PunRPC]
    public void GiveOwnershipToPlayerForOtherClients(string playerID)
    {
        OwnerID = playerID;
    }

    public void RemovePlayerOwnershipForLocalClient()
    {
        gameObject.layer = LayerMask.NameToLayer(CustomLayerMasks.unownedLayerMaskName);
        OwnerID = string.Empty;
        LostOwnershipEvent?.Invoke();
    }

    public void RemovePlayerOwnership()
    {
        gameObject.layer = LayerMask.NameToLayer(CustomLayerMasks.unownedLayerMaskName);
        photonView.RPC(nameof(RemovePlayerOwnershipRPC), RpcTarget.All);
    }
    [PunRPC]
    public void RemovePlayerOwnershipRPC()
    {
        OwnerID = string.Empty;
    }
}