using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Only to be present in the AI scene - prevents player from playing AI's turn.
/// </summary>
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
