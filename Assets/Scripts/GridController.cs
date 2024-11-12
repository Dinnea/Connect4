using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Personal.GridFramework;
using System;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEditor;

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
    public Action<MoveData> OnTokenDropped;
    public Action<Token> OnWin;
    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private void Awake()
    {
        initializeBoard();
        OnPlayerTurnChanged?.Invoke(currentPlayer);
    }

    private void OnEnable()
    {
        Cursor.OnBoardClicked += DropToken;
        OnTokenDropped += CheckForWin;
    }
    private void OnDisable()
    {
        Cursor.OnBoardClicked -= DropToken;
        OnTokenDropped -= CheckForWin;
    }

    /// <summary>
    /// Generates grid and spawns meshes to create a visual for grid. 1 mesh per cell.
    /// </summary>
    void initializeBoard()
    {
        _board = new GridXY<Token>(_dimensions.x, _dimensions.y, _cellSize, _origin, (GridXY<Token> g, int x, int y) => Token.Empty);
        
       
        _meshFilter = GetComponent<MeshFilter>();

        _mesh = new Mesh();
        _mesh.name = "Grid";

        //arraySizes
         Vector3[] vertices = new Vector3[_dimensions.y * _dimensions.x * 4];
         int[] triangles = new int[_dimensions.y * _dimensions.x * 6];
         Vector2[] uv = new Vector2[_dimensions.y * _dimensions.x * 4];

         //set tracker indices
         int v = 0; int t = 0; int u = 0;

         for (int x = 0; x < _dimensions.x; x++)
         {
            for (int y = 0; y < _dimensions.y; y++)
            {
                Vector3 cellOffset = new Vector3(x * _cellSize,  y * _cellSize, 0);
                //populate the arrays
                vertices[v] = new Vector3(_origin.x, _origin.y, 0) + cellOffset;
                vertices[v + 1] = new Vector3(_origin.x, _cellSize, 0) + cellOffset;
                vertices[v + 2] = new Vector3(_cellSize, 0, _origin.y) + cellOffset;
                vertices[v + 3] = new Vector3(_cellSize, _cellSize, 0) + cellOffset;

                triangles[t] = v;
                triangles[t + 1] = triangles[t + 4] = v + 1;
                triangles[t + 2] = triangles[t + 3] = v + 2;
                triangles[t + 5] = v + 3;
                uv[u] = new Vector2(0, 0);
                uv[u + 1] = new Vector2(0, 1);
                uv[u + 2] = new Vector2(1, 0);
                uv[u + 3] = new Vector2(1, 1);
                
                v += 4;
                t += 6;
                u += 4;
            }

            _mesh.vertices = vertices;
            _mesh.uv = uv;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
            _meshFilter.mesh = _mesh;
            }
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
    /// <summary>
    /// Checks for win, starting from given token coordinates.
    /// Runs veritcal, horizontal and diagonal checks.
    /// </summary>
    /// <param name="tokenCoords"> Point from which checking is started</param>
    void CheckForWin(MoveData eventData)
    {

        if ( CheckForHorizontalWin(eventData.boardCoords) ||
             CheckForVerticalWin(eventData.boardCoords)   ||
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

    public GridXY<Token> GetBoard()
    {
        return _board;
    }
}
