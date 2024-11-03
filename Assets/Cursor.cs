using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Personal.Utilities;
using System.Drawing;

public class Cursor : MonoBehaviour
{
    [SerializeField] LayerMask layer;

    Token playerToken = Token.Player;
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2Int coord = GridController.board.WorldToGridPostion(VectorMath.GetMousePositionWorld(Camera.main, layer));
            Debug.Log(coord);
            //coord x is the vertical coord
            //So i need to go through every row (y) from 0 to 5, looking for an empty spot

            DropToken(coord.x);
        }
    }

    void DropToken(int column)
    {
        for (int rowToCheck = 0; rowToCheck < GridController.board.GetHeightInRows(); rowToCheck++)
        {
            Token currentCellContent = GridController.board.GetCellContent(column, rowToCheck);
            if (currentCellContent == Token.Empty)
            {
                GridController.board.SetCellContent(column, rowToCheck, playerToken);
                if (playerToken == Token.Player) playerToken = Token.Enemy;
                else if(playerToken == Token.Enemy) playerToken = Token.Player;
                break;
            }
        }
    }
}
