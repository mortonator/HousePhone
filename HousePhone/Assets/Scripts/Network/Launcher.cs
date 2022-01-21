using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject menuScreen;
    [SerializeField] GameObject connectingScreen;
    string rejoinGame_roomName;

    void Start()
    {
        Debug.Log("ConnectingToServer...");
        PhotonNetwork.ConnectUsingSettings();

        if (PhotonNetwork.InRoom)
        {
            rejoinGame_roomName = PhotonNetwork.CurrentRoom.Name;
            PhotonNetwork.LeaveRoom();
        }
        else if (PhotonNetwork.InLobby)
            OnJoinedLobby();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("ConnectedToServer");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("ConnectingToLobby...");
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("ConnectedToLobby");
        connectingScreen.SetActive(false);
        menuScreen.SetActive(true);
    }
}
