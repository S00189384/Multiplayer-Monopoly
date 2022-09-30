using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//Player piece script which holds the tile index that the player is currently on.

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
