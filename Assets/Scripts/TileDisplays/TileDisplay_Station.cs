using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDisplay_Station : TileDisplay_PurchasableTile
{
    [HideInInspector]
    [SerializeField] private TileData_Station stationData;
    [SerializeField] private SpriteRenderer SPREND_Station;

    public override void Awake()
    {
        base.Awake();
        TileInstance_Purchasable myPurchasableTileInstance = GetComponent<TileInstance_Purchasable>();
        myPurchasableTileInstance.MortgagedEvent += ChangeToMortgaged;
        myPurchasableTileInstance.UnmortgagedEvent += ChangeToUnmortgaged;
    }

    public override void UpdateDisplay(TileData tileData)
    {
        stationData = (TileData_Station) tileData;

        TMPRO_Name.text = stationData.Name;
        TMPRO_PurchaseCost.text =  $"Cost ${stationData.PurchaseCost}";

        SPREND_Station.sprite = stationData.SPR_Station;
    }

    public override void ChangeToMortgaged(TileInstance_Purchasable purchasableTileInstance)
    {
        GO_Mortgaged.SetActive(true);
        TMP_TileNameMortgaged.text = stationData.Name;
        TMP_MortgagedInfo.text = $"Mortgaged for ${stationData.PurchaseCost}";

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
        TMP_TileNameMortgaged.text = stationData.Name;
        TMP_MortgagedInfo.text = $"Mortgaged for ${stationData.PurchaseCost}";
    }

    [PunRPC]
    public override void ChangeToUnmortgagedRemoteClients()
    {
        GO_Mortgaged.SetActive(false);
    }
}