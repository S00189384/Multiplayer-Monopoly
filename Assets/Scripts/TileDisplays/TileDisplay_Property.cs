using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileDisplay_Property : TileDisplay_PurchasableTile
{
    [HideInInspector]
    [SerializeField] private TileData_Property propertyData;

    [SerializeField] private SpriteRenderer SREND_PropertyColour;

    public override void Awake() 
    {
        base.Awake();
        TileInstance_Purchasable myPurchasableTileInstance = GetComponent<TileInstance_Purchasable>();
        myPurchasableTileInstance.MortgagedEvent += ChangeToMortgaged;
        myPurchasableTileInstance.UnmortgagedEvent += ChangeToUnmortgaged;
    }

    public override void UpdateDisplay(TileData tileData)
    {
        propertyData = tileData as TileData_Property;

        this.tileData = propertyData;

        if (propertyData != null)
        {
            TMPRO_Name.text = propertyData.Name;
            TMPRO_PurchaseCost.text = $"${propertyData.PurchaseCost}";
            SREND_PropertyColour.color = propertyData.PropertyColourSet.propertyColour;
        }
    }
    public override void ChangeToMortgaged(TileInstance_Purchasable purchasableTileInstance)
    {
        GO_Mortgaged.SetActive(true);
        TMP_TileNameMortgaged.text = propertyData.Name;   
        TMP_MortgagedInfo.text = $"Mortgaged for ${propertyData.PurchaseCost}";

        photonView.RPC(nameof(ChangeToMortgagedRemoteClients), RpcTarget.Others);
    }

    public override void ChangeToUnmortgaged(TileInstance_Purchasable purchasableTileInstance)
    {
        GO_Mortgaged.SetActive(false);

        photonView.RPC(nameof(ChangeToUnmortgagedRemoteClients), RpcTarget.Others);
    }

    [PunRPC]
    public override void ChangeToMortgagedRemoteClients()
    {
        GO_Mortgaged.SetActive(true);
        TMP_TileNameMortgaged.text = propertyData.Name;
        TMP_MortgagedInfo.text = $"Mortgaged for ${propertyData.PurchaseCost}";
    }

    [PunRPC]
    public override void ChangeToUnmortgagedRemoteClients()
    {
        GO_Mortgaged.SetActive(false);
    }
}