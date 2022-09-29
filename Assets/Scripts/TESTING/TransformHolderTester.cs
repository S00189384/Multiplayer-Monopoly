using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransformHolderTester : MonoBehaviourPun
{
    public TextMeshProUGUI tmproSpawnIndex;

    public Transform[] transforms;
    public int currentSpawnIndex = 0;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Player[] players = PhotonNetwork.PlayerList;
            for (int i = 0; i < players.Length; i++)
            {
                //GameObject player = PhotonNetwork.Instantiate($"Pieces/PlayerPiece", Vector3.zero, Quaternion.identity);
                //player.transform.position = transforms[currentSpawnIndex].position;
                Vector3 spawnPosition = transforms[currentSpawnIndex].position;
                photonView.RPC(nameof(SpawnPlayer), players[i], spawnPosition);
            }
        }

        //GameObject player = PhotonNetwork.Instantiate($"Pieces/PlayerPiece", Vector3.zero, Quaternion.identity);
        //player.transform.position = transforms[currentSpawnIndex].position;

        //photonView.RPC("SpawnPlayer", RpcTarget.Others);
    }

    [PunRPC]
    public void SpawnPlayer(Vector3 position)
    {
        GameObject player = PhotonNetwork.Instantiate($"Pieces/PlayerPiece", Vector3.zero, Quaternion.identity);
        player.transform.position = position;
        currentSpawnIndex++;
        tmproSpawnIndex.text = currentSpawnIndex.ToString();
    }
}
