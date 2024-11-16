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

/// <summary>
/// Stores info about WHO has made a move and WHERE did the token land.
/// </summary>
public class MoveData
{
    public Token currentPlayer;
    public Vector2Int tokenCoords;
    public MoveData(Token currentPlayer, Vector2Int coords)
    {
        this.currentPlayer = currentPlayer;
        tokenCoords = coords;
    }
}
/// <summary>
/// Spawns the game board, you can get information about the board from here.
/// </summary>
public class Grid : MonoBehaviour
{
    //Only one board in game, static board is easier to access.
    static GridXY<Token> _board;

    [SerializeField] Vector2Int _dimensions = new Vector2Int(6, 6);
    [SerializeField] Vector3 _origin = Vector3.zero;
    [SerializeField] float _cellSize = 2;

    
    private MeshFilter _meshFilter;
    private Mesh _mesh;

    private void Awake()
    {
        initializeBoard();
        
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

    /// <summary>
    /// Shortcut to only get the x coordinate on the board at given location.ss
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    int FindColumn(Vector3 location)
    {
        return _board.WorldToGridPostion(location).x;
    }

    /// <summary>
    /// Find the lowest cell in given column that is not yet taken up.
    /// </summary>
    /// <param name="column"></param>
    /// <returns></returns>
    public static Vector2Int FindFirstFreeCellInColumn(int column)
    {
        for (int row = 0; row < _board.GetHeightInRows(); row++) //Go row by row starting at the bottom
        {
            Token currentCellContent = _board.GetCellContent(column, row);
            if (currentCellContent == Token.Empty) //Check if the cell is empty,if yes:
            {
                return new Vector2Int(column, row);
            }
        }
        return new Vector2Int(column, -1);
    }

    public static GridXY<Token> GetBoard()
    {
        return _board;
    }
}
