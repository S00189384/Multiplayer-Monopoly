using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to property tile instances. Manages building and removing of houses and hotels.

public class PropertyTileBuildingConstructor : MonoBehaviourPun
{
    private string ownerID;

    private const string HOUSE_PREFAB_RESOURCE_PATH = "Pieces/Piece_House";
    private const string HOTEL_PREFAB_RESOURCE_PATH = "Pieces/Piece_Hotel";

    //owner id - tile photon view id.
    public static event Action<string,int> PlayerBuiltHouseEvent; 
    public static event Action<string,int> PlayerBuiltHotelEvent; 
    public static event Action<string,int> PlayerRemovedHouseEvent; 
    public static event Action<string,int> PlayerRemovedHotelEvent; 

    [Header("Positions of Houses and Hotels")]
    [SerializeField] private Transform[] houseSpawnTransforms = new Transform[GameDataSlinger.NUM_MAX_HOUSES_PER_PROPERTY];
    [SerializeField] private Transform hotelSpawnTransform;

    [Header("Spawned Houses and Hotels")]
    [SerializeField] private List<int> spawnedHousesPhotonIDList = new List<int>();
    private int spawnedHotelPhotonID = -1;

    public int NumHousesBuilt { get { return spawnedHousesPhotonIDList.Count; } }
    public int NumConstructedBuildings { get { return NumHousesBuilt + (HotelOnProperty ? 1 : 0); } }
    public bool HouseOnProperty { get { return spawnedHousesPhotonIDList.Count > 0; } }
    public bool HotelOnProperty { get { return spawnedHotelPhotonID != -1; } }
    public bool PropertyContainsAnyBuildings { get { return HouseOnProperty || HotelOnProperty; } }

    private void Awake()
    {
        GetComponent<TileInstance_Property>().NewOwnerEvent += OnNewOwnerOfThisProperty;
    }

    private void OnNewOwnerOfThisProperty(string ownerID)
    {
        this.ownerID = ownerID;
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Space) && photonView.ViewID == 41)
    //    {
    //        SpawnHouse();
    //    }

    //    if (Input.GetKeyDown(KeyCode.Space) && photonView.ViewID == 39)
    //    {
    //        SpawnHouse();
    //    }
    //}


    #region House Spawning
    public void SpawnHouse()
    {
        int spawnedHousePhotonID = PhotonNetwork.Instantiate(HOUSE_PREFAB_RESOURCE_PATH, houseSpawnTransforms[NumHousesBuilt].position, Quaternion.identity).GetPhotonView().ViewID;
        photonView.RPC(nameof(SpawnHouseRPC), RpcTarget.All, spawnedHousePhotonID);
    }

    [PunRPC]
    private void SpawnHouseRPC(int photonIDOfSpawnedHouse)
    {
        PlayerBuiltHouseEvent?.Invoke(ownerID, photonView.ViewID);
        spawnedHousesPhotonIDList.Add(photonIDOfSpawnedHouse);
    }
    #endregion
    #region House Removing
    public void RemoveAHouse()
    {
        int photonViewIDOfHouse = spawnedHousesPhotonIDList[NumHousesBuilt - 1];
        photonView.RPC(nameof(RemoveAHouseRPC), RpcTarget.All, photonViewIDOfHouse);
    }
    [PunRPC]
    public void RemoveAHouseRPC(int photonViewIDOfHouse)
    {
        PlayerRemovedHouseEvent?.Invoke(ownerID, photonViewIDOfHouse);
        Destroy(PhotonNetwork.GetPhotonView(photonViewIDOfHouse).gameObject);
        spawnedHousesPhotonIDList.Remove(photonViewIDOfHouse);
    }
    #endregion
    #region Hotel Spawning
    public void SpawnHotel()
    {
        spawnedHotelPhotonID = PhotonNetwork.Instantiate(HOTEL_PREFAB_RESOURCE_PATH, hotelSpawnTransform.position, Quaternion.identity).GetPhotonView().ViewID;
        photonView.RPC(nameof(SpawnHotelRPC), RpcTarget.All, spawnedHotelPhotonID);
    }
    [PunRPC]
    public void SpawnHotelRPC(int photonIDOfSpawnedHotel)
    {
        PlayerBuiltHotelEvent?.Invoke(ownerID, photonView.ViewID);
        spawnedHotelPhotonID = photonIDOfSpawnedHotel;
    }
    #endregion
    #region Hotel Removing
    public void RemoveHotel()
    {
        int photonViewIDOfHotel = spawnedHotelPhotonID;
        photonView.RPC(nameof(RemoveHotelRPC), RpcTarget.All,photonViewIDOfHotel);
    }
    [PunRPC]
    public void RemoveHotelRPC(int photonViewIDOfHotel)
    {
        PlayerRemovedHotelEvent?.Invoke(ownerID, photonViewIDOfHotel);
        Destroy(PhotonNetwork.GetPhotonView(photonViewIDOfHotel).gameObject);
        spawnedHotelPhotonID = -1;
    }
    #endregion

    public void DestroyAllBuildings()
    {
        if (HouseOnProperty)
            DestroyAllHouses();

        if (HotelOnProperty)
            RemoveHotelRPC(spawnedHotelPhotonID);
    }

    private void DestroyAllHouses()
    {
        for (int i = 0; i < NumHousesBuilt; i++)
        {
            Destroy(PhotonNetwork.GetPhotonView(spawnedHousesPhotonIDList[i]).gameObject);
        }
        spawnedHousesPhotonIDList.Clear();
    }
}