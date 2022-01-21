using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class HostManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView pv;

    [SerializeField] GameObject canvas;
    [SerializeField] AudioSource snd_Music;
    [Header("Children")]
    public HostSetup setup;
    public HostMenu menu;
    public HostGame_JO game_JO;
    public HostGame_CN_A game_CN_A;
    public HostGame_NT game_NT;

    void Start()
    {
        if (pv.IsMine && pv.Owner.IsMasterClient)
        {
            GameManager.Instance.hostManager = this;
            setup.Setup();
            menu.Setup();
            canvas.SetActive(true);
            game_JO.Setup();
            game_CN_A.Setup();

            snd_Music.Play();
        }
        else
        {
            canvas.SetActive(false);
        }
    }

    public void LoadScreen(int i)
    {
        if(i==-1)
        {
            setup.gameObject.SetActive(false);
            menu.gameObject.SetActive(true);
        }
    }
}
