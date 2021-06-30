using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkController: MonoBehaviourPunCallbacks
{
    public static TypedLobby Lobby;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion("usw");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connection established!");
    }

    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from server for reason: {cause}");
    }

}
