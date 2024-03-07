using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUUnitHandler : MonoBehaviour
{
    private List<GameObject> cpuVillagerUnitList = new List<GameObject>();
    private List<GameObject> cpuMilitaryUnitList = new List<GameObject>();
    [SerializeField] Transform townCenterNode = null;
    private float resourceScanerRange = 100;
    private int militaryStrength = 0;
    [SerializeField] GameObject archMagePrefab = null;

    private static CPUUnitHandler _instance;
    public static CPUUnitHandler Instance { get { return _instance; } }

    // Private Constructor to prevent creating instance
    private CPUUnitHandler() { }
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

    public List<GameObject> GetCPUVillagerUnitList() => cpuVillagerUnitList;

    public GameObject GetSpecificVillagerFromList(GameObject villagerToFind)
    {
        return cpuVillagerUnitList.Find((villager) => villager.GetInstanceID() == villagerToFind.GetInstanceID());
    }

    public List<GameObject> GetCPUMilitaryUnitList() => cpuMilitaryUnitList;

    public void CheckIfVillagerIsIdle()
    {
        foreach (var unit in cpuVillagerUnitList)
        {
            if (unit.GetComponent<CPUGatherer>().GetCPUGathererStage() == GathererState.Idle)
            {
              SetSpecificUnitCommand(ConvertResourceTypeToCommand(unit.GetComponent<CPUGatherer>().GetLastJobResource()), unit);
            }
        }
    }

    public bool HasIdleVillager()
    {
        
        foreach (var unit in cpuVillagerUnitList)
        {
            if (unit.GetComponent<CPUGatherer>().GetCPUGathererStage() == GathererState.Idle)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasEnoughVillagerOnResource(ResourceType resourceType, int neededWorkerJobCount)
    {
        int currentIntervalWorkerCount = 0;
        for (int i = 0; i < cpuVillagerUnitList.Count; i++)
        {
            if (cpuVillagerUnitList[i].GetComponent<CPUGatherer>().GetLastJobResource() == resourceType)
            {
                currentIntervalWorkerCount++;
            }
        }
        if (neededWorkerJobCount > currentIntervalWorkerCount)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void AddToCPUVillagerUnitList(GameObject unit) => cpuVillagerUnitList.Add(unit);

    public void RemoveFromCPUVillagerUnitList(GameObject unit) => cpuVillagerUnitList.Remove(unit);

    public void AddToCPUMilitaryUnitList(GameObject unit) => cpuMilitaryUnitList.Add(unit);

    public void RemoveFromCPUMilitaryUnitList(GameObject unit) => cpuMilitaryUnitList.Remove(unit);

    public void SendCommandToVillager(Transform target, GameObject sendToAllVillager = null)
    {
        if (cpuVillagerUnitList.Count > 0)
        {
            if (sendToAllVillager == null)
            {
                foreach (var cpuUnit in cpuVillagerUnitList)
                {
                    if (cpuUnit.GetComponent<CPUGatherer>().GetCPUGathererStage() == GathererState.Idle)
                    {
                        cpuUnit.GetComponent<CPUUnitMovement>().Move(target);
                    }
                }
            }
            else
            {
                sendToAllVillager.GetComponent<CPUUnitMovement>().Move(target);
            }
        }
        else
        {
           Debug.LogWarning("No units left");
        }
    }

    public void SendCommandToMilitaryUnits(Transform target)
    {
        foreach (var cpuUnit in cpuMilitaryUnitList)
        {
            if (cpuUnit == cpuMilitaryUnitList[0])
            {
                UnitFormation.Instance.GetFormationPosition(target.position, cpuUnit);
            }


            cpuUnit.GetComponent<CPUUnitMovement>().Move(target, true);
        }
    }

    public void SendCommandToArchMage(Transform target) => CPUArchMage.Instance.GetComponent<CPUUnitMovement>().Move(target);

    public void SetSpecificUnitCommand(CPUCommandUnit command, GameObject sendToAllVillager = null, int buildingIndexForSameType = 0)
    {
        switch (command)
        {
            case CPUCommandUnit.Build_Barracks:
                // build building at node
                SendCommandToVillager(CPUBuildingManager.Instance.GetSpecificBuildingNode(11).transform, sendToAllVillager);
                break;
            case CPUCommandUnit.Build_Depot:
                // build building at node
                SendCommandToVillager(CPUBuildingManager.Instance.GetSpecificBuildingNode(15 + buildingIndexForSameType).transform, sendToAllVillager);
                break;
            case CPUCommandUnit.Build_Farm:
                // build building at node
                /*
                 * The CPU will not build any farms. Reasons for this are: There is no strategic advantage and it would extremely complicate balancing. 
                 * However, the function has been prepared in such a way that it can easily be added later, should this ever be necessary.
                 */
                break;
            case CPUCommandUnit.Build_Forge:
                // build building at node
                SendCommandToVillager(CPUBuildingManager.Instance.GetSpecificBuildingNode(12).transform, sendToAllVillager);
                break;
            case CPUCommandUnit.Build_Magetower:
                // build building at node
                SendCommandToVillager(CPUBuildingManager.Instance.GetSpecificBuildingNode(13).transform, sendToAllVillager);
                break;
            case CPUCommandUnit.Build_SiegeWorkshop:
                // build building at node
                SendCommandToVillager(CPUBuildingManager.Instance.GetSpecificBuildingNode(14).transform, sendToAllVillager);
                break;
            case CPUCommandUnit.Build_TownCenter:
                // build building at node
                SendCommandToVillager(CPUBuildingManager.Instance.GetSpecificBuildingNode(0).transform, sendToAllVillager);
                break;
            case CPUCommandUnit.Collect_Food:
                SendCommandToVillager(AutoFindNearestResourceNode(townCenterNode.position, resourceScanerRange, ResourceType.Food), sendToAllVillager);
                break;
            case CPUCommandUnit.Collect_Gold:
                SendCommandToVillager(AutoFindNearestResourceNode(townCenterNode.position, resourceScanerRange, ResourceType.Gold), sendToAllVillager);
                break;
            case CPUCommandUnit.Collect_Iron:
                SendCommandToVillager(AutoFindNearestResourceNode(townCenterNode.position, resourceScanerRange, ResourceType.Iron), sendToAllVillager);
                break;
            case CPUCommandUnit.Collect_Stone:
                SendCommandToVillager(AutoFindNearestResourceNode(townCenterNode.position, resourceScanerRange, ResourceType.Stone), sendToAllVillager);
                break;
            case CPUCommandUnit.Collect_Wood:
                SendCommandToVillager(AutoFindNearestResourceNode(townCenterNode.position, resourceScanerRange, ResourceType.Wood), sendToAllVillager);
                break;
            case CPUCommandUnit.Engage_Player:
                // walk to specifc player unit/building position
                break;
            default:
                Debug.LogWarning("Unknown Command!");
                break;
        }
        // will replace the target input. choose the command for the units
    }

    private Transform AutoFindNearestResourceNode(Vector3 center, float radius, ResourceType resourceType)
    {
        Transform nearestResourceNode = null;
        if (resourceScanerRange <= 1000)
        {
            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            float closestDistanceSqr = Mathf.Infinity;
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Resource"))
                {
                    Resource resource = hitCollider.GetComponent<Resource>();
                    if (resource.GetResourceAmount() > 0 && resource.GetResourceType() == resourceType 
                        && !resource.IsCapacityReached())
                    {
                        Vector3 directionToTarget = hitCollider.transform.position - transform.position;
                        float dSqrToTarget = directionToTarget.sqrMagnitude;
                        if (dSqrToTarget < closestDistanceSqr)
                        {
                            closestDistanceSqr = dSqrToTarget;
                            nearestResourceNode = hitCollider.transform;
                        }
                    }
                }
            }
            if (nearestResourceNode == null)
            {
                resourceScanerRange = radius + 100;
            }
        }
        if(nearestResourceNode != null)
        {
            nearestResourceNode.GetComponent<Resource>().IncreaseAmountOfWorkersOnNode();
        }
        return nearestResourceNode;
    }

    public void CalcMilitaryStrength()
    {
        militaryStrength = 0;
        foreach (var unit in cpuMilitaryUnitList)
        {
            militaryStrength += unit.GetComponent<Unit>().GetMilitaryStrength();
        }
    }

    public int GetCPUMilitaryStrength() => militaryStrength;

    private CPUCommandUnit ConvertResourceTypeToCommand(ResourceType resourceType)
    {
        CPUCommandUnit command = CPUCommandUnit.Collect_Food;
        switch (resourceType)
        {
            case ResourceType.Food:
                command = CPUCommandUnit.Collect_Food;
                break;
            case ResourceType.Gold:
                command = CPUCommandUnit.Collect_Gold;
                break;
            case ResourceType.Iron:
                command = CPUCommandUnit.Collect_Iron;
                break;
            case ResourceType.Stone:
                command = CPUCommandUnit.Collect_Stone;
                break;
            case ResourceType.Wood:
                command = CPUCommandUnit.Collect_Wood;
                break;
            default:
                break;
        }
        return command;
    }
    public void RespawnArchMage(Transform archMageSpawner) => Instantiate(archMagePrefab, archMageSpawner.position, Quaternion.identity);

    public float GetCurrentResourceScanerRange() => resourceScanerRange;

}
