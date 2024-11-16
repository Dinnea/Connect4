using Personal.GridFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks for win condition and triggers the win event.
/// </summary>
public class WinChecker : MonoBehaviour
{

    int tokensToWin = 4;
    public Action<Token> OnWin;

    GridXY<Token> _board;


    private void Start()
    {
        _board = Grid.GetBoard();
    }

    private void OnEnable()
    {
        TokenSpawner.OnTokenDropped += CheckForWin;
    }

    private void OnDisable()
    {
        TokenSpawner.OnTokenDropped -= CheckForWin;
    }

    /// <summary>
    /// Checks for win, starting from given token coordinates.
    /// Runs veritcal, horizontal and diagonal checks.
    /// Only checks around the newly dropped token.
    /// </summary>
    /// <param name="tokenCoords"> Point from which checking is started</param>
    void CheckForWin(MoveData eventData)
    {
        if (_board.CheckInBounds(eventData.tokenCoords)) //only proceed if the coords are valid
        {
            if (CheckForHorizontalWin(eventData) ||
             CheckForVerticalWin(eventData) ||
             CheckForDiagonalWin(eventData))
            {
                Debug.Log(eventData.currentPlayer + " wins!");
                OnWin?.Invoke(eventData.currentPlayer);
            }
            else Debug.Log("No win reached.");
        }        
    }
    bool CheckForVerticalWin(MoveData eventData)
    {
        // Y+i
        int sequenceCount = 1;
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(eventData.tokenCoords.x, eventData.tokenCoords.y + 1);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                //Debug.Log("In bounds, continuing");
                if (eventData.currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        //Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           //else stop checking in this direction.
            }
            else                                                    //Do not check out of bounds
            {
                //Debug.Log("Out of bounds.");
                break;
            }
        }

        //Y-i
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(eventData.tokenCoords.x, eventData.tokenCoords.y - i);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                //Debug.Log("In bounds, continuing");
                if (eventData.currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                       //Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                        //else stop checking in this direction.
            }
            else
            {
                //Debug.Log("Out of bounds.");                        //Do not check out of bounds
                break;
            }
        }
        return false;
    }
    bool CheckForHorizontalWin(MoveData eventData)
    {
        // X+i
        int sequenceCount = 1;
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(eventData.tokenCoords.x + i, eventData.tokenCoords.y);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                //Debug.Log("In bounds, continuing");
                if (eventData.currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        //Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           //else stop checking in this direction
            }
            else                                                      // do not check out of bounds
            {
                //Debug.Log("Out of bounds.");
                break;
            }
        }

        //X-i
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(eventData.tokenCoords.x - i, eventData.tokenCoords.y);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                //Debug.Log("In bounds, continuing");
                if (eventData.currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        //Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                          //else stop checking in this direction
            }
            else
            {
                //Debug.Log("Out of bounds."); 
                break;                                             // do not check out of bounds
            }
        }
        return false;
    }

    /// <summary>
    /// Condenses both diagnola win checks into one method.
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    bool CheckForDiagonalWin(MoveData eventData)
    {
        return (CheckForDiagonalWinDownhillSlope(eventData) || CheckForDiagonalWinUphillSlope(eventData));
    }
    bool CheckForDiagonalWinDownhillSlope(MoveData eventData)
    {
        //x-i, y+i
        int sequenceCount = 1;
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(eventData.tokenCoords.x - i, eventData.tokenCoords.y + i);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                //Debug.Log("In bounds, continuing");
                if (eventData.currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        //Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                            //else stop checking in this direction
            }
            else                                                  //Do not check out of bounds
            {
                //Debug.Log("Out of bounds.");
                break;
            }
        }

        //x+i, y-i
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(eventData.tokenCoords.x + i, eventData.tokenCoords.y - i);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                //Debug.Log("In bounds, continuing");
                if (eventData.currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        //Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           //else stop checking in this direction
            }
            else
            {
                //Debug.Log("Out of bounds.");                           //Do not check out of bounds
                break;
            }
        }
        return false;
    }
    bool CheckForDiagonalWinUphillSlope(MoveData eventData)
    {
        //x-i, y-i
        int sequenceCount = 1;
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(eventData.tokenCoords.x - i, eventData.tokenCoords.y - i);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                //Debug.Log("In bounds, continuing");
                if (eventData.currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           //else stop checking in this direction
            }
            else                                                    //No checking out of bounds!
            {
                //Debug.Log("Out of bounds.");
                break;
            }
        }

        //x+i, y+i
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(eventData.tokenCoords.x + i, eventData.tokenCoords.y + i);
            //First check if in bounds
            if (_board.CheckInBounds(newCoords))
            {
                //Debug.Log("In bounds, continuing");
                if (eventData.currentPlayer == _board.GetCellContent(newCoords)) //if the token is the same
                {
                    sequenceCount++;                                  //add to the count
                    if (sequenceCount == tokensToWin)
                    {
                        //Debug.Log("Win condition found!");
                        return true;                                 //end early if the win condition has been met
                    }
                }
                else break;                                           //else stop checking in this direction
            }
            else                                                    //No checking out of bounds!
            {
                //Debug.Log("Out of bounds.");
                break;
            }
        }
        return false;
    }
}
