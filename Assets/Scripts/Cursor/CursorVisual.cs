using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorVisual : MonoBehaviour
{
    [SerializeField] Material _playerSkin;
    [SerializeField] Material _enemySkin;
    [SerializeField]MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        BoardController.OnTurnStarted += SwitchTokenSkin;
    }
    private void OnDisable()
    {
        BoardController.OnTurnStarted -= SwitchTokenSkin;
    }

    void SwitchTokenSkin(Token currentPlayer)
    {
        if      (currentPlayer == Token.Player) _meshRenderer.material = _playerSkin;
        else if (currentPlayer == Token.Enemy ) _meshRenderer.material = _enemySkin;
    }
}
