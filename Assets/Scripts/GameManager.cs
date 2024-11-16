using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static KeyCode _restart = KeyCode.R;

    private void Update()
    {
        if (Input.GetKeyDown(_restart))
        {
            RestartGame();
        }
    }
    public static void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static KeyCode GetRestartKey() { return _restart; }
}
