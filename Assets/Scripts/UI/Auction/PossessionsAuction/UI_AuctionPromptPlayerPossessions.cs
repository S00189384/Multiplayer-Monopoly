using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AuctionPromptPlayerPossessions : MonoBehaviour
{
    [SerializeField] private UI_AuctionPlayerPossessionsPanel playerPossessionsPanel;
    [SerializeField] private UI_AuctionTurnDisplayManager turnDisplayManager;
    [SerializeField] private GameObject GO_SpectatingAuctionDisplay;

    public void InitialisePrompt(string playerIDPossessionsUpForAuction)
    {
        GetComponent<AuctionTurnManager>().ReceiveAuctionTypeOfCurrentAuction(AuctionType.PlayerPossessions);

        turnDisplayManager.SpawnActivePlayersDisplay(GameManager.Instance.ActivePlayersIDList);

        OwnedPlayerTileTracker ownedPlayerTileTracker = TileOwnershipManager.Instance.GetOwnedPlayerTileTracker(playerIDPossessionsUpForAuction);
        PlayerInventory playerInventory = GameManager.Instance.GetPlayerPieceByID(playerIDPossessionsUpForAuction).GetComponent<PlayerInventory>();
        playerPossessionsPanel.InitialisePanel(ownedPlayerTileTracker, playerInventory);

        if (!GameManager.Instance.LocalPlayerIsAnActivePlayer)
            GO_SpectatingAuctionDisplay.SetActive(true);
    }
}
