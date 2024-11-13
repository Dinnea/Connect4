using Personal.GridFramework;
using System;
using UnityEngine;
public enum Token
{
    Empty,
    Player,
    Enemy
}

public class MoveData
{
    public Token token;
    public Vector2Int boardCoords;
    public MoveData(Token token, Vector2Int coords)
    {
        this.token = token;
        boardCoords = coords;
    }
}
public class BoardManager : MonoBehaviour
{
    [SerializeField] GameObject _playerToken;
    [SerializeField] GameObject _enemyToken;


    [SerializeField] float _spawnedZOffset;

    Grid _gridController;
    GridXY<Token> _board;
    public Action<Token> OnPlayerTurnChanged;
    public Action<MoveData> OnTokenDropped;
    public Action<Token> OnWin;

    //Denotes which token will be drop/which player's turn it is
    Token currentPlayer = Token.Player;

    int tokensToWin = 4;

    private void Awake()
    {
        _gridController = GetComponent<Grid>();
        OnPlayerTurnChanged?.Invoke(currentPlayer);

    }

    private void Start()
    {
        _board = Grid.GetBoard();
    }
    private void OnEnable()
    {
        Cursor.OnBoardClicked += DropToken;
        OnTokenDropped += SpawnToken;
        OnTokenDropped += CheckForWin;
    }
    private void OnDisable()
    {
        Cursor.OnBoardClicked -= DropToken;
        OnTokenDropped -= SpawnToken;
        OnTokenDropped -= SpawnToken;
    }

    void DropToken(Vector2Int coords)
    {
        int column = coords.x;
        if (column != -1) //check bounds
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
                OnTokenDropped?.Invoke(new MoveData(currentPlayer, newTokenCoords));

                //CheckForWin(newTokenCoords);


                /* Turn switching */
                if (currentPlayer == Token.Player) currentPlayer = Token.Enemy;      //If player turn, switch to enemy
                else if (currentPlayer == Token.Enemy) currentPlayer = Token.Player; //If enemy turn, switch to player.

                OnPlayerTurnChanged?.Invoke(currentPlayer); //inform subcribers that the turn has been changed
                /**/
                break; //Once the token is in grid, no need to continue checking.
            }
        }
    }
    
    void SpawnToken(MoveData eventData)
    {
        GameObject toInstantiate = null;
        if (eventData.token == Token.Player)
        {
            toInstantiate = _playerToken;
        }
        else if (eventData.token == Token.Enemy)
        {
            toInstantiate = _enemyToken;
        }
        else 
        {
            Debug.LogError("Invalid token.");
        }

        //float step = _speed * Time.deltaTime;

        Vector3 targetLocation = _board.GridToWorldPosition(eventData.boardCoords) + _board.GetCellOffset();
        targetLocation = new Vector3(targetLocation.x, targetLocation.y, _spawnedZOffset);
        //Vector3 spawnLocation = new Vector3(targetLocation.x, 60, targetLocation.z);
        GameObject newToken = Instantiate(toInstantiate, targetLocation, Quaternion.identity);
        //newToken.transform.position = Vector3.MoveTowards(newToken.transform.position, targetLocation, step);
    }

    /// <summary>
    /// Checks for win, starting from given token coordinates.
    /// Runs veritcal, horizontal and diagonal checks.
    /// </summary>
    /// <param name="tokenCoords"> Point from which checking is started</param>
    void CheckForWin(MoveData eventData)
    {

        if (CheckForHorizontalWin(eventData.boardCoords) ||
             CheckForVerticalWin(eventData.boardCoords) ||
             CheckForDiagonalWin(eventData.boardCoords))
        {
            Debug.Log(currentPlayer + " wins!");
            OnWin?.Invoke(currentPlayer);
        }
        else Debug.Log("No win reached.");
    }
    bool CheckForVerticalWin(Vector2Int tokenCoords)
    {
        // Y+i
        int sequenceCount = 1;
        for (int i = 1; i <= tokensToWin - 1; i++)
        {
            Vector2Int newCoords = new Vector2Int(tokenCoords.x, tokenCoords.y + 1);
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
