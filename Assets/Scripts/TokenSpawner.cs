using Personal.GridFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenSpawner : MonoBehaviour
{
    [SerializeField] GameObject _playerToken;
    [SerializeField] GameObject _enemyToken;

    GridController _gridController;
    GridXY<Token> _board;

    private void Awake()
    {
        _gridController = GetComponent<GridController>();
        _board = _gridController.GetBoard();
    }
    private void OnEnable()
    {
        _gridController.OnTokenDropped += SpawnToken;
    }
    private void OnDisable()
    {
        _gridController.OnTokenDropped -= SpawnToken;
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

        Vector3 targetLocation = _board.GridToWorldPosition(eventData.boardCoords) + _board.GetCellOffset();

        GameObject newToken = Instantiate(toInstantiate, targetLocation, Quaternion.identity);
    }
}
