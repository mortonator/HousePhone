using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostMenu : MonoBehaviour
{
    [SerializeField] Transform content;
    [SerializeField] HostGame_JO game_JO;
    [SerializeField] HostGame_CN_A game_CN_A;
    [SerializeField] HostGame_NT game_NT;

    const int gameCount = 3;
    int index;

    public void Setup()
    {
        gameObject.SetActive(false);
        index = 0;
        content.localPosition = Vector3.zero;

        UpdatePlayScreen();
    }
    public void GoToMenu()
    {
        gameObject.SetActive(true);

        if (index == 0)
            game_JO.gameObject.SetActive(false);
        else if (index == 1)
            game_CN_A.gameObject.SetActive(false);
    }

    public void PlayGame()
    {
        gameObject.SetActive(false);

        if (index == 0)
            game_JO.SetupGame();
        else if (index == 1)
            game_CN_A.SetupGame();
        else if (index == 2)
            game_NT.SetupGame();
    }
    public void LeftButton()
    {
        index--;
        if (index < 0)
            index = gameCount - 1;

        UpdatePlayScreen();
    }
    public void RightButton()
    {
        index++;
        if (index >= gameCount)
            index = 0;

        UpdatePlayScreen();
    }
    void UpdatePlayScreen() => content.localPosition = Vector3.left * ((1000 * index) + 500);

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
