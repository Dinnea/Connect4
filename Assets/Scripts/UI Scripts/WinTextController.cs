using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinTextController : MonoBehaviour
{
    TextMeshProUGUI _textMesh;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _textMesh.enabled = false;
    }

    private void OnEnable()
    {
        WinChecker.OnWin += ChangeText;
    }
    private void OnDisable()
    {
        WinChecker.OnWin -= ChangeText;
    }

    void ChangeText(Token player)
    {
        _textMesh.enabled = true;
        _textMesh.text = player.ToString() + " Wins!";
    }
}
