using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Changed script execution order to fix the issue that new turn event would fire and the player displays had not been spawned yet to handle the new turn.

//TODO:
//Master client leaves destroys this spawned displays for all players - transfer ownership when client disconnects?

public class PlayerDisplayManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private const string playerDisplayPrefabResourcesPath = "UI/PlayerDisplay";

    private Dictionary<string, UI_PlayerDisplay> spawnedPlayerDisplayDict = new Dictionary<string, UI_PlayerDisplay>();
    private string previousTurn;

    private void Awake()
    {
        if(PhotonNetwork.IsMasterClient)
            GameManager.AllPlayersSpawnedEvent += OnAllPlayersSpawned;

        PhotonNetwork.AddCallbackTarget(this);
        Bank.PlayerDeclaredBankruptDueToBankPaymentEvent += OnPlayerDeclaredBankrupcy;
        Bank.PlayerDeclaredBankruptDueToPlayerPaymentEvent += OnPlayerDelcaredBankruptDueToPlayer;
    }

    private void OnPlayerDelcaredBankruptDueToPlayer(string playerIDBankrupt, string playerIDCausedBankrupcy)
    {
        spawnedPlayerDisplayDict[playerIDBankrupt].ChangeDisplayToBankrupt(RpcTarget.All);
    }

    private void OnPlayerDeclaredBankrupcy(string playerID)
    {
        spawnedPlayerDisplayDict[playerID].ChangeDisplayToBankrupt(RpcTarget.All);
    }

    private void OnAllPlayersSpawned()
    {
        SpawnPlayerDisplays();      
    }

    private void OnNewPlayerTurn(string playerID)
    {
        if (previousTurn != string.Empty && previousTurn != null)
            spawnedPlayerDisplayDict[previousTurn].ChangeOutlineColour(Color.black);

        spawnedPlayerDisplayDict[playerID].ChangeOutlineColour(Color.green);

        previousTurn = playerID;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        spawnedPlayerDisplayDict[otherPlayer.UserId].ChangeDisplayToDisconnected();
        spawnedPlayerDisplayDict.Remove(otherPlayer.UserId);
    }

    public void SpawnPlayerDisplays() //Master client only spawns displays.
    {
        for (int i = 0; i < GameManager.Instance.SpawnedPlayersDictionary.Count; i++)
        {
            string playerID = GameManager.Instance.SpawnedPlayersDictionary.Keys.ElementAt(i);
            string playerName = GameManager.Instance.GetPlayerNicknameFromID(playerID);

            GameObject spawnedPlayerDisplay = PhotonNetwork.Instantiate(playerDisplayPrefabResourcesPath, Vector3.zero, Quaternion.identity);
            int spawnedPlayerDisplayViewID = spawnedPlayerDisplay.GetPhotonView().ViewID;
            photonView.RPC(nameof(OnPlayerDisplaySpawned), RpcTarget.All, spawnedPlayerDisplayViewID,playerID,playerName);
        }
    }

    [PunRPC]
    private void OnPlayerDisplaySpawned(int viewIDOfSpawnedDisplay, string playerID, string playerName)
    {
        UI_PlayerDisplay spawnedPlayerDisplay = PhotonNetwork.GetPhotonView(viewIDOfSpawnedDisplay).gameObject.GetComponent<UI_PlayerDisplay>();
        FixDisplayOfSpawnedPlayerDisplay(spawnedPlayerDisplay.transform);
        spawnedPlayerDisplay.UpdateDisplay(playerName, $"${GameDataSlinger.PLAYER_STARTING_MONEY.ToString()}", GameManager.Instance.GetPlayerSprite(playerID));
        spawnedPlayerDisplayDict.Add(playerID, spawnedPlayerDisplay);

        GameManager.Instance.GetPlayerPieceByID(playerID).GetComponent<PlayerMoneyAccount>().BalanceChangedEvent += spawnedPlayerDisplay.ChangeBalance;
    }

    private void FixDisplayOfSpawnedPlayerDisplay(Transform spawnedDisplayTransform)
    {
        spawnedDisplayTransform.SetParent(this.transform);
        spawnedDisplayTransform.position = transform.position;
        spawnedDisplayTransform.localEulerAngles = Vector3.zero;
        spawnedDisplayTransform.localScale = Vector3.one;
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == EventCodes.NewTurnEventCode)
        {
            string newTurnID = (string)photonEvent.CustomData;
            OnNewPlayerTurn(newTurnID);
        }
    }
    private void OnDestroy()
    {
        if(PhotonNetwork.IsMasterClient)
            GameManager.AllPlayersSpawnedEvent -= OnAllPlayersSpawned;

        PhotonNetwork.RemoveCallbackTarget(this);
        Bank.PlayerDeclaredBankruptDueToBankPaymentEvent -= OnPlayerDeclaredBankrupcy;
        Bank.PlayerDeclaredBankruptDueToPlayerPaymentEvent -= OnPlayerDelcaredBankruptDueToPlayer;
    }
}