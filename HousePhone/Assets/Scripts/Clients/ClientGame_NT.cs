using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ClientGame_NT : MonoBehaviourPunCallbacks // NoThanks
{
    [SerializeField] TMP_Text pointCounter;
    [SerializeField] TMP_Text chipCounter;
    [SerializeField] GameObject[] cards;

    public void YesButton()
    {

    }
    public void NoButton()
    {

    }
}
