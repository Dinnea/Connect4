using Personal.GridFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic Random AI class. Plays tokens randomly without logic.
/// </summary>
public class PlayerAIRandom : MonoBehaviour
{
    GridXY<Token> _board;
    [SerializeField] float _delay = 1f;

    public static Action<int> OnAITokenDrop;
    private void Start()
    {
        _board = GridComponent.GetBoard();
    }

    private void OnEnable()
    {
        BoardController.OnTurnStarted += PlayTokenWithDelay;
        BoardController.OnTokenFailedToDrop += PlayTokenWithDelay;
        WinChecker.OnWin += DisableAI;

    }

    private void OnDisable()
    {
        BoardController.OnTurnStarted -= PlayTokenWithDelay;
        BoardController.OnTokenFailedToDrop -= PlayTokenWithDelay;
        WinChecker.OnWin -= DisableAI;
    }
    void PlayTokenWithDelay(Token currentPlayer)
    {
        if (currentPlayer == Token.Enemy)
        {
            Invoke("PlayToken", _delay);
        }
    }

    void PlayTokenNoDelay()
    {
        int columnToPlay = UnityEngine.Random.Range(0, _board.GetWidthInColumns());
        OnAITokenDrop?.Invoke(columnToPlay);
    }

    void DisableAI(Token player)
    {
        gameObject.SetActive(false);
    }
}
