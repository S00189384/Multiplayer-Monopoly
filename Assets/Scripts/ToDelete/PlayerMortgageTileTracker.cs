using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMortgageTileTracker : MonoBehaviour
{
    //[SerializeField] private List<int> mortgagedTilesPhotonIDList = new List<int>();

    //public int NumMortgagedTiles { get { return mortgagedTilesPhotonIDList.Count; } }

    //public event Action PlayerNowOwnsAMortgagedTileEvent;
    //public event Action PlayerNoLongerOwnsAMortgagedTileEvent;

    //private void Awake()
    //{
    //    OwnedPlayerTileTracker playerOwnedTileTracker = GetComponent<OwnedPlayerTileTracker>();
    //    playerOwnedTileTracker.GainedTileEvent += OnPlayerGainedATile;
    //    playerOwnedTileTracker.LostTileEvent += OnPlayerLostATile;
    //}

    //private void OnPlayerGainedATile(TileInstance_Purchasable tileGained)
    //{
    //    tileGained.MortgagedEvent += OnTileBecameMortgaged;
    //    tileGained.UnmortgagedEvent += OnTileBecameUnmortgaged;
    //}
    //private void OnPlayerLostATile(TileInstance_Purchasable tileLost)
    //{
    //    tileLost.MortgagedEvent -= OnTileBecameMortgaged;
    //    tileLost.UnmortgagedEvent -= OnTileBecameUnmortgaged;
    //}

    //private void OnTileBecameMortgaged(TileInstance_Purchasable tileThatWasMortgaged)
    //{
    //    mortgagedTilesPhotonIDList.Add(tileThatWasMortgaged.photonView.ViewID);
    //    if (NumMortgagedTiles == 1)
    //        PlayerNowOwnsAMortgagedTileEvent?.Invoke();

    //    print("Mortgage tile tracker added mortgaged tile.");
    //}

    //private void OnTileBecameUnmortgaged(TileInstance_Purchasable tileThatWasUnmortgaged)
    //{
    //    mortgagedTilesPhotonIDList.Remove(tileThatWasUnmortgaged.photonView.ViewID);
    //    if (NumMortgagedTiles <= 0)
    //        PlayerNoLongerOwnsAMortgagedTileEvent?.Invoke();
    //    print("Mortgage tile tracker removed unmortgaged tile.");
    //}
}