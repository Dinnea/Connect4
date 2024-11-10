using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Personal.GridFramework;
using System;

public enum Token
{
    Empty,
    Player, 
    Enemy
}

public class GridController : MonoBehaviour
{
    GridXY<Token> _board;

    //Denotes which token will be drop/which player's turn it is
    Token player = Token.Player;

    [SerializeField] Vector2Int _dimensions = new Vector2Int(6, 6);
    [SerializeField] Vector3 _origin = Vector3.zero;
    [SerializeField] float _cellSize = 2;

    public Action<Token> OnPlayerTurnChanged;

    private void Start()
    {
        InitializeBoard();
        OnPlayerTurnChanged?.Invoke(player);
    }

    private void OnEnable()
    {
        Cursor.OnBoardClicked += DropToken;
    }
    private void OnDisable()
    {
        Cursor.OnBoardClicked -= DropToken;
    }

    void InitializeBoard()
    {
        _board = new GridXY<Token>(_dimensions.x, _dimensions.y, _cellSize, _origin, (GridXY<Token> g, int x, int y) => Token.Empty);
    }

    int FindColumn(Vector3 location)
    {
        return _board.WorldToGridPostion(location).x;
    }

    void DropToken(Vector3 location)
    {
        int column = FindColumn(location);
        if ( column != -1) //check bounds
        {
            DropToken(column);
        }
    }
    void DropToken(int column)
    {
        for (int rowToCheck = 0; rowToCheck < _board.GetHeightInRows(); rowToCheck++) //Go row by row starting at the bottom
        {
            Token currentCellContent = _board.GetCellContent(column, rowToCheck);

            if (currentCellContent == Token.Empty) //Check if the cell is empty,if yes:
            {
                _board.SetCellContent(column, rowToCheck, player); //Set the cell content to the right token.
                
                if (player == Token.Player) player = Token.Enemy;      //If player turn, switch to enemy
                else if (player == Token.Enemy) player = Token.Player; //If enemy turn, switch to player.

                OnPlayerTurnChanged?.Invoke(player); //inform that the turn has been switched
                break; //exit the function :> 
            }
        }
    }
}
