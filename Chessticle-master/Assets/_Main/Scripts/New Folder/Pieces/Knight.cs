using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Knight : BasePiece
{
    public override void Setup(bool newIsWhite, PieceManager newPM)
    {
        base.Setup(newIsWhite, newPM);

        movement = new Vector3Int(1, 1, 1);
        if (pieceManager.theme == null)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/basic/knight");
        else GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + pieceManager.theme.spriteFolder + "/knight");
    }

    private void CreateCellPath(int yDirection)
    {
        int currentX = currentCell.boardPosition.x;
        int currentY = currentCell.boardPosition.y;
        Cell targetCell;

        // max left 
        try{
            targetCell = currentCell.board.allCells[currentX - 2][currentY + 1 * yDirection];
            MatchesState(targetCell);
        }
        catch (Exception e) { e.ToString(); }

        // left 
        try
        {
            targetCell = currentCell.board.allCells[currentX - 1][currentY + 2 * yDirection];
            MatchesState(targetCell);
        }
        catch (Exception e) { e.ToString(); }

        // max right 
        try
        {
            targetCell = currentCell.board.allCells[currentX + 2][currentY + 1 * yDirection];
            MatchesState(targetCell);
        }
        catch (Exception e) { e.ToString(); }

        // right 
        try
        {
            targetCell = currentCell.board.allCells[currentX + 1][currentY + 2 * yDirection];
            MatchesState(targetCell);
        }
        catch (Exception e) { e.ToString(); }
    }

    protected override void CheckPathing()
    {
        // top
        CreateCellPath(1);
        // down
        CreateCellPath(-1);
    }

    private void MatchesState(Cell target)
    {
        CellState targetState = target.GetState(this);

        if(targetState != CellState.FRIEND && targetState != CellState.CHECK && targetState != CellState.CHECK_ENEMY && targetState != CellState.CHECK_FRIEND)
        {
            if (!pieceManager.checkVerificationInProcess)
            {
                // Add to list
                if (targetState == CellState.ENEMY)
                {
                    target.outlineImage.GetComponent<Image>().color = new Color(1, 0, 0, (float)0.5);
                }
                else
                {
                    target.outlineImage.GetComponent<Image>().color = new Color(0, 1, 0, (float)0.5);
                }
            }            
            //highlightedCells.Add(target);
            addPossibleCell(target);
        }        
    }
}
