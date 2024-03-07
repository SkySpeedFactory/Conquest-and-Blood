using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    private static Crosshair _instance;
    public static Crosshair Instance { get { return _instance; } }

    private bool isInFpsPerspective = false;
    [SerializeField] Image crosshair = null;

    // Private Constructor to prevent creating instance
    private Crosshair() { }

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

    public void ActivateCrosshair(bool isActive)
    {
        if (crosshair != null)
        {
            crosshair.gameObject.SetActive(isActive);
        }
    }
    
    public bool GetIsInFpsPerspective() => isInFpsPerspective;
}
