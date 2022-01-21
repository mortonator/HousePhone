using Photon.Pun;
using SimpleLottery;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class HostGame_JO : MonoBehaviourPunCallbacks // NoThanks
{
    [SerializeField] PhotonView pv;
    [SerializeField] AudioSource snd_TimerTing;
    [Header("Setup")]
    [SerializeField] GameObject setupScreen;
    [SerializeField] GameObject gameScreen;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text sliderTxt;
    [Header("Tutorial")]
    [SerializeField] GameObject tutortialScreen;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI tutorialText_numOfRound;
    [Header("Guesser")]
    [SerializeField] TextMeshProUGUI guesserText;
    [SerializeField] GameObject answerScreen;
    [SerializeField] TextMeshProUGUI answerText;
    [Header("Hinters")]
    [SerializeField] Sprite tick_spr;
    [SerializeField] Sprite cross_spr;
    [SerializeField] Transform itemParent;
    [SerializeField] GameObject itemPrefab;
    [Header("Hints")]
    [SerializeField] Transform hintParent;
    [SerializeField] TextMeshProUGUI hintText;
    [Header("Timers")]
    [SerializeField] GameObject timer30;
    [SerializeField] Image timer30Bar;
    [SerializeField] GameObject timer15;
    [SerializeField] Image timer15Bar;
    [Header("End Game")]
    [SerializeField] GameObject endScreen;
    [SerializeField] TextMeshProUGUI endScore;
    [SerializeField] TextMeshProUGUI replayText;

    ClientManager guesser;
    List<ClientManager> hinters;
    Dictionary<int, GameObject> hinterObjects;
    List<int> previousGuessers;
    List<string> hints;
    List<string> selectedWordList;
    int round, hintsGiven, score, maxRounds, bell_milestone;
    float timer;
    bool longTimer = true, tutorialed = false, pauseTimer=false;

    public void Setup()
    {
        gameObject.SetActive(false);
    }
    public void SetupGame()
    {
        gameObject.SetActive(true);
        setupScreen.SetActive(true);
        gameScreen.SetActive(false);

        slider.value = 3;
        SetupSlider(3);
    }
    public void SetupSlider(float v)
    {
        sliderTxt.text = "Rounds: " + (int)v;
    }
    public void StartGame()
    {
        maxRounds = (int)slider.value * GameManager.Instance.clientManagers.Count;
        tutorialText_numOfRound.text = $"The game will last for {maxRounds} rounds, so each player will have {(int)slider.value} attempts at guessing.\nHow close can you get to the max score of {maxRounds} as a team?";

        setupScreen.SetActive(false);
        gameScreen.SetActive(true);
        endScreen.SetActive(false);
        tutortialScreen.SetActive(true);

        pv.RPC("Load_JO", RpcTarget.Others);

        tutorialed = false;
        longTimer = true;
        timer15.SetActive(false);
        timer30.SetActive(true);
        timer = 30; bell_milestone = 30;
    }
    [PunRPC] void Load_JO()
    {
        GameManager.localClient.LoadScreen(0);
    }
    public void EndTutorialTimer()
    {
        tutortialScreen.SetActive(false);
        endScreen.SetActive(false);

        hinters = new List<ClientManager>();
        previousGuessers = new List<int>();
        tutorialed = true;
        pauseTimer = false;

        BuildWordList();
        round = -1;
        StartRound();
    }


    void BuildWordList()
    {
        score = 0;
        selectedWordList = ListManager.Instance.GetRandomWordList(maxRounds);
        scoreText.text = score + "/" + maxRounds;
    }
    void StartRound()
    {
        round++;
        if (round >= maxRounds)
        {
            EndGame();
            return;
        }

        hints = new List<string>();
        answerScreen.SetActive(false);

        GenerateGuesser();
        ShowPlayers();
        pv.RPC("RPC_Others_StartGuessing", RpcTarget.Others, guesser.pv.ControllerActorNr, selectedWordList[round]);

        longTimer = true;
        timer15.SetActive(false);
        timer30.SetActive(true);
        timer = 30; bell_milestone = 30;
    }
    void GenerateGuesser()
    {
        if (previousGuessers.Count == hinters.Count)
        {
            previousGuessers = new List<int>();
        }

        hinters = GameManager.Instance.clientManagers.Values.ToList();
        hinters.Shuffle();

        int i = -1;
        do
        {
            i++;
            guesser = hinters[i];
        } while (previousGuessers.Contains(guesser.pv.OwnerActorNr));

        previousGuessers.Add(guesser.pv.ControllerActorNr);
        hinters.RemoveAt(i);
    }
    void ShowPlayers()
    {
        guesserText.text = guesser.setup.inputName;

        GameObject item;
        hinterObjects = new Dictionary<int, GameObject>();

        for (int i = itemParent.childCount - 1; i > 0; i--)
        {
            Destroy(itemParent.GetChild(i).gameObject);
        }
        for (int i = hintParent.childCount - 1; i > 0; i--)
        {
            Destroy(hintParent.GetChild(i).gameObject);
        }

        foreach (ClientManager man in hinters)
        {
            item = Instantiate(itemPrefab, itemParent);
            item.SetActive(true);
            item.GetComponentInChildren<TextMeshProUGUI>().text = man.setup.inputName;
            hinterObjects.Add(man.pv.ControllerActorNr, item);
        }
    }
    [PunRPC] void RPC_Others_StartGuessing(int guesserAN, string w)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == guesserAN)
            GameManager.localClient.game_JO.StartRound_Guesser();
        else
            GameManager.localClient.game_JO.StartRound(w);
    }


    public void PauseTimer()
    {
        pauseTimer = !pauseTimer;
    }
    void Update()
    {
        if (!pauseTimer && timer > 0)
        {
            timer -= Time.deltaTime;

            if (longTimer)
            {
                timer30Bar.fillAmount = timer / 30f;
                if (timer <= 0)
                {
                    if (tutorialed)
                        EndHintingTimer();
                    else
                        EndTutorialTimer();
                }
            }
            else
            {
                timer15Bar.fillAmount = timer / 15f;
                if (timer <= 0)
                    EndCheckHints();
            }

            if (timer <= bell_milestone)
            {
                bell_milestone -= 5;
                snd_TimerTing.Play();
            }
        }
    }
    public void ClientSentWord(int act)
    {
        hinterObjects[act].GetComponentInChildren<Image>().sprite = tick_spr;
        hintsGiven++;

        if (hintsGiven == GameManager.Instance.clientManagers.Count - 1)
            EndHintingTimer();
    }



    void EndHintingTimer()
    {
        int[] actNums = new int[GameManager.Instance.clientManagers.Count - 1];
        string[] hints = new string[GameManager.Instance.clientManagers.Count - 1];
        int i = 0;
        foreach (var c in GameManager.Instance.clientManagers.Values)
        {
            if (c == guesser)
                continue;

            actNums[i] = c.pv.ControllerActorNr;
            hints[i] = c.game_JO.hintWord;
            i++;
        }
        foreach (GameObject item in hinterObjects.Values)
        {
            item.GetComponentInChildren<Image>().sprite = cross_spr;
        }

        hintsGiven = 0;
        pv.RPC("RPC_CheckHints", RpcTarget.Others, guesser.pv.ControllerActorNr, actNums, hints);

        longTimer = false;
        timer15.SetActive(true);
        timer30.SetActive(false);
        timer = 15; bell_milestone = 15;
    }
    [PunRPC] void RPC_CheckHints(int g, int[] acts, string[] hints)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == g)
            return;

        GameManager.localClient.game_JO.CheckHints(acts, hints);
    }
    public void CheckedHints(int act, bool allow)
    {
        hinterObjects[act].GetComponentInChildren<Image>().sprite = tick_spr;
        hintsGiven++;

        if (allow)
            hints.Add(GameManager.Instance.clientManagers[act].game_JO.hintWord);

        if (hintsGiven == GameManager.Instance.clientManagers.Count - 1)
            EndCheckHints();
    }
    void EndCheckHints()
    {
        hintsGiven = 0;
        timer = 0; bell_milestone = 0;
        ShowHints();
        pv.RPC("RPC_AllowGuesser", RpcTarget.Others, guesser.pv.OwnerActorNr);
    }



    void ShowHints()
    {
        TextMeshProUGUI txt;

        for (int i = hintParent.childCount - 1; i > 0; i--)
        {
            Destroy(hintParent.GetChild(i).gameObject);
        }

        foreach (string h in hints)
        {
            txt = Instantiate(hintText, hintParent);
            txt.text = h;
            txt.gameObject.SetActive(true);
        }
    }
    [PunRPC] void RPC_AllowGuesser(int guesser)
    {
        if (guesser == PhotonNetwork.LocalPlayer.ActorNumber)
            GameManager.localClient.game_JO.AllowGuess();
    }
    public void GuessAttempt(string guess)
    {
        answerScreen.SetActive(true);

        if (string.IsNullOrWhiteSpace(guess))
            answerText.text = $"Your Answer: [skip] \n\nThe correct Answer: {selectedWordList[round]}";
        else
            answerText.text = $"Your Answer: {guess} \n\nThe correct Answer: {selectedWordList[round]}";

        pv.RPC("RPC_GuessCheck", RpcTarget.Others, guesser.pv.ControllerActorNr);
    }
    [PunRPC] void RPC_GuessCheck(int act)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == act)
            GameManager.localClient.game_JO.CheckGuess();
    }
    public void GuessCorrect(int cor)
    {
        Debug.Log("GuessCorrect: " + cor);
        score += cor;
        scoreText.text = score + "/" + maxRounds;
        StartRound();
    }


    void EndGame()
    {
        endScreen.SetActive(true);
        answerScreen.SetActive(false);
        endScore.text = score + "/" + maxRounds;

        SetReplayText();
    }
    void SetReplayText()
    {
        string s;
        float perc = (float)score / maxRounds;

        if (perc == 1)
            s = "Perfect score!Can you do it again?";
        else if (perc >= 0.8f)
            s = "Incredible! Your friends must be impressed!";
        else if (perc >= 0.66f)
            s = "Awesome! That's a score worth celebrating!";
        else if (perc >= 0.5f)
            s = "Wow, not bad at all!";
        else if (perc >= 0.4f)
            s = "You're in the average. Can you do better?";
        else if (perc >= 0.25f)
            s = "That's a good start. Try again!";
        else if (perc > 0)
            s = "Try again, and again, and again.";
        else
            s = "Umm.......... maybe have another go?";

        replayText.text = s;
    }
    public void QuitGame()
    {
        GameManager.Instance.hostManager.menu.GoToMenu();
        pv.RPC("RPC_QuitGame", RpcTarget.Others);
    }
    [PunRPC] void RPC_QuitGame() => GameManager.localClient.LoadScreen(-1);
}