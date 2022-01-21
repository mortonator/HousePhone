using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public static ClientManager localClient;


    public Dictionary<int, ClientManager> clientManagers;
    [HideInInspector] public HostManager hostManager;

    PhotonView pv;

    static Sprite[] bodies;
    static Sprite[] faces;
    static Sprite[] hairs;
    static Sprite[] kits;
    public static int[] avatarLimits { get; protected set; }


    void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);

        Instance = this;
        hostManager = null;
        pv = GetComponent<PhotonView>();
        clientManagers = new Dictionary<int, ClientManager>();

        LoadAvatarParts();

        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SpawnManager"), Vector3.zero, Quaternion.identity);
    }
    void LoadAvatarParts()
    {
        bodies = Resources.LoadAll<Sprite>("Avatars/Bodies");
        faces = Resources.LoadAll<Sprite>("Avatars/Faces");
        hairs = Resources.LoadAll<Sprite>("Avatars/Hairs");
        kits = Resources.LoadAll<Sprite>("Avatars/Kits");
        avatarLimits = new int[4] { bodies.Length - 1, faces.Length - 1, hairs.Length - 1, kits.Length - 1 };
    }
    public static Sprite GetAvatarPart(int p, int i)
    {
        if (p == 0)
            return bodies[i];

        if (p == 1)
            return faces[i];

        if (p == 2)
            return hairs[i];

        return kits[i];
    }


    public void AddPlayerItem(int actorNumber, ClientManager client)
    {
        clientManagers.Add(actorNumber, client);
    }
    public void RemovePlayerItem(int actorNumber)
    {
        clientManagers.Remove(actorNumber);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.IsMasterClient)
        {
            SceneManager.LoadScene(0);
            PhotonNetwork.LeaveRoom();
        }
        else if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            RemovePlayerItem(otherPlayer.ActorNumber);
        }
    }
}