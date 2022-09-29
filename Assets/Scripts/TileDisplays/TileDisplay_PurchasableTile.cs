using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class TileDisplay_PurchasableTile : TileDisplay
{
    [Header("Purchasable Tile Components")]
    [SerializeField] protected TextMeshPro TMPRO_Name;
    [SerializeField] protected TextMeshPro TMPRO_PurchaseCost;

    [Header("Mortgaged Tile Components")]
    [SerializeField] protected GameObject GO_Mortgaged;
    [SerializeField] protected TextMeshPro TMP_MortgagedInfo;
    [SerializeField] protected TextMeshPro TMP_TileNameMortgaged;

    public override void Awake()
    {
        base.Awake();
        GetComponent<TileInstance_Purchasable>().NewOwnerEvent += OnNewOwnerOfTile;
    }

    private void OnNewOwnerOfTile(string playerID)
    {
        //Locally change the sorting layer display for local mortgage selection.
        if (playerID == PhotonNetwork.LocalPlayer.UserId)
            ChangeSortingLayerName("Owned");

        //Change owned display to player icon.
    }

    public abstract void ChangeToMortgaged(TileInstance_Purchasable purchasableTileInstance);
    public abstract void ChangeToUnmortgaged(TileInstance_Purchasable purchasableTileInstance);

    public abstract void ChangeToMortgagedRemoteClients();
    public abstract void ChangeToUnmortgagedRemoteClients();
}