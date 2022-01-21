using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class ClientGame_CN_A : MonoBehaviourPunCallbacks // CodeNames Alliance
{
    PhotonView pv;
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject teamScreen;
    [SerializeField] GameObject clueScreen;
    [SerializeField] GameObject gridScreen;
    [SerializeField] GameObject selectionScreen;
    [Header("Clue")]
    [SerializeField] TMP_InputField clueInput;
    [SerializeField] TMP_Text clueCountText;
    [SerializeField] Button clueUpCountBtn;
    [SerializeField] Button clueDownCountBtn;
    [Header("Grid")]
    [SerializeField] CN_A_GridItem[] gridItems;
    [Header("Selection")]
    [SerializeField] Image selectImage;
    [SerializeField] Sprite unselectSpr, selectSpr;
    [SerializeField] TMP_Text selectText;
    [SerializeField] Button arrowUp, arrowDown, arrowLeft, arrowRight;

    int clueCount;

    string[] words;
    int[] keys;
    int x, y;
    bool selected;

    public void Setup()
    {
        pv = GetComponent<PhotonView>();
        gameObject.SetActive(false);
        Debug.Log("SetupGame");
    }
    public void SetupGame()
    {
        gameObject.SetActive(true);
        titleScreen.SetActive(true);
        teamScreen.SetActive(false);
        clueScreen.SetActive(false);
        gridScreen.SetActive(false);
        selectionScreen.SetActive(false);
    }


    public void ShowScreen(int screen)
    {
        titleScreen.SetActive(false);
        teamScreen.SetActive(false);
        clueScreen.SetActive(false);
        gridScreen.SetActive(false);
        selectionScreen.SetActive(false);

        if (screen == 0)
            titleScreen.SetActive(true);
        else if (screen == 1)
            teamScreen.SetActive(true);
        else if (screen == 2)
            clueScreen.SetActive(true);
        else if (screen == 3)
            selectionScreen.SetActive(true);
    }


    public void TeamA_Btn()
    {
        pv.RPC("RPC_TeamChoice", RpcTarget.MasterClient, pv.ControllerActorNr, true);
    }
    public void TeamB_Btn()
    {
        pv.RPC("RPC_TeamChoice", RpcTarget.MasterClient, pv.ControllerActorNr, false);
    }
    [PunRPC] void RPC_TeamChoice(int actor, bool teamA)
    {
        GameManager.Instance.hostManager.game_CN_A.TeamChoice(actor, teamA);
    }


    public void ClueUpCount()
    {
        clueCount--;
        if (clueCount < 0)
            clueCount = 0;
        if (clueCount == 0)
            clueUpCountBtn.interactable = false;

        clueDownCountBtn.interactable = true;
        clueCountText.text = clueCount.ToString();
    }
    public void ClueDownCount()
    {
        clueCount++;
        if (clueCount > 15)
            clueCount = 15;
        if (clueCount == 15)
            clueDownCountBtn.interactable = false;

        clueUpCountBtn.interactable = true;
        clueCountText.text = clueCount.ToString();
    }
    public void ClueConfirm()
    {
        pv.RPC("RPC_ClueConfirm", RpcTarget.MasterClient, pv.ControllerActorNr, clueInput.text + " " + clueCount);
        clueInput.text = string.Empty;
    }
    [PunRPC] void RPC_ClueConfirm(int actor, string clue_txt)
    {
        GameManager.Instance.hostManager.game_CN_A.ClueConfirm(actor, clue_txt);
    }


    public void LeaveGameButton()
    {
        pv.RPC("RPC_LeaveGame", RpcTarget.MasterClient, pv.OwnerActorNr);
    }
    [PunRPC] void RPC_LeaveGame(int act)
    {
        GameManager.Instance.hostManager.game_CN_A.LeaveGame();
    }


    public void SetWordsAndKeys(string[] words, int[] keys, bool teamA)
    {
        this.words = words;
        this.keys = keys;
        SetGrid(teamA);

        clueInput.text = string.Empty;
        clueCount = 1;
        clueCountText.text = "1";
        clueUpCountBtn.interactable = true;
        clueDownCountBtn.interactable = true;

        x = 1;
        y = 1;
        ArrowLeft();
        ArrowUp();

        selected = false;
        selectImage.sprite = unselectSpr;
    }
    void SetGrid(bool teamA)
    {
        int index;
        if (!teamA)
        {
            for (int i = 24; i >= 0; i--)
            {
                index = keys[i];
                gridItems[i].word.text = words[i];

                if (6 < index && index < 16)
                    gridItems[i].background.color = Color.green;
                else if (index == 1 || index == 17 || index == 25)
                    gridItems[i].background.color = Color.magenta;
                else
                    gridItems[i].background.color = Color.grey;
            }
        }
        else
        {
            for (int i = 24; i >= 0; i--)
            {
                index = keys[i];
                gridItems[i].word.text = words[i];

                if (index < 10)
                    gridItems[i].background.color = Color.green;
                else if (index == 15 || index == 16 || index == 17)
                    gridItems[i].background.color = Color.magenta;
                else
                    gridItems[i].background.color = Color.grey;
            }
        }
    }


    public void ArrowUp()
    {
        y--;
        selectText.text = words[(y * 5) + x];

        arrowDown.interactable = true;
        if (y == 0)
            arrowUp.interactable = false;
    }
    public void ArrowDown()
    {
        y++;
        selectText.text = words[(y * 5) + x];

        arrowUp.interactable = true;
        if (y == 4)
            arrowDown.interactable = false;
    }
    public void ArrowLeft()
    {
        x--;
        selectText.text = words[(y * 5) + x];

        arrowRight.interactable = true;
        if (x == 0)
            arrowLeft.interactable = false;
    }
    public void ArrowRight()
    {
        x++;
        selectText.text = words[(y * 5) + x];

        arrowLeft.interactable = true;
        if (x == 4)
            arrowRight.interactable = false;
    }
    public void ArrowSelect()
    {
        pv.RPC("RPC_SelectWord", RpcTarget.MasterClient, pv.ControllerActorNr, (y * 5) + x);
    }
    public void ArrowStop()
    {
        pv.RPC("RPC_SelectWord", RpcTarget.MasterClient, pv.ControllerActorNr, -1);
    }
    [PunRPC] void RPC_SelectWord(int _id, int wordIndex)
    {
        GameManager.Instance.hostManager.game_CN_A.SelectWord(_id, wordIndex);
    }
}
