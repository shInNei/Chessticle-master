using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pawn : BasePiece
{
    public override void Setup(bool newIsWhite, PieceManager newPM)
    {
        base.Setup(newIsWhite, newPM);
        movement = isWhite ? new Vector3Int(0, 1, 1) : new Vector3Int(0, -1, -1);
        if (pieceManager.theme == null)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/basic/pawn");
        else GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + pieceManager.theme.spriteFolder + "/pawn");

        hasMoved = false;
    }

    public override void Move()
    {
        // En passant
        Cell targ = TargetCell;
        Cell beforeMove = currentCell;
        bool isFirstMove = !hasMoved;

        base.Move();
		
        if (isFirstMove)
        {
            if (targ.boardPosition.y == beforeMove.boardPosition.y + 2 * movement.y)
            {
                Cell enPassantCell = beforeMove.board.allCells[beforeMove.boardPosition.x][beforeMove.boardPosition.y + movement.y];
                enPassantCell.enPassant = this;
                pieceManager.enPassantCell = enPassantCell;
            }
        }
        if(currentCell.boardPosition.y == 0 || currentCell.boardPosition.y == 7)
        {
            pieceManager.PawnPromotion(this, beforeMove);
        }

    }

    private bool MatchesState(Cell target, CellState targetState)
    {
        CellState cellstate = target.GetState(this);

        if(cellstate == targetState)
        {
            if (!pieceManager.checkVerificationInProcess)
            {
                // Add to list
                if (cellstate == CellState.ENEMY || cellstate == CellState.PASSANT)
                {
                    target.outlineImage.GetComponent<Image>().color = new Color(1, 0, 0, (float)0.5);
                }
                else
                {
                    target.outlineImage.GetComponent<Image>().color = new Color(0, 1, 0, (float)0.5);
                }
                //highlightedCells.Add(target);
            }
            addPossibleCell(target);
            return true;
        }
        if (cellstate == CellState.FREE || cellstate == CellState.CHECK)
            return true;
        return false;
    }

    protected override void CheckPathing()
    {
        
        // target pos
        int currentX = currentCell.boardPosition.x;
        int currentY = currentCell.boardPosition.y;

        // top left
        try
        {
            MatchesState(currentCell.board.allCells[currentX - movement.z][currentY + movement.z], CellState.ENEMY);
            MatchesState(currentCell.board.allCells[currentX - movement.z][currentY + movement.z], CellState.PASSANT);
        }
        catch (Exception e) { e.ToString(); }

        try
        {
            // forward
            if (MatchesState(currentCell.board.allCells[currentX][currentY + movement.y], CellState.FREE))
            {
                if (!hasMoved)
                {
                    MatchesState(currentCell.board.allCells[currentX][currentY + movement.y * 2], CellState.FREE);
                }
            }
        }
        catch (Exception e) { e.ToString(); }

        // top right
        try
        {
            MatchesState(currentCell.board.allCells[currentX + movement.z][currentY + movement.z], CellState.ENEMY);
            MatchesState(currentCell.board.allCells[currentX + movement.z][currentY + movement.z], CellState.PASSANT);
        }
        catch (Exception e) { e.ToString(); }

    }

}
