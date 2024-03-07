using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgMainMenu : MonoBehaviour
{
    private static BgMainMenu _instance;
    public static BgMainMenu Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if(level != 0 && level != 1) // Game scene or win/loose scene
        {
            Destroy(this.gameObject);
        }
    }
}
