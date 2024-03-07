using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUUnitSpawner : MonoBehaviour
{
    private static CPUUnitSpawner _instance;
    public static CPUUnitSpawner Instance { get { return _instance; } }

    // Private Constructor to prevent creating instance
    private CPUUnitSpawner() { }

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

    public GameObject ChooseUnitToSpawn(Building building, int unitIndex) 
    {
        return building.SpawnCPUUnit(unitIndex);
    } 
}
