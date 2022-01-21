using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.IO;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text errorText;
    [SerializeField] Button joinButton;
    [SerializeField] Button clearJoinButton;
    [SerializeField] TMP_InputField joinInput;

    List<RoomInfo> roomList;
    static readonly string[] Alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

    void Start()
    {
        joinInput.onValidateInput += delegate (string s, int i, char c)
        {
            if (s.Length >= 6) { return '\0'; }
            c = char.ToUpper(c);
            return char.IsLetter(c) ? c : '\0';
        };
    }


    #region Callbacks
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.roomList = roomList;
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorText.text = message;
        errorText.color = Color.red;
    }
    public override void OnJoinedRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    #endregion

    #region Buttons
    public void HostButton()
    {
        RoomInfo _roomExists = null;
        string _roomName = "";
        do
        {
            _roomName = "";
            for (int i = 0; i < 6; i++)
                _roomName += (char)Random.Range(65, 91);

            _roomExists = null;
            _roomExists = roomList.FirstOrDefault(r => r.Name == _roomName);
        }
        while (_roomExists != null);

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 10;
        PhotonNetwork.CreateRoom(_roomName, options);
    }

    public void CheckJoinInput(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            clearJoinButton.interactable = false;
            joinButton.interactable = false;

            return;
        }

        clearJoinButton.interactable = true;
        joinButton.interactable = true;
    }
    public void ClearJoinInput()
    {
        joinInput.text = string.Empty;

        clearJoinButton.interactable = false;
        joinButton.interactable = false;
    }
    public void JoinButton()
    {
        errorText.text = "Joining " + joinInput.text;
        errorText.color = Color.black;

        if (PhotonNetwork.ReconnectAndRejoin() == false)
            PhotonNetwork.JoinRoom(joinInput.text.ToUpper());
    }
    public void Cancel()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.ConnectUsingSettings();
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
}
