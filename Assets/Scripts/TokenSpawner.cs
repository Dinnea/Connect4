using Personal.GridFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TokenSpawner : MonoBehaviour
{
    [SerializeField] GameObject _playerToken;
    [SerializeField] GameObject _enemyToken;


    [SerializeField] float _spawnedZOffset;

    GridController _gridController;
    GridXY<Token> _board;

    private void Awake()
    {
        _gridController = GetComponent<GridController>();
        
    }

    private void Start()
    {
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

        //float step = _speed * Time.deltaTime;

        Vector3 targetLocation = _board.GridToWorldPosition(eventData.boardCoords) + _board.GetCellOffset();
        targetLocation = new Vector3(targetLocation.x, targetLocation.y, _spawnedZOffset);
        //Vector3 spawnLocation = new Vector3(targetLocation.x, 60, targetLocation.z);
        GameObject newToken = Instantiate(toInstantiate, targetLocation, Quaternion.identity);
        //newToken.transform.position = Vector3.MoveTowards(newToken.transform.position, targetLocation, step);
    }


}
