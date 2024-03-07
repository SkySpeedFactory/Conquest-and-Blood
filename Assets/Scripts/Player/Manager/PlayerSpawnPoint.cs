using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private static PlayerSpawnPoint _instance;
    public static PlayerSpawnPoint Instance { get { return _instance; } }

    [SerializeField] GameObject player = null;
    [SerializeField] GameObject rtsCamera = null;
    [SerializeField] GameObject selectionSystem = null;

    // Private Constructor to prevent creating instance
    private PlayerSpawnPoint() { }
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
      