using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class HostGame_CN_A : MonoBehaviourPunCallbacks // CodeNames Alliance
{
    [Header("Screens")]
    [SerializeField] GameObject setupScreen;
    [SerializeField] GameObject gameScreen;
    [SerializeField] GameObject endScreen;
    [SerializeField] TMP_Text endText;
    [Header("Setup")]
    [SerializeField] TMP_Text dif_txt;
    [SerializeField] Button playBtn;
    [Header("Teams")]
    [SerializeField] TMP_Text teamA_setup;
    [SerializeField] TMP_Text teamB_setup;
    [SerializeField] TMP_Text teamA_prefab;
    [SerializeField] TMP_Text teamB_prefab;
    [SerializeField] Transform teamA_parent;
    [SerializeField] Transform teamB_parent;
    [Header("Game")]
    [SerializeField] TMP_Text mistakes_txt;
    [SerializeField] TMP_Text task_txt;
    [SerializeField] TMP_Text action_txt;
    [SerializeField] TMP_Text teamClues_txt;
    [Header("Game Words")]
    [SerializeField] Color agentColour;
    [SerializeField] Color assassinColour;
    [SerializeField] Color citizenColour;
    [SerializeField] Image[] keyImages;
    [SerializeField] CN_A_GridItem[] gridItems;

    Dictionary<int, int> actorTeams;
    Dictionary<int, int> actorTeamTexts;
    List<TMP_Text> teamA_nameList;
    List<TMP_Text> teamB_nameList;
    List<string> selectedWordList;
    int[] keycardList;

    PhotonView pv;
    int mistakes, agentsFound;
    int teamAWordSelected, teamBWordSelected;
    [TextArea(500,500)]string teamAClue, teamBClue;
    bool client_isLocalATeam;

    void OnValidate()
    {
        if (keyImages.Length == 3)
        {
            keyImages[0].color = agentColour;
            keyImages[1].color = assassinColour;
            keyImages[2].color = citizenColour;

            for (int i = gridItems.Length - 1; i >= 0; i--)
                gridItems[i].background.color = citizenColour;
        }
    }
    public void Setup()
    {
        pv = GetComponent<PhotonView>();
        gameObject.SetActive(false);

        if (keyImages.Length == 4)
        {
            keyImages[0].color = agentColour;
            keyImages[1].color = assassinColour;
            keyImages[2].color = citizenColour;
            keyImages[3].color = citizenColour;
        }
    }
    public void SetupGame()
    {
        gameObject.SetActive(true);
        setupScreen.SetActive(true);
        gameScreen.SetActive(false);
        endScreen.SetActive(false);

        teamA_nameList = new List<TMP_Text>();
        teamB_nameList = new List<TMP_Text>();

        actorTeams = new Dictionary<int, int>();
        actorTeamTexts = new Dictionary<int, int>();

        foreach (var p in GameManager.Instance.clientManagers.Values)
        {
            actorTeams.Add(p.pv.ControllerActorNr, 0);
            actorTeamTexts.Add(p.pv.ControllerActorNr, -1);
        }

        pv.RPC("Load_CN_A", RpcTarget.Others);
    }
    public void RandomiseTeams()
    {
        int teamACount = 0;
        int teamBCount = 0;
        int t;
        foreach (var p in GameManager.Instance.clientManagers.Values)
        {
            if (teamACount == teamBCount)
                t = Random.Range(0, 2);
            else if (teamACount > teamBCount)
                t = 2;
            else
                t = 1;

            if (t == 0)
            {
                actorTeams[p.pv.ControllerActorNr] = 1;
                teamACount++;
            }
            else if (t == 1)
            {
                actorTeams[p.pv.ControllerActorNr] = 2;
                teamBCount++;
            }
        }

        AssignNames();
    }
    void AssignNames()
    {
        //set play button as active until a player is found without a team.
        playBtn.interactable = true;

        string teamAStr = "";
        string teamBStr = "";

        int aIndex = 0;
        int bIndex = 0;
        foreach (var p in GameManager.Instance.clientManagers.Values)
        {
            if (actorTeams[p.pv.ControllerActorNr] == 1)
            {
                //if need more in the list, add one
                if (teamA_nameList.Count <= aIndex)
                    teamA_nameList.Add(Instantiate(teamA_prefab, teamA_parent));

                teamA_nameList[aIndex].gameObject.SetActive(true);
                teamA_nameList[aIndex].text = p.setup.inputName;
                actorTeamTexts[p.pv.ControllerActorNr] = aIndex;

                teamAStr += p.setup.inputName + "\n";
                aIndex++;
            }
            else if (actorTeams[p.pv.ControllerActorNr] == 2)
            {
                //if need more in the list, add one
                if (teamB_nameList.Count <= bIndex)
                    teamB_nameList.Add(Instantiate(teamB_prefab, teamB_parent));

                teamB_nameList[bIndex].gameObject.SetActive(true);
                teamB_nameList[bIndex].text = p.setup.inputName;
                actorTeamTexts[p.pv.ControllerActorNr] = bIndex;

                teamBStr += p.setup.inputName + "\n";
                bIndex++;
            }
            else if (actorTeams[p.pv.ControllerActorNr] == 0)
            {
                playBtn.interactable = false;
            }
        }

        teamA_setup.text = teamAStr;
        teamB_setup.text = teamBStr;

        //go through unused TMP_Texts and deactivate them.
        for (; aIndex < teamA_nameList.Count; aIndex++)
            teamA_nameList[aIndex].gameObject.SetActive(false);
        for (; bIndex < teamB_nameList.Count; bIndex++)
            teamB_nameList[bIndex].gameObject.SetActive(false);
    }
    [PunRPC] void Load_CN_A()
    {
        GameManager.localClient.LoadScreen(1);
        GameManager.localClient.game_CN_A.ShowScreen(1);
    }


    public void StartGame()
    {
        setupScreen.SetActive(false);
        gameScreen.SetActive(true);

        agentsFound = 0;
        GetWordList();

        foreach (TMP_Text item in teamA_nameList)
            item.color = Color.grey;
        foreach (TMP_Text item in teamB_nameList)
            item.color = Color.grey;

        action_txt.text = "Game Starts";

        pv.RPC("SetWordsAndKeys", RpcTarget.Others, selectedWordList, keycardList, actorTeams);
        RequestClue();
    }
    public void GetWordList()
    {
        selectedWordList = ListManager.Instance.GetRandomWordList(25);
        keycardList = new int[25];

        string debugList = "";
        int k;
        for (int i = 24; i >= 0; i--)
        {
            gridItems[i].word.text = selectedWordList[i];
            gridItems[i].background.color = citizenColour;

            do
            {
                k = Random.Range(1, 26);
            } while (keycardList.Contains(k));

            keycardList[i] = k;
            debugList += k.ToString() + ", ";
        }
        Debug.Log(debugList);
    }
    [PunRPC] void SetWordsAndKeys(string[] words, int[] keys, Dictionary<int, int> teams)
    {
        client_isLocalATeam = teams[PhotonNetwork.LocalPlayer.ActorNumber] == 1;
        GameManager.localClient.game_CN_A.SetWordsAndKeys(words, keys, client_isLocalATeam);
    }


    void RequestClue()
    {
        teamAClue = string.Empty;
        teamBClue = string.Empty;

        teamAWordSelected = -1;
        teamBWordSelected = -1;

        task_txt.text = "Task:\nEnter a clue with a number of connections. One clue per team.";
        pv.RPC("RPC_ShowScreen", RpcTarget.Others, 2);
    }
    [PunRPC] void RPC_ShowScreen(int s)
    {
        GameManager.localClient.game_CN_A.ShowScreen(s);
    }


    public void TeamChoice(int actor, bool teamA)
    {
        actorTeams[actor] = teamA ? 1 : 2;
        AssignNames();
    }
    public void ChangeDifficulty(float i)
    {
        if (i == 0)
        {
            dif_txt.text = "Mistakes: 12";
            mistakes = 12;
        }
        else if (i == 1)
        {
            dif_txt.text = "Mistakes: 10";
            mistakes = 10;
        }
        else if (i == 2)
        {
            dif_txt.text = "Mistakes: 8";
            mistakes = 8;
        }
    }


    public void ClueConfirm(int actor, string clue_txt)
    {
        if (actorTeams[actor] == 1)
        {
            teamA_nameList[actorTeamTexts[actor]].color = Color.white;
            teamAClue = clue_txt;

            if (!string.IsNullOrWhiteSpace(teamBClue))
                AllCluesRecieved();
            else
                pv.RPC("RPC_ClueRecieved", RpcTarget.Others, true);

            action_txt.text = $"{GameManager.Instance.clientManagers[actor].setup.inputName} has given a clue from Team A.";
        }
        else
        {
            teamB_nameList[actorTeamTexts[actor]].color = Color.white;
            teamBClue = clue_txt;

            if (!string.IsNullOrWhiteSpace(teamAClue))
                AllCluesRecieved();
            else
                pv.RPC("RPC_ClueRecieved", RpcTarget.Others, false);

            action_txt.text = $"{GameManager.Instance.clientManagers[actor].setup.inputName} has given a clue from Team B.";
        }
    }
    [PunRPC] void RPC_ClueRecieved(bool teamA)
    {
        if (teamA == client_isLocalATeam) // if teamA and is teamA, or teamB and is teamB
        {
            GameManager.localClient.game_CN_A.ShowScreen(0);
        }
    }
    void AllCluesRecieved()
    {
        teamClues_txt.text = "Clues:\nA= " + teamAClue + "\nB= " + teamBClue;
        task_txt.text = "Task:\nUse the clue given to figure out who are you agents.";

        pv.RPC("RPC_ShowScreen", RpcTarget.Others, 3);
    }


    public void LeaveGame()
    {
        GameManager.Instance.hostManager.menu.GoToMenu();
        pv.RPC("RPC_QuitGame", RpcTarget.Others);
    }
    [PunRPC] void RPC_QuitGame() => GameManager.localClient.LoadScreen(-1);


    public void SelectWord(int _id, int wordIndex)
    {
        if (actorTeams[_id] == 1)
        {
            teamAWordSelected = wordIndex;

            if (teamAWordSelected == -1)
            {
                pv.RPC("RPC_ClueRecieved", RpcTarget.Others, true);
            }
            else
            {
                action_txt.text = $"{GameManager.Instance.clientManagers[_id].setup.inputName} from Team A has guessed the word {selectedWordList[wordIndex]}";
                CheckWord(CheckWordSelected(true, teamAWordSelected));
            }
        }
        else
        {
            teamBWordSelected = wordIndex;

            if (teamBWordSelected == -2)
            {
                pv.RPC("RPC_ClueRecieved", RpcTarget.Others, false);
            }
            else if (teamBWordSelected != -1)
            {
                CheckWord(CheckWordSelected(false, teamBWordSelected));
                action_txt.text = $"{GameManager.Instance.clientManagers[_id].setup.inputName} from Team B has guessed the word {selectedWordList[wordIndex]}";
            }
        }

        if (teamAWordSelected == -2 && teamBWordSelected == -2)
            RequestClue();
    }
    void CheckWord(bool hasLose)
    {
        if (hasLose)
            GameOver();

        if (agentsFound == 15)
            GameWon();
    }
    bool CheckWordSelected(bool teamA, int index)
    {
        int isAgent = CheckIfWordIsAgent(teamA, index);

        if (isAgent == 1)
        {
            //agent
            if (gridItems[index].background.color != agentColour)
                agentsFound++;

            if (gridItems[index].background.color != assassinColour)
                gridItems[index].background.color = agentColour;

            return false;
        }
        else if (isAgent == 2)
        {
            //assassin
            gridItems[index].background.color = assassinColour;
            return true;
        }
        else
        {
            //civilian
            gridItems[index].team.text = teamA ? "A" : "B";

            mistakes--;
            mistakes_txt.text = "Mistakes Left: "+ mistakes.ToString("00");

            if (mistakes <= 0)
                return true;
            else
                return false;
        }
    }
    int CheckIfWordIsAgent(bool teamA, int index)
    {
        Debug.Log($"Check word {selectedWordList[index]} for teamA {teamA.ToString()} with index of {index} and key of {keycardList[index]}");
        index = keycardList[index];

        if (teamA)
        {

            if (6 < index && index < 16)
                return 1;
            else if (index == 1 || index == 17 || index == 25)
                return 2;
            else
                return 0;
        }
        else
        {
            if (index < 10)
                return 1;
            else if (index == 15 || index == 16 || index == 17)
                return 2;
            else
                return 0;
        }
    }


    void GameOver()
    {
        endScreen.SetActive(true);
        pv.RPC("RPC_ShowScreen", RpcTarget.Others, 0);

        if (mistakes == 0)
            endText.text = "Game Over!\nToo many mistakes were made, the assassin found you.";
        else
            endText.text = "Game Over!\nAssassin was uncovered, the alliance was lost.";
    }
    void GameWon()
    {
        endScreen.SetActive(true);
        pv.RPC("RPC_ShowScreen", RpcTarget.Others, 0);

        endText.text = "Congratulations!\nThe alliance was successful.";
    }
}