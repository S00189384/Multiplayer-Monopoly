using Photon.Pun;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Prompt for an auction for a single purchasable tile.
//Depending on the type of the tile, the prompt spawns a different display for the tile to show each player the information for the tile being auctioned.

public class UI_AuctionPromptSinglePurchasableTile : MonoBehaviourPun
{
    [Header("Possible Tile Info Display Spawning")]
    [SerializeField] private UI_PropertyInformation propertyInfoPrefab;
    [SerializeField] private UI_StationInformation stationInfoPrefab;
    [SerializeField] private UI_UtilityInformation utilityInfoPrefab;
    [SerializeField] private Transform tileInfoSpawnTransform;
    [Header("Components of prompt")]
    [SerializeField] private TextMeshProUGUI TMP_OriginalTilePurchaseCost;
    [SerializeField] private GameObject GO_SpectatingAuctionDisplay;
    [SerializeField] private UI_AuctionTurnDisplayManager turnDisplayManager;

    public void InitialisePrompt(string playerIDStartedAuction, int photonIDOfTileForAuction)
    {
        GetComponent<AuctionTurnManager>().ReceiveAuctionTypeOfCurrentAuction(AuctionType.SingleTile);

        //Spawn info display of the tile that is up for auction.
        Dictionary<Type,Action<int>> tileInfoDisplayMethodCallDictionary = new Dictionary<Type, Action<int>>()
        {
             { typeof(TileInstance_Property), SpawnPropertyInfoDisplay},
             { typeof(TileInstance_Station), SpawnStationInfoDisplay},
             { typeof(TileInstance_Utility), SpawnUtilityInfoDisplay}
        };

        Type typeOfPurchasableTile = TileOwnershipManager.Instance.GetPurchasableTileTypeFromPhotonID(photonIDOfTileForAuction);
        tileInfoDisplayMethodCallDictionary[typeOfPurchasableTile](photonIDOfTileForAuction);

        turnDisplayManager.SpawnActivePlayersDisplay(GameManager.Instance.ActivePlayersIDList);

        if (!GameManager.Instance.LocalPlayerIsAnActivePlayer)
            GO_SpectatingAuctionDisplay.SetActive(true);
    }

    public void SpawnPropertyInfoDisplay(int photonViewID)
    {
        TileInstance_Property propertyLandedOn = PhotonNetwork.GetPhotonView(photonViewID).GetComponent<TileInstance_Property>();
        UI_PropertyInformation spawnedPropertyInformation = Instantiate(propertyInfoPrefab, tileInfoSpawnTransform.position, Quaternion.identity, tileInfoSpawnTransform);
        spawnedPropertyInformation.transform.localEulerAngles = Vector3.zero;
        spawnedPropertyInformation.UpdateDisplay(propertyLandedOn.propertyData);
        TMP_OriginalTilePurchaseCost.text = $"Original purchase cost: ${propertyLandedOn.PurchaseCost}";
    }
    private void SpawnStationInfoDisplay(int photonViewID)
    {
        TileInstance_Station stationLandedOn = PhotonNetwork.GetPhotonView(photonViewID).GetComponent<TileInstance_Station>();
        UI_StationInformation spawnedStationInformation = Instantiate(stationInfoPrefab, tileInfoSpawnTransform.position, Quaternion.identity, tileInfoSpawnTransform);
        spawnedStationInformation.transform.localEulerAngles = Vector3.zero;
        spawnedStationInformation.UpdateDisplay(stationLandedOn.tileDataStation);
        TMP_OriginalTilePurchaseCost.text = $"Original purchase cost: ${stationLandedOn.PurchaseCost}";
    }

    private void SpawnUtilityInfoDisplay(int photonViewID)
    {
        TileInstance_Utility utilityLandedOn = PhotonNetwork.GetPhotonView(photonViewID).GetComponent<TileInstance_Utility>();
        UI_UtilityInformation spawnedUtilityInformation = Instantiate(utilityInfoPrefab, tileInfoSpawnTransform.position, Quaternion.identity, tileInfoSpawnTransform);
        spawnedUtilityInformation.transform.localEulerAngles = Vector3.zero;
        spawnedUtilityInformation.UpdateDisplay(utilityLandedOn.tileDataUtility);
        TMP_OriginalTilePurchaseCost.text = $"Original purchase cost: ${utilityLandedOn.PurchaseCost}";
    }
}