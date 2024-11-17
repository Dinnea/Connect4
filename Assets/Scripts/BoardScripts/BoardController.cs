using Personal.GridFramework;
using System;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] GameObject _playerToken;
    [SerializeField] GameObject _enemyToken;


    [SerializeField] float _spawnedZOffset;
    float _spawnedYLocation;

    GridXY<Token> _board;
    public static Action<MoveData> OnTokenDropped;
    public static Action OnTokenFailedToDrop;
    public static Action<Token> OnTurnStarted;

    Cursor _cursor;
 

    //Denotes which token will be drop/which player's turn it is
    Token currentPlayer = Token.Player;

    private void Awake()
    {
        _cursor = FindObjectOfType<Cursor>();
    }
    private void Start()
    {
        _board = GridComponent.GetBoard();
        //lets subcribers know that first turn started 
        OnTurnStarted?.Invoke(currentPlayer);

        


    }
    private void OnEnable()
    {
        _cursor.OnClicked += DropToken;
        OnTokenDropped += SpawnToken;
        
    }
    private void OnDisable()
    {
        _cursor.OnClicked -= DropToken;
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
                Vector2Int droppedTokenCoords = new Vector2Int(column, rowToCheck);
                _board.SetCellContent(droppedTokenCoords, currentPlayer); //Set the cell content to the right token.

                
                //Inform subcribers that the token has been dropped; 
                OnTokenDropped?.Invoke(new MoveData(currentPlayer, droppedTokenCoords));

                SwitchTurn();
                return; //Once the token is in grid, no need to continue checking for free spaces;
            }
        }
        //will spawn sound feedback here or smth
        Debug.Log("Token not dropped.");
        OnTokenFailedToDrop?.Invoke();
    }
    
    /// <summary>
    /// Switches player turn to enemy turn and vice versa.
    /// </summary>
    void SwitchTurn()
    {
        /* Turn switching */
        if (currentPlayer == Token.Player) currentPlayer = Token.Enemy;      //If player turn, switch to enemy
        else if (currentPlayer == Token.Enemy) currentPlayer = Token.Player; //If enemy turn, switch to player.
        /**/
        OnTurnStarted?.Invoke(currentPlayer);
    }
    void SpawnToken(MoveData eventData)
    {
        if (_board.CheckInBounds(eventData.tokenCoords)) //only execute if location is valid
        {
            GameObject toInstantiate = null;
            if (eventData.currentPlayer == Token.Player)
            {
                toInstantiate = _playerToken;
            }
            else if (eventData.currentPlayer == Token.Enemy)
            {
                toInstantiate = _enemyToken;
            }
            else
            {
                Debug.LogError("Invalid token.");
            }

            
            //this is target destination of the tokens
            Vector3 targetLocation = _board.GridToWorldPosition(eventData.tokenCoords) + _board.GetCellOffset();
            targetLocation = new Vector3(targetLocation.x, targetLocation.y, _spawnedZOffset);

            //This is from where the tokens spawn
            _spawnedYLocation = _cursor.GetCursorLocation().y;
            Vector3 spawnLocation = new Vector3(targetLocation.x, _spawnedYLocation, targetLocation.z);

            //token spawned
            GameObject newToken = Instantiate(toInstantiate, spawnLocation, Quaternion.identity);
            newToken.GetComponent<TokenMovement>().SetTargetLocation(targetLocation);
        }
    }
}
