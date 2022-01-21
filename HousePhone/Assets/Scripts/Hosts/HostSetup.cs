using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class HostSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView pv;
    [Space]
    [SerializeField] TextMeshProUGUI codeText;
    [SerializeField] TextMeshProUGUI readyTimerText;
    [Space]
    [SerializeField] ClientSetupItem clientItemPrefab;
    [SerializeField] Transform itemParents;

    List<ClientSetupItem> clientItems;
    Coroutine c;

    public void Setup()
    {
        gameObject.SetActive(true);
        codeText.text = PhotonNetwork.CurrentRoom.Name;
        clientItems = new List<ClientSetupItem>();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        clientItems.Add(Instantiate(clientItemPrefab, itemParents));
        clientItems[clientItems.Count - 1].Setup(newPlayer.ActorNumber);
        clientItems[clientItems.Count - 1].gameObject.SetActive(true);

        StopReadyCount();
    }

    public void UpdatePlayerItems()
    {
        StopReadyCount();

        bool isAllReady=true;
        foreach (ClientSetupItem c in clientItems)
            if (c.UpdateItem()==false)
                isAllReady = false;

        if(isAllReady)
            c = StartCoroutine(AllIsReady());
    }
    const string prefix = "Starting in... ";
    IEnumerator AllIsReady()
    {
        readyTimerText.text = prefix+"5";
        yield return new WaitForSeconds(1);
        readyTimerText.text = prefix + "4";
        yield return new WaitForSeconds(1);
        readyTimerText.text = prefix + "3";
        yield return new WaitForSeconds(1);
        readyTimerText.text = prefix + "2";
        yield return new WaitForSeconds(1);
        readyTimerText.text = prefix + "1";
        yield return new WaitForSeconds(1);
        readyTimerText.text = prefix + "0";
        StartGame();
    }
    void StopReadyCount()
    {
        if (c == null)
            return;

        StopCoroutine(c);
        c = null;
        readyTimerText.text = "";
    }
    void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        pv.RPC("RPC_StartGame", RpcTarget.Others);

        GetComponentInParent<HostManager>().LoadScreen(-1);
    }

    [PunRPC] void RPC_StartGame()
    {
        GameManager.localClient.LoadScreen(-1);
    }
}
