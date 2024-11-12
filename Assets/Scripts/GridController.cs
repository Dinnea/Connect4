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
    Token currentPlayer = Token.Player;

    int tokensToWin = 4;

    [SerializeField] Vector2Int _dimensions = new Vector2Int(6, 6);
    [SerializeField] Vector3 _origin = Vector3.zero;
    [SerializeField] float _cellSize = 2;

    public Action<Token> OnPlayerTurnChanged;
    public Action<Token> OnWin;

    private void Start()
    {
        InitializeBoard();
        OnPlayerTurnChanged?.Invoke(currentPlayer);
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
                Vector2Int newTokenCoords = new Vector2Int(column, rowToCheck);
                _board.SetCellContent(newTokenCoords, currentPlayer); //Set the cell content to the right token.

                //Check for win - no need to check the whole board every time, only the cells near the newly placed token.
                CheckForWin(newTokenCoords);
                

                /* Turn switching */
                if (currentPlayer == Token.Player) currentPlayer = Token.Enemy;      //If player turn, switch to enemy
                else if (currentPlayer == Token.Enemy) currentPlayer = Token.Player; //If enemy turn, switch to player.

                OnPlayerTurnChanged?.Invoke(currentPlayer); //inform subcribers that the turn has been changed
                /**/
                break; //Once the token is in grid, no need to continue checking.
            }
        }
    }
    /// <summary>
    /// Checks for win, starting from given token coordinates.
    /// </summary>
    /// <param name="tokenCoords"></param>

    void CheckForWin(Vector2Int tokenCoords)
    {

        if ( CheckForHorizontalWin(tokenCoords) ||
             CheckForVerticalWin(tokenCoords)   ||
             CheckForDiagonalWin(tokenCoords))
        {
            Debug.Log(currentPlayer + " wins!");
            OnWin?.Invoke(currentPlayer);
        }
        else Debug.Log("No win reached.");


        //diagonal win
        //x+n, y+n
        //x+n, y-n
        //x-n, y+n
        //x-n, y-n


    }
    bool CheckForVerticalWin(Vector2Int tokenCoords)
    {
        // Y+i
        int sequenceCount = 1;
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(tokenCoords.x, tokenCoords.y +1);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                Debug.Log("In bounds, continuing");
                if (currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           //else time to go in the other direction.
            }
            else
            {
                Debug.Log("Out of bounds.");
                break;
            }
        }

        //Y-i
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(tokenCoords.x, tokenCoords.y - i);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                Debug.Log("In bounds, continuing");
                if (currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;
            }
            else
            {
                Debug.Log("Out of bounds.");
                break;
            }
        }
        return false;
    }
    bool CheckForHorizontalWin(Vector2Int tokenCoords)
    {
        // X+i
        int sequenceCount = 1;
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(tokenCoords.x + i, tokenCoords.y);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                Debug.Log("In bounds, continuing");
                if (currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           //else time to go in the other direction.
            }
            else
            {
                Debug.Log("Out of bounds.");
                break;
            }
        }

        //X-i
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(tokenCoords.x - i, tokenCoords.y);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                Debug.Log("In bounds, continuing");
                if (currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           
            }
            else
            {
                Debug.Log("Out of bounds.");
                break;
            }
        }

        return false;
    }
    bool CheckForDiagonalWin(Vector2Int tokenCoords)
    {
        return (CheckForDiagonalWinDownhillSlope(tokenCoords) || CheckForDiagonalWinUphillSlope(tokenCoords));
    }
    bool CheckForDiagonalWinDownhillSlope(Vector2Int tokenCoords) 
    {
        //x-i, y+i
        int sequenceCount = 1;
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(tokenCoords.x - i, tokenCoords.y + i);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                Debug.Log("In bounds, continuing");
                if (currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           //else time to go in the other direction.
            }
            else
            {
                Debug.Log("Out of bounds.");
                break;
            }
        }

        //x+i, y-i
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(tokenCoords.x + i, tokenCoords.y - i);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                Debug.Log("In bounds, continuing");
                if (currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           //else time to go in the other direction.
            }
            else
            {
                Debug.Log("Out of bounds.");
                break;
            }
        }
        return false; 
    }

    bool CheckForDiagonalWinUphillSlope(Vector2Int tokenCoords) 
    {
        //x-i, y-i
        int sequenceCount = 1;
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(tokenCoords.x - i, tokenCoords.y - i);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                Debug.Log("In bounds, continuing");
                if (currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           //else time to go in the other direction.
            }
            else
            {
                Debug.Log("Out of bounds.");
                break;
            }
        }

        //x+i, y+i
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(tokenCoords.x + i, tokenCoords.y + i);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                Debug.Log("In bounds, continuing");
                if (currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           //else time to go in the other direction.
            }
            else
            {
                Debug.Log("Out of bounds.");
                break;
            }
        }
        return false;
    }

    
}
