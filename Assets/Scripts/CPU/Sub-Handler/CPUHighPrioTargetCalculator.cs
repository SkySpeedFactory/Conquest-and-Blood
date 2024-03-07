using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUHighPrioTargetCalculator : MonoBehaviour
{
    private static CPUHighPrioTargetCalculator _instance;
    public static CPUHighPrioTargetCalculator Instance { get { return _instance; } }


    private CPUManager cpuManager;
    private UnitSelections unitSelections;
    private PlaceFoundation placeFoundation;
    private PlayerController player;

    private float scannerRangeForPlayerUnitsNearBase = 200f;

    // Private Constructor to prevent creating instance
    private CPUHighPrioTargetCalculator() { }

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

    void Start()
    {
        cpuManager = CPUManager.Instance;
        unitSelections = UnitSelections.Instance;
        placeFoundation = PlaceFoundation.Instance;
        player = PlayerController.Instance;
    }

    public List<Transform> CalcCPUHighPrioTarget(int gameMode)
    {
        List<Transform> prioTargetList = new List<Transform>();
        if (GetPlayerUnitsNearBase() != null)
        {
            prioTargetList.Add(GetPlayerUnitsNearBase());
        }
        for (int i = 0; i < placeFoundation.GetInstBuildingsList().Count; i++)
        {
            prioTargetList.Add(placeFoundation.GetInstBuildingsList()[i].transform);
        }
        if (gameMode == 1)
        {
            prioTargetList.Add(player.transform);
        }
        for (int i = 0; i < unitSelections.GetUnitList().Count; i++)
        {
            prioTargetList.Add(unitSelections.GetUnitList()[i].transform);
        }
        if (gameMode != 1)
        {
            prioTargetList.Add(player.transform);
        }
        return prioTargetList;
    }

    private Transform GetPlayerUnitsNearBase()
    {
        Transform baseCenter = cpuManager.GetTownCenter().transform;
        Transform detectedTarget = null;
        if (baseCenter != null)
        {
            for (int i = 0; i < unitSelections.GetUnitList().Count; i++)
            {
                if (Vector3.Distance(unitSelections.GetUnitList()[i].transform.position, baseCenter.position) <= scannerRangeForPlayerUnitsNearBase)
                {
                    detectedTarget = unitSelections.GetUnitList()[i].transform;
                }
            }
        }
        return detectedTarget;
    }
}
