using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ClientManager : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    [Space]

    [SerializeField] GameObject canvas;
    public ClientSetup setup;
    public GameObject menu;
    public ClientGame_JO game_JO;
    public ClientGame_CN_A game_CN_A;
    public ClientGame_NT game_NT;

    void Start()
    {
        if (pv.IsMine && !pv.Owner.IsMasterClient)
        {
            GameManager.localClient = this;

            canvas.SetActive(true);
            setup.Setup();
            menu.SetActive(false);
            game_JO.Setup();
            game_CN_A.Setup();
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            canvas.SetActive(false);
            GameManager.Instance.AddPlayerItem(pv.Owner.ActorNumber, this);
        }
        else
        {
            canvas.SetActive(false);
        }
    }

    public void LoadScreen(int index)
    {
        if (index == -1) //menu
        {
            setup.gameObject.SetActive(false);
            game_JO.gameObject.SetActive(false);
            menu.SetActive(true);
        }
        else if (index == 0) //game 1, JustOne!
        {
            menu.SetActive(false);
            game_JO.SetupGame();
        }
        else if (index == 1) //game 2, Codenames
        {
            menu.SetActive(false);
            game_CN_A.SetupGame();
        }
    }
}
