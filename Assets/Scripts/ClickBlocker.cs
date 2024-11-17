using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickBlocker : MonoBehaviour
{
    [SerializeField]Cursor _cursor;

    private void Awake()
    {
        _cursor = GetComponent<Cursor>();
    }

    private void OnEnable()
    {
        BoardController.OnTurnStarted += SwitchCursorState;
    }

    private void OnDisable()
    {
        BoardController.OnTurnStarted -= SwitchCursorState;
    }

    void SwitchCursorState(Token currentPlayer)
    {
        if (currentPlayer == Token.Player) _cursor.enabled = true;
        else if (currentPlayer == Token.Enemy) _cursor.enabled = false;
    }

}
