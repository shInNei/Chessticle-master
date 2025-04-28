using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bishop : BasePiece
{
    public override void Setup(bool newIsWhite, PieceManager newPM)
    {
        base.Setup(newIsWhite, newPM);

        movement = new Vector3Int(0, 0, 7);
        if(pieceManager.theme == null)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/basic/bishop");
        else GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + pieceManager.theme.spriteFolder + "/bishop");
    }
}
