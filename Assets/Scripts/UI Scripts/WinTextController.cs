using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinTextController : MonoBehaviour
{
    TextMeshProUGUI _textMesh;
    Image _image;

    private void Awake()
    {
        _image = GetComponentInParent<Image>();
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();

        _image.enabled = false;
        _textMesh.enabled = false;
    }

    private void OnEnable()
    {
        WinChecker.OnWin += WinScreen;
    }
    private void OnDisable()
    {
        WinChecker.OnWin -= WinScreen;
    }

    void WinScreen(Token player)
    {
        _image.enabled = true;
        _textMesh.enabled = true;
        _textMesh.text = player.ToString() + " Wins!";
    }
}
