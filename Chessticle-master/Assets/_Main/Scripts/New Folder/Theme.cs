using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Theme : MonoBehaviour
{
    public Color whiteCell;
    public Color blackCell;

    public Color whitePiece;
    public Color blackPiece;

    public Sprite textureSprite;

    public GameManager gm;

    public string spriteFolder;

    public void ChangeTheme()
    {
        gm.pieceManager.ApplyTheme(this);    
    }
    
}
