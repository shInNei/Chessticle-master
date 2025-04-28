using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown ddTime;
    public TMP_Dropdown ddLevel;
    public TMP_Dropdown ddIASide;


    /// <summary>
    /// Launch game 1vs1 mode
    /// </summary>
    public void PlayGameLocal()
    {
        if (ddTime.value == 0)
        {
            PieceManager.whiteTime = 60;
            PieceManager.blackTime = 60;
        }
        if (ddTime.value == 1)
        {
            PieceManager.whiteTime = 300;
            PieceManager.blackTime = 300;
        }
        if (ddTime.value == 2)
        {
            PieceManager.whiteTime = 900;
            PieceManager.blackTime = 900;
        }
        if (ddTime.value == 3)
        {
            PieceManager.whiteTime = 3600;
            PieceManager.blackTime = 3600;
        }

        PieceManager.IAmode = false;
        SceneManager.LoadScene(2); // Game
    }
    public void PlayGameOnline()
    {
        SceneManager.LoadScene(1); // Game
    }

    /// <summary>
    /// Launch game player vs AI mode
    /// </summary>
    public void PlayIA()
    {
        PieceManager.IAmode = true;

        if (ddIASide.value == 0)
            PieceManager.isIAWithe = false;
        if (ddIASide.value == 1)
            PieceManager.isIAWithe = true;

        IA.level = IA.IA_Level[ddLevel.value];

        SceneManager.LoadScene(2); // Game
    }

    /// <summary>
    /// Exit application
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
