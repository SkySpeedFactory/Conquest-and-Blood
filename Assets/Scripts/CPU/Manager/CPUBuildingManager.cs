using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUBuildingManager : MonoBehaviour
{
    [SerializeField] GameObject baseBuildingNodeContainer = null; // Nodes in Base
    [SerializeField] GameObject outpostBuildingNodeContainer = null; // Nodes outside of Base
    private List<GameObject> baseBuildingNodes = new List<GameObject>();
    private List<GameObject> cpuBuildingsList = new List<GameObject>();

    private static CPUBuildingManager _instance;
    public static CPUBuildingManager Instance { get { return _instance; } }

    // Private Constructor to prevent creating instance
    private CPUBuildingManager() { }
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

    // Start is called before the first frame update
    void Start()
    {
        InitializeBuildingNodes(baseBuildingNodeContainer);
        InitializeBuildingNodes(outpostBuildingNodeContainer);
    }

    private void InitializeBuildingNodes(GameObject nodeContainer)
    {
        for (int i = 0; i < nodeContainer.transform.childCount; i++)
        {
            baseBuildingNodes.Add(nodeContainer.transform.GetChild(i).gameObject);
        }
    }

    public GameObject GetSpecificBuildingNode(int index) => baseBuildingNodes[index];

    public List<GameObject> GetStorageBuildings()
    {
        List<GameObject> storageBuildings = new List<GameObject>();
        foreach (var building in cpuBuildingsList)
        {
            if (building.GetComponent<Building>().CheckIfBuildingHasResourceStorage() && building.transform.Find("BuildingPrefab").gameObject.activeInHierarchy)
            {
                storageBuildings.Add(building);
            }
        }
        return storageBuildings;
    }

    public void AddBuildingToBuildingsList(GameObject buildingToAdd) => cpuBuildingsList.Add(buildingToAdd);
    public void RemoveBuildingFromBuildingsList(GameObject buildingToRemove) => cpuBuildingsList.Remove(buildingToRemove);
}
