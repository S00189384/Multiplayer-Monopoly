using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectingScene : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        if(!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
