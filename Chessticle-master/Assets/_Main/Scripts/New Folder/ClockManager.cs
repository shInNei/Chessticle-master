using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{
    
    [HideInInspector]
    public bool launched = false;

    private Timer clockWhite;
    private Timer clockBlack;

    public TMP_Text displayWhite;
    public TMP_Text displayBlack;

    public GameObject highlightClockW;
    public GameObject highlightClockB;

    private PieceManager pm = null;

    private bool isWhiteTurn = true;

    /// <summary>
    /// Init black and white clocks
    /// </summary>
    /// <param name="whiteTime"></param>
    /// <param name="blackTime"></param>
    /// <param name="newPm"></param>
    public void Setup(float whiteTime, float blackTime, PieceManager newPm)
    {

        pm = newPm;
        launched = false;
        clockWhite = new Timer();
        clockBlack = new Timer();

        clockWhite.Setup(whiteTime, displayWhite);
        clockBlack.Setup(blackTime, displayBlack);

        highlightClockW.SetActive(true);
        highlightClockB.SetActive(false);

        highlightClockW.GetComponent<Image>().color = new Color((float)0.1490196, (float)0.1607843, (float)0.2588235, 1);
        highlightClockB.GetComponent<Image>().color = new Color((float)0.1490196, (float)0.1607843, (float)0.2588235, 1);


    }

    /// <summary>
    /// Start clocks
    /// </summary>
    public void StartClocks()
    {
        clockWhite.Start();
        clockBlack.Start();
        launched = true;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (launched)
        {
            if (isWhiteTurn == true)
            {
                clockWhite.Update();
            }
            else
            {
                clockBlack.Update();
            }
            if (clockBlack.runOut)
            {
                pm.gameState = GameState.WHITE_WIN;
                pm.ShowResult();
                launched = false;
            }
            if (clockWhite.runOut)
            {
                pm.gameState = GameState.BLACK_WIN;
                pm.ShowResult();
                launched = false;
            }
        }       
    }

    public void StopClocks()
    {
        clockWhite.Stop();
        clockBlack.Stop();        
    }
    public void ContinueClocks()
    {
        clockWhite.Continue();
        clockBlack.Continue();

    }

    public void changeTurn()
    {
        isWhiteTurn = !isWhiteTurn;
        highlightClockW.SetActive(!highlightClockW.activeSelf);
        highlightClockB.SetActive(!highlightClockB.activeSelf);

    }

    public void setTurn(bool isWhiteTurn)
    {
        this.isWhiteTurn = isWhiteTurn;
        highlightClockW.SetActive(isWhiteTurn);
        highlightClockB.SetActive(!isWhiteTurn);
    }
}
