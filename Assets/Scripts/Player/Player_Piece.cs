using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

//Player piece script which holds the tile index that the player is currently on.
//Player piece models aare capsules for now until I create the 3d models for monopoly pieces.
//As a temporary thing, the players owned piece is set to green for them to show them what their piece is.

public class Player_Piece : MonoBehaviourPun, IPunObservable
{
    public int TileIndex = GameDataSlinger.PLAYER_START_TILE_INDEX;
    public Player Player;
    public Material localPlayerMaterial;

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

    public void RecievePlayerData(Player playerData)
    {
        Player = playerData;

        if(Player.UserId == PhotonNetwork.LocalPlayer.UserId)
            GetComponent<MeshRenderer>().material = localPlayerMaterial;
    }

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