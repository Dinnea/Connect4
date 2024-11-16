using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Personal.Utilities;
using System.Drawing;
using System;
using UnityEditor;
using Personal.GridFramework;

public class Cursor : MonoBehaviour
{
    [SerializeField] LayerMask layer;

    public Action<Vector2Int> OnBoardClicked;
    [SerializeField] float _cursorDepth;
    [SerializeField] float _cursorBoardGap = 1.2f;
    Vector3 _cursorLocation;


    GridXY<Token> _board;
    private void Start()
    {
        _board = GridComponent.GetBoard();
    }

    private void Update()
    {
        MoveOnBoard();
    }

    private void OnEnable()
    {
        WinChecker.OnWin += Disable;
    }

    private void OnDisable()
    {
        WinChecker.OnWin -= Disable;
    }

    void MoveOnBoard()
    {
         _cursorLocation = VectorMath.GetMousePositionWorld(Camera.main, layer);
        Vector2Int cellCoords = _board.WorldToGridPostion(_cursorLocation);
        if (_board.CheckInBounds(cellCoords.x, 0))//if isnt out of bounds, change location, get ready to send click info to observers
        {
            //Find the destination cell.
            transform.position = new Vector3(_board.GridToWorldPosition(cellCoords).x, _board.GridToWorldPosition(cellCoords.x, _board.GetHeightInRows(), true).y + _cursorBoardGap, _cursorDepth);
        }
        if (Input.GetMouseButtonDown(0))
        {
            OnBoardClicked?.Invoke(cellCoords);
        }
    }

    public Vector3 GetCursorLocation()
    {
        return transform.position;
    }

    void Disable(Token player)
    {
        gameObject.SetActive(false);
    }
}
