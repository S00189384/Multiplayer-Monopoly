using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPropertyBuildingTracker : MonoBehaviourPun
{
    private string playerID;

    public event Action PlayerGainedAbilityToSellPropertyBuildings;
    public event Action PlayerLostAbilityToSellPropertyBuildings;

    [SerializeField] private int numConstructedBuildings;
    private int NumConstructedBuildings 
    {
        get => numConstructedBuildings;
        set
        {
            numConstructedBuildings = value;
            if(numConstructedBuildings <= 0)
            {
                PlayerLostAbilityToSellPropertyBuildings?.Invoke();
            }
            else if(numConstructedBuildings == 1) 
            {
                PlayerGainedAbilityToSellPropertyBuildings?.Invoke();
            }
        } 
    }

    [PunRPC]
    private void RemoteSetNumConstructedBuildings(int numConstructedBuildings)
    {
        NumConstructedBuildings = numConstructedBuildings;
    }

    public bool CanSellAPropertyBuilding { get { return numConstructedBuildings > 0; } }

    private void Awake()
    {
        GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;

        PropertyTileBuildingConstructor.PlayerBuiltHouseEvent += OnPlayerBuiltAHouse;
        PropertyTileBuildingConstructor.PlayerBuiltHotelEvent += OnPlayerBuiltAHotel;

        PropertyTileBuildingConstructor.PlayerRemovedHotelEvent += OnPlayerRemovedHotel;
        PropertyTileBuildingConstructor.PlayerRemovedHouseEvent += OnPlayerRemovedHouse;
    }

    private void OnAllPlayersSpawned()
    {
        playerID = GetComponent<Player_Piece>().Player.UserId;
    }

    private void OnPlayerBuiltAHouse(string playerID, int photonIDOfTile)
    {
        if (this.playerID == playerID)
            NumConstructedBuildings++;
    }

    private void OnPlayerBuiltAHotel(string playerID, int photonIDOfTile)
    {
        if (this.playerID == playerID)
            NumConstructedBuildings++;
    }
    private void OnPlayerRemovedHouse(string playerID, int photonIDOfTile)
    {
        if (this.playerID == playerID)
            NumConstructedBuildings--;
    }

    private void OnPlayerRemovedHotel(string playerID, int photonIDOfTile)
    {
        if (this.playerID == playerID)
            NumConstructedBuildings--;
    }
}