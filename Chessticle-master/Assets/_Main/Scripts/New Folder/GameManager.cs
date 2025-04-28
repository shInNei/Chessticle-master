using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Board board;

    public PieceManager pieceManager;

    void Start()
    {
        board.Create();

        pieceManager.Setup(board);
    }

    public void BackMenu()
    {
        if (PieceManager.IAmode)
            pieceManager.stockfish.Close();
        SceneManager.LoadScene(0); // Menu
    }
    public void Reload()
    {
        pieceManager.ResetGame();
    }

    public void Reverse()
    {
        board.transform.localRotation *= Quaternion.Euler(180, 180, 0);
        foreach (List<Cell> row in board.allCells)
        {
            foreach (Cell boardCell in row)
            {
                if(boardCell.currentPiece != null)
                    boardCell.currentPiece.PlaceInit(boardCell);
            }
        }
    }
    public void Pause()
    {
        pieceManager.clockManager.StopClocks();
    }
    public void Continue()
    {
        pieceManager.clockManager.ContinueClocks();
    }
}

