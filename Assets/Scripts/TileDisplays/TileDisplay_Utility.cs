using Photon.Pun;
using UnityEngine;

public class TileDisplay_Utility : TileDisplay_PurchasableTile
{
    [HideInInspector]
    [SerializeField] private TileData_Utility utilityData;

    [SerializeField] private SpriteRenderer SPREND_Utility;

    public override void Awake()
    {
        base.Awake();
        TileInstance_Purchasable myPurchasableTileInstance = GetComponent<TileInstance_Purchasable>();
        myPurchasableTileInstance.MortgagedEvent += ChangeToMortgaged;
        myPurchasableTileInstance.UnmortgagedEvent += ChangeToUnmortgaged;
    }

    public override void UpdateDisplay(TileData tileData)
    {
        utilityData = (TileData_Utility)tileData;

        TMPRO_Name.text = utilityData.Name;
        TMPRO_PurchaseCost.text = $"Cost ${utilityData.PurchaseCost}";

        SPREND_Utility.sprite = utilityData.SPR_Utility;
    }

    public override void ChangeToMortgaged(TileInstance_Purchasable purchasableTileInstance)
    {
        GO_Mortgaged.SetActive(true);
        TMP_TileNameMortgaged.text = utilityData.Name;
        TMP_MortgagedInfo.text = $"Mortgaged for ${utilityData.PurchaseCost}";

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
        TMP_TileNameMortgaged.text = utilityData.Name;
        TMP_MortgagedInfo.text = $"Mortgaged for ${utilityData.PurchaseCost}";
    }

    [PunRPC]
    public override void ChangeToUnmortgagedRemoteClients()
    {
        GO_Mortgaged.SetActive(false);
    }
}