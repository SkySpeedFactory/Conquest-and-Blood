using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUIRotationFPS : MonoBehaviour
{
    private static CameraUIRotationFPS _instance;
    public static CameraUIRotationFPS Instance { get { return _instance; } }

    // Private Constructor to prevent creating instance
    private CameraUIRotationFPS() { }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
}
