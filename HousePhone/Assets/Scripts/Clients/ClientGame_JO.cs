using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class ClientGame_JO : MonoBehaviourPunCallbacks // JustOne
{
    [SerializeField] GameObject titleScreen;
    [Header("Guess")]
    [SerializeField] GameObject guessCheck;
    [SerializeField] TMP_InputField guessField;
    [SerializeField] Button guessButton;
    [Header("Hint")]
    [SerializeField] GameObject hintScreen;
    [SerializeField] TMP_Text targetWord;
    [SerializeField] TMP_InputField hintField;
    [Header("Check Hints")]
    [SerializeField] GameObject checkScreen;
    [SerializeField] TMP_Text yourHints;
    [SerializeField] TMP_Text otherHints;
    [Header("Check Guess")]
    [SerializeField] GameObject checkGuessScreen;

    public string hintWord { get; protected set; }
    PhotonView pv;

    public void Setup()
    {
        pv = GetComponent<PhotonView>();
        gameObject.SetActive(false);
    }
    public void SetupGame()
    {
        gameObject.SetActive(true);
        titleScreen.SetActive(true);

        hintScreen.SetActive(false);
        guessCheck.SetActive(false);
        checkScreen.SetActive(false);
    }

    public void StartRound(string word)
    {
        targetWord.text = word;
        hintField.text = string.Empty;

        hintScreen.SetActive(true);

        titleScreen.SetActive(false);
        guessCheck.SetActive(false);
        checkScreen.SetActive(false);
        checkGuessScreen.SetActive(false);
    }
    public void StartRound_Guesser()
    {
        titleScreen.SetActive(true);
        guessField.text = string.Empty;

        guessCheck.SetActive(false);
        hintScreen.SetActive(false);
        checkScreen.SetActive(false);
        checkGuessScreen.SetActive(false);
    }
    public void EnterHint()
    {
        hintScreen.SetActive(false);
        titleScreen.SetActive(true);

        pv.RPC("RPC_EnterHint", RpcTarget.MasterClient, pv.OwnerActorNr, hintField.text);
    }
    [PunRPC] void RPC_EnterHint(int act, string word)
    {
        hintWord = word;
        GameManager.Instance.hostManager.game_JO.ClientSentWord(act);
    }

    public void CheckHints(int[] acts, string[] hints)
    {
        string otherHintsStr = "";
        for (int i = 0; i < acts.Length; i++)
        {
            if (acts[i] != pv.ControllerActorNr)
                otherHintsStr += hints[i] + ", ";
        }

        otherHintsStr = otherHintsStr.Remove(otherHintsStr.Length - 2, 2);
        otherHints.text = "Other hints: \n"+ otherHintsStr;
        yourHints.text = "Your hint: \n" + hintField.text;

        titleScreen.SetActive(false);
        checkScreen.SetActive(true);
    }

    public void AllowHint(bool allow)
    {
        checkScreen.SetActive(false);
        titleScreen.SetActive(true);

        pv.RPC("RPC_CheckedHint", RpcTarget.MasterClient, pv.ControllerActorNr, allow);
    }
    [PunRPC] void RPC_CheckedHint(int act, bool allow) => GameManager.Instance.hostManager.game_JO.CheckedHints(act, allow);
    public void AllowGuess()
    {
        titleScreen.SetActive(false);
        guessCheck.SetActive(true);

        guessButton.interactable = true;
    }
    public void SendGuess()
    {
        guessButton.interactable = false;
        pv.RPC("RPC_SendGuess", RpcTarget.MasterClient, guessField.text);
    }
    [PunRPC] void RPC_SendGuess(string guess) => GameManager.Instance.hostManager.game_JO.GuessAttempt(guess);


    public void CheckGuess()
    {
        guessCheck.SetActive(false);
        checkGuessScreen.SetActive(true);
    }
    public void GuessCorrect(int i)
    {
        checkGuessScreen.SetActive(false);
        titleScreen.SetActive(true);

        pv.RPC("RPC_GuessCorrect", RpcTarget.MasterClient, i);
    }
    [PunRPC] void RPC_GuessCorrect(int i) => GameManager.Instance.hostManager.game_JO.GuessCorrect(i);
}
