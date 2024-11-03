using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Personal.GridFramework;

public enum Token
{
    Empty,
    Player, 
    Enemy
}

public class GridController : MonoBehaviour
{
    public static GridXY<Token> board;

    [SerializeField] Vector2Int _dimensions = new Vector2Int(6, 6);
    [SerializeField] Vector3 _origin = Vector3.zero;
    [SerializeField] float _cellSize = 2;

    private void Start()
    {
        InitializeBoard();
    }
    void InitializeBoard()
    {
        board = new GridXY<Token>(_dimensions.x, _dimensions.y, _cellSize, _origin, (GridXY<Token> g, int x, int y) => Token.Empty);
    }
}
