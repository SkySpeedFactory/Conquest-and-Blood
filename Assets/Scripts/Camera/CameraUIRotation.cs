using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUIRotation : MonoBehaviour
{
    private static CameraUIRotation _instance;
    public static CameraUIRotation Instance { get { return _instance; } }

    // Private Constructor to prevent creating instance
    private CameraUIRotation() { }

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
