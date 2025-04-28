using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum GameState
{
    INGAME,
    WHITE_WIN,
    BLACK_WIN,
    PAT,
    NULLE
}

public class PieceManager : MonoBehaviour
{
    [HideInInspector]
    public bool isKingAlive;

    private Board chessBoard;

    [HideInInspector]
    public GameState gameState;

    [HideInInspector]
    public Theme theme = null;

    [HideInInspector]
    public new AudioSource audio;

    public ClockManager clockManager;

    public GameObject piecePrefab;
    public TMP_Text result;

    private List<BasePiece> whitePieces = null;
    private List<BasePiece> blackPieces = null;

    public static float blackTime = 60;
    public static float whiteTime = 60;

    public static bool IAmode = true;
    public static bool isIAWithe = false;
    public IA stockfish = null;
    [HideInInspector]
    public bool IATurn = false;

    [HideInInspector]
    public Cell enPassantCell = null;
   

    [HideInInspector]
    public King whiteKing = null;
    [HideInInspector]
    public King blackKing = null;
    [HideInInspector]
    public bool checkVerificationInProcess = false;

    private string[] pieceOrder = { "P", "P", "P", "P", "P", "P", "P", "P",
        "R", "KN", "B", "Q", "K", "B", "KN", "R" };


    private Dictionary<string, Type> pieceDico = new Dictionary<string, Type>()
    {
        {"P", typeof(Pawn)},
        {"R", typeof(Rook)},
        {"KN", typeof(Knight)},
        {"B", typeof(Bishop)},
        {"K", typeof(King)},
        {"Q", typeof(Queen)}
    };

    public Dictionary<string, int> coordA = new Dictionary<string, int>()
    {
        {"a", 0},
        {"b", 1},
        {"c", 2},
        {"d", 3},
        {"e", 4},
        {"f", 5},
        {"g", 6},
        {"h", 7}
    };

    public Dictionary<string, int> coordB = new Dictionary<string, int>()
    {
        {"1", 0},
        {"2", 1},
        {"3", 2},
        {"4", 3},
        {"5", 4},
        {"6", 5},
        {"7", 6},
        {"8", 7}
    };

    public Dictionary<int, string> posA = new Dictionary<int, string>()
    {
        {0, "a"},
        {1, "b"},
        {2, "c"},
        {3, "d"},
        {4, "e"},
        {5, "f"},
        {6, "g"},
        {7, "h"},
    };

    public Dictionary<int, string> posB = new Dictionary<int, string>()
    {
        {0, "1"},
        {1, "2"},
        {2, "3"},
        {3, "4"},
        {4, "5"},
        {5, "6"},
        {6, "7"},
        {7, "8"},
    };

    /// <summary>
    /// init chess piece on the board
    /// </summary>
    /// <param name="board"></param>
    public void Setup(Board board)
    {
        audio = gameObject.AddComponent<AudioSource>();

        chessBoard = board;
        gameState = GameState.INGAME;

        result.text = "";

        isKingAlive = true;

        whitePieces = CreatePieces(true, chessBoard);
        blackPieces = CreatePieces(false, chessBoard);

        PlacePieces("2", "1", whitePieces, board);
        PlacePieces("7", "8", blackPieces, board);

        SetInteractive(whitePieces, false);
        SetInteractive(blackPieces, false);

        enPassantCell = null;
        checkVerificationInProcess = false;
        clockManager.Setup(whiteTime, blackTime, this);

        if (IAmode){
            stockfish.Setup();
            if (isIAWithe)
            {
                StartCoroutine(showIAMoveCoroutine());
                clockManager.displayBlack.text = "Player";
                clockManager.displayWhite.text = "AI " + IA.IA_Game_Level[IA.level];
            }
            else
            {
                SetInteractive(whitePieces, true);
                clockManager.displayWhite.text = "Player";
                clockManager.displayBlack.text = "AI " + IA.IA_Game_Level[IA.level];
            }
        }
        else
        {
            SetInteractive(whitePieces, true);            
        }

        
    }

    /// <summary>
    /// Put piece at original position
    /// </summary>
    public void ResetGame()
    {
        if (IATurn)
            return;
        gameState = GameState.INGAME;

        result.text = "";

        foreach (List<Cell> row in chessBoard.allCells)
        {
            foreach (Cell boardCell in row)
            {
                boardCell.outlineImage.enabled = false;
                if (boardCell.currentPiece != null)
                {
                    boardCell.currentPiece.Kill();
                }
                boardCell.enPassant = null;
            }
        }

        whitePieces.Clear();
        blackPieces.Clear();

        whitePieces = CreatePieces(true, chessBoard);
        blackPieces = CreatePieces(false, chessBoard);

       
            
        PlacePieces("2", "1", whitePieces, chessBoard);
        PlacePieces("7", "8", blackPieces, chessBoard);

        SetInteractive(whitePieces, false);
        SetInteractive(blackPieces, false);

        enPassantCell = null;
        isKingAlive = true;

        clockManager.Setup(whiteTime, blackTime, this);

        checkVerificationInProcess = false;

        if (IAmode)
        {
            stockfish.Close();
            stockfish.Setup();
            if (isIAWithe)
            {
                StartCoroutine(showIAMoveCoroutine());
                clockManager.displayBlack.text = "Player";
                clockManager.displayWhite.text = "AI " + IA.IA_Game_Level[IA.level];
            }
            else
            {
                SetInteractive(whitePieces, true);
                clockManager.displayWhite.text = "Player";
                clockManager.displayBlack.text = "AI " + IA.IA_Game_Level[IA.level];
            }
        }
        else
        {
            SetInteractive(whitePieces, true);
        }
    }

    /// <summary>
    /// Create piece list
    /// </summary>
    /// <param name="isWhite"></param>
    /// <param name="board"></param>
    /// <returns></returns>
    private List<BasePiece> CreatePieces(bool isWhite, Board board)
    {
        List<BasePiece> pieceList = new List<BasePiece>();

        float board_width = board.GetComponent<RectTransform>().rect.width;
        float board_height = board.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < pieceOrder.Length; i++)
        {
            GameObject newPieceObject = Instantiate(piecePrefab);
            newPieceObject.transform.SetParent(transform);

            newPieceObject.transform.localScale = new Vector3(1, 1, 1);
            newPieceObject.transform.localRotation = Quaternion.identity;

            
            float piece_width = board_width / board.Column - BasePiece.CellPadding;
            float piece_height = board_height / board.Row - BasePiece.CellPadding;
            newPieceObject.GetComponent<RectTransform>().sizeDelta = new Vector2(piece_width, piece_height);
            

            string key = pieceOrder[i];
            Type pieceType = pieceDico[key];

            BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);
            pieceList.Add(newPiece);

            if (pieceDico[key] == typeof(King))
            {
                if (isWhite)
                    whiteKing = (King) newPiece;
                else
                    blackKing = (King) newPiece;
            }

            newPiece.Setup(isWhite, this);
        }

        return pieceList;
    }
        
    /// <summary>
    /// Place piece on the board
    /// </summary>
    /// <param name="pawnRow"></param>
    /// <param name="royaltyRow"></param>
    /// <param name="pieces"></param>
    /// <param name="board"></param>
    private void PlacePieces(string pawnRow, string royaltyRow, List<BasePiece> pieces, Board board)
    {
        for (int i = 0; i < board.Column; i++)
        {
            pieces[i].PlaceInit(board.allCells[i][coordB[pawnRow]]);
            pieces[i + 8].PlaceInit(board.allCells[i][coordB[royaltyRow]]);
        }
    }

    /// <summary>
    /// Set piece draggable
    /// </summary>
    /// <param name="pieces"></param>
    /// <param name="state"></param>
    private void SetInteractive(List<BasePiece> pieces, bool state)
    {
        foreach(BasePiece piece in pieces)
        {
            if (piece.inDrag)
                piece.OnEndDrag(null);
            piece.enabled = state;
        }
    }

    /// <summary>
    /// Set turn to change turn
    /// </summary>
    /// <param name="isWhiteTurn"></param>
    public void SetTurn(bool isWhiteTurn)
    {
        if (IAmode)
        {
            clockManager.setTurn(isWhiteTurn);
            SetInteractive(whitePieces, false);
            SetInteractive(blackPieces, false);
            StartCoroutine(showIAMoveCoroutine());
        }
        else
        {
            if (isKingAlive == false)
                return;

            SetInteractive(whitePieces, isWhiteTurn);
            SetInteractive(blackPieces, !isWhiteTurn);

            if (clockManager.launched == false)
            {
                clockManager.StartClocks();
            }
            clockManager.setTurn(isWhiteTurn);
        }        
    }

    /// <summary>
    /// Display AI move
    /// </summary>
    /// <returns></returns>
    private IEnumerator showIAMoveCoroutine()
    {
        
        IATurn = true;
        string best = stockfish.GetBestMove();
        yield return new WaitForSeconds((float)2);
        
        string depA = best.Substring(0, 1);
        string depB = best.Substring(1, 1);
        string arrA = best.Substring(2, 1);
        string arrB = best.Substring(3, 1);

        Cell dep = chessBoard.allCells[coordA[depA]][coordB[depB]];
        Cell targ = chessBoard.allCells[coordA[arrA]][coordB[arrB]];

        // If promotion
        if(dep.currentPiece.GetType() == typeof(Pawn) && (coordB[arrB] == 0 || coordB[arrB] == 7))
        {
            best += "q";
        }

        Debug.Log(best);

        stockfish.setIAmove(best);

        dep.currentPiece.TargetCell = targ;
        dep.currentPiece.Move();
        IATurn = false;

        if (GameState.INGAME == gameState)
        {
            if (isIAWithe)
                SetInteractive(blackPieces, true);
            else
                SetInteractive(whitePieces, true);

            clockManager.setTurn(!isIAWithe);
        }
    }

    /// <summary>
    /// Transform pawn into queen
    /// </summary>
    /// <param name="pawn"></param>
    /// <param name="BeforeCell"></param>
    public void PawnPromotion(Pawn pawn, Cell BeforeCell)
    {
        pawn.currentCell.RemovePiece();
        GameObject newPieceObject = Instantiate(piecePrefab);
        newPieceObject.transform.SetParent(transform);

        newPieceObject.transform.localScale = new Vector3(1, 1, 1);
        newPieceObject.transform.localRotation = Quaternion.identity;

        float board_width = BeforeCell.board.GetComponent<RectTransform>().rect.width;
        float board_height = BeforeCell.board.GetComponent<RectTransform>().rect.height;

        float piece_width = board_width / BeforeCell.board.Column - BasePiece.CellPadding;
        float piece_height = board_height / BeforeCell.board.Row - BasePiece.CellPadding;
        newPieceObject.GetComponent<RectTransform>().sizeDelta = new Vector2(piece_width, piece_height);

        Queen queen = (Queen)newPieceObject.AddComponent(typeof(Queen));
        //base.pieceManager.pieceList.Add(newPiece);

        queen.Setup(pawn.isWhite, this);
        //queen.PlaceInit(promotionCell);
        queen.TargetCell = pawn.currentCell;
        queen.currentCell = BeforeCell;
        queen.currentCell.currentPiece = queen;
        queen.Move();
        if (pawn.isWhite)
        {
            whitePieces.Remove(pawn);
            whitePieces.Add(queen);
        } else
        {
            blackPieces.Remove(pawn);
            blackPieces.Add(queen);
        }
        queen.gameObject.SetActive(true);        
    }

    /// <summary>
    /// return black king or white king
    /// </summary>
    /// <param name="isWhite"></param>
    /// <returns></returns>
    public King getKing(bool isWhite)
    {
        if (isWhite)
            return whiteKing;
        else
            return blackKing;
    }

    /// <summary>
    /// Display result on the scene
    /// </summary>
    public void ShowResult()
    {        
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/basic/end"));

        SetInteractive(whitePieces, false);
        SetInteractive(blackPieces, false);

        clockManager.StopClocks();

        clockManager.highlightClockW.SetActive(false);
        clockManager.highlightClockB.SetActive(false);

        result.enabled = false;

        StartCoroutine(showResultCoroutine());
    }

    private IEnumerator showResultCoroutine()
    {
        yield return new WaitForSeconds((float)2.1);
        if (gameState == GameState.BLACK_WIN)
        {
            result.text = "Winner: Black";
            clockManager.highlightClockB.SetActive(true);
            clockManager.highlightClockB.GetComponent<Image>().color = new Color(1, (float)0.6816, 0, 1);
        }
        if (gameState == GameState.WHITE_WIN)
        {
            result.text = "Winner: White";
            clockManager.highlightClockW.SetActive(true);
            clockManager.highlightClockW.GetComponent<Image>().color = new Color(1, (float)0.6816, 0, 1);

        }
        if (gameState == GameState.PAT)
        {
            result.text = "PAT !";
        }
        result.enabled = true;
    }

    /// <summary>
    /// Change chess board theme
    /// </summary>
    /// <param name="newTheme"></param>
    public void ApplyTheme(Theme newTheme)
    {
        theme = newTheme;
        for (int x = 0; x < chessBoard.Column; x++)
        {
            for (int y = 0; y < chessBoard.Row; y++)
            {
                Cell cell = chessBoard.allCells[x][y];
                cell.GetComponent<Image>().color = theme.blackCell;
                cell.GetComponent<Image>().sprite = theme.textureSprite;

                if (cell.currentPiece != null)
                {
                    if (cell.currentPiece.isWhite)
                        cell.currentPiece.GetComponent<Image>().color = theme.whitePiece;
                    else
                        cell.currentPiece.GetComponent<Image>().color = theme.blackPiece;
                    if(cell.currentPiece.GetType() == typeof(King))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/king");
                    if (cell.currentPiece.GetType() == typeof(Pawn))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/pawn");
                    if (cell.currentPiece.GetType() == typeof(Queen))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/queen");
                    if (cell.currentPiece.GetType() == typeof(Bishop))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/bishop");
                    if (cell.currentPiece.GetType() == typeof(Knight))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/knight");
                    if (cell.currentPiece.GetType() == typeof(Rook))
                        cell.currentPiece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + theme.spriteFolder + "/rook");
                }
            }
        }

        // Color white cell
        for (int x = 0; x < chessBoard.Column; x += 2)
        {
            for (int y = 0; y < chessBoard.Row; y++)
            {
                // Offset for every other line
                int offset = (y % 2 != 0) ? 0 : 1;
                int finalX = x + offset;

                // Col
                //Color col = new Color32(230, 220, 187, 255);
                Image im = chessBoard.allCells[finalX][y].GetComponent<Image>();
                im.color = theme.whiteCell;
            }
        }
    }
}


