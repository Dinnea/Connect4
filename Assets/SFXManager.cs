using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioSource _onDropSFX;
    [SerializeField] AudioSource _onFailedDropSFX;
    [SerializeField] AudioSource _onEnemyWinSFX;
    [SerializeField] AudioSource _onPlayerWinSFX;

    private void OnEnable()
    {
        BoardController.OnTokenDropped += PlayOnDropSFX;
        BoardController.OnTokenFailedToDrop += PlayOnFailedDropSFX;
        WinChecker.OnWin += PlayWinSFX;
    }

    private void OnDisable()
    {
        BoardController.OnTokenDropped -= PlayOnDropSFX;
        BoardController.OnTokenFailedToDrop -= PlayOnFailedDropSFX;
        WinChecker.OnWin -= PlayWinSFX;
    }

    void PlayOnDropSFX(MoveData move)
    {
        _onDropSFX.Play();
    }

    void PlayOnFailedDropSFX()
    {
        _onFailedDropSFX.Play();
    }

    void PlayWinSFX(Token player)
    {
        if(player == Token.Player) _onPlayerWinSFX.Play();
        if(player == Token.Enemy) _onEnemyWinSFX.Play();
    }

}
