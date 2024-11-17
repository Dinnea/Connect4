using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicBetweenScenes : MonoBehaviour
{
    public static BackgroundMusicBetweenScenes instance;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
