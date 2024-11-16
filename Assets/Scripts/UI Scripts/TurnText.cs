using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnText : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        BoardController.OnTurnStarted += ChangeText;
        WinChecker.OnWin += DisableText;
    }

    private void OnDisable()
    {
        BoardController.OnTurnStarted -= ChangeText;
        WinChecker.OnWin -= DisableText;
    }

    void ChangeText(Token player)
    {
        //TODO: flip the text maybe? 180 degree. coroutne probably
        textMesh.text = player.ToString() + "'s turn!";
        //another 180 degree
    }

    void DisableText(Token player)
    {
        gameObject.SetActive(false);
    }
}
