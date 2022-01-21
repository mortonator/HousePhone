using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class ClientSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView pv;
    [Space]
    [SerializeField] TMPro.TMP_InputField inputF;
    [Header("Avatar")]
    [SerializeField] Image[] avatar = new Image[4];
    [Header("Ready")]
    [SerializeField] Image isReadyButton;
    [SerializeField] Sprite isReady_spr;
    [SerializeField] Sprite isNotReady_spr;

    public int[] avatarIndex { get; protected set; }
    public string inputName { get; protected set; }
    public bool isReady { get; protected set; }

    public void Setup()
    {
        gameObject.SetActive(true);

        avatarIndex = new int[4];

        avatar[0].sprite = GameManager.GetAvatarPart(0, avatarIndex[0]);
        avatar[1].sprite = GameManager.GetAvatarPart(1, avatarIndex[1]);
        avatar[2].sprite = GameManager.GetAvatarPart(2, avatarIndex[2]);
        avatar[3].sprite = GameManager.GetAvatarPart(3, avatarIndex[3]);
    }

    public void NextImage(int i)
    {
        avatarIndex[i] += 1;
        if (avatarIndex[i] > GameManager.avatarLimits[i])
            avatarIndex[i] = 0;

        avatar[i].sprite = GameManager.GetAvatarPart(i, avatarIndex[i]);
    }
    public void PreviousImage(int i)
    {
        avatarIndex[i] -= 1;
        if (avatarIndex[i] < 0)
            avatarIndex[i] = GameManager.avatarLimits[i];

        avatar[i].sprite = GameManager.GetAvatarPart(i, avatarIndex[i]);
    }

    public void ConfirmSetup()
    {
        if (string.IsNullOrEmpty(inputF.text))
            return;

        isReady = !isReady;
        isReadyButton.sprite = isReady ? isReady_spr : isNotReady_spr;
        inputName = inputF.text;

        pv.RPC("RPC_ConfirmSetup", RpcTarget.MasterClient, inputName, avatarIndex, isReady);
    }
    [PunRPC] public void RPC_ConfirmSetup(string n, int[] i, bool _ready)
    {
        Debug.Log("parts: " + i + " | " + i.Length);

        inputName = n;
        avatarIndex = i;
        isReady = _ready;
        GameManager.Instance.hostManager.setup.UpdatePlayerItems();
    }
}
