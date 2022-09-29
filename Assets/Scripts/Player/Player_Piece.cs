using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Piece : MonoBehaviourPun, IPunObservable
{
    public int TileIndex = GameDataSlinger.PLAYER_START_TILE_INDEX;
    public Player Player;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }    
        else if(stream.IsReading)
        {
            transform.position = (Vector3)stream.ReceiveNext();
        }
    }

    public void RecievePlayerData(Player playerData) => Player = playerData;

    public void SetTileIndex(int index)
    {
        photonView.RPC(nameof(SetTileIndexRPC), RpcTarget.All, index);
    }

    [PunRPC]
    private void SetTileIndexRPC(int index)
    {
        TileIndex = index;
    }
}
