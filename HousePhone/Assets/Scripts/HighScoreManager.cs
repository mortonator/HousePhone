using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public bool IsWebGL 
    {
        get
        {
            return Application.platform == RuntimePlatform.WebGLPlayer;
        }
    }

    public string GetHighScore_ForGame(string gameName)
    {
        return string.Empty;
    }
}
