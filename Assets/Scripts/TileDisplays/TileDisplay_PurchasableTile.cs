using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

//Script which handles the appearance of a purchasable tile.
//If the player mortgages / unmortgages the purchasable tile, the tile changes its appearance for all clients to show this.
//When this tile gets a new owner, the sorting layer is changed for the purpose of being highlighted when the player activates the UI for mortgaging / unmortgaging this tile.
//On getting a new owner, an icon is displayed above the tile to show the players who the owner is of that tile.

public abstract class TileDisplay_PurchasableTile : TileDisplay
{
    [Header("Purchasable Tile Components")]
    [SerializeField] protected TextMeshPro TMPRO_Name;
    [SerializeField] protected TextMeshPro TMPRO_PurchaseCost;

    [Header("Mortgaged Tile Components")]
    [SerializeField] protected GameObject GO_Mortgaged;
    [SerializeField] protected TextMeshPro TMP_MortgagedInfo;
    [SerializeField] protected TextMeshPro TMP_TileNameMortgaged;

    [SerializeField] protected SpriteRenderer SPR_OwnerIcon;

    public override void Awake()
    {
        base.Awake();
        GetComponent<TileInstance_Purchasable>().NewOwnerEvent += OnNewOwnerOfTile;
        GetComponent<TileInstance_Purchasable>().LostOwnershipEvent += OnTileLostOwnership;
    }

    private void OnTileLostOwnership()
    {
        RemoveOwnerSprite();
    }

    private void OnNewOwnerOfTile(string playerID)
    {
        //Locally change the sorting layer display for local mortgage selection.
        if (playerID == PhotonNetwork.LocalPlayer.UserId)
            ChangeSortingLayerName("Owned");

        //Change owned display to player icon.
        ChangeOwnerSprite(playerID);
    }

    //private void ChangeOwnerSpriteForAllClients(string playerID)
    //{
    //    photonView.RPC(nameof(ChangeOwnerSprite), RpcTarget.All, playerID);
    //}

    //[PunRPC]
    public void ChangeOwnerSprite(string playerID)
    {
        SPR_OwnerIcon.sprite = GameManager.Instance.GetPlayerSprite(playerID);
    }
    public void RemoveOwnerSprite() => SPR_OwnerIcon.sprite = null;

    public abstract void ChangeToMortgaged(TileInstance_Purchasable purchasableTileInstance);
    public abstract void ChangeToUnmortgaged(TileInstance_Purchasable purchasableTileInstance);

    public abstract void ChangeToMortgagedRemoteClients();
    public abstract void ChangeToUnmortgagedRemoteClients();
}