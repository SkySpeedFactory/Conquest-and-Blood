using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceFoundation : MonoBehaviour
{
    private static PlaceFoundation _instance;
    public static PlaceFoundation Instance { get { return _instance; } }

    // Instances
    PlayerManager playerManager;

    [SerializeField] List<GameObject> buildingsList = new List<GameObject>(); // Buildings that are available
    List<GameObject> instBuildingsList = new List<GameObject>(); // Instantiated Buildings in game
    List<int> buildCountOfBuildings = new List<int>();
    List<int> maxBuildCountOfBuildings = new List<int>();
    List<ScriptableBuilding> buildingDataList = new List<ScriptableBuilding>();

    GameObject building = null; // Selected item from buildingsList
    private GameObject selectedBuilding = null; // Instance of GameObject building
    private GameObject foundationPrefab = null;
    private GameObject buildingPrefab = null;

    private Color defaultColor;
    private Color cantPlaceColor;

    public GameObject TownCenter { get; private set; } = null;

    private bool isBuildingSelected = false;
    private bool isFoundationPlaced = false;
    private bool hasSelection = false;
    private bool hasEnoughSpace = false;

    private float buildingRotation = 150f;
    private float buildingToPlayerDistance = 22f;
    [SerializeField] GameObject player = null;

    //Prefab-Data Buildings
    [SerializeField] ScriptableBuilding townCenterData;
    [SerializeField] ScriptableBuilding farmData;
    [SerializeField] ScriptableBuilding warehouseData;
    [SerializeField] ScriptableBuilding barracksData;
    [SerializeField] ScriptableBuilding forgeData;
    [SerializeField] ScriptableBuilding mageTowerData;
    [SerializeField] ScriptableBuilding siegeWorkshopData;

    // Private Constructor to prevent creating instance
    private PlaceFoundation() { }

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
        InitBuildingDataList();
        InitBuildCount();
    }

    private void Start()
    {
        playerManager = PlayerManager.Instance;
        cantPlaceColor = Color.red;
    }
    
    void Update()
    {
        CheckIfHasSelection();
        PlaceOnMousePosition();
        if (isFoundationPlaced)
        {
            isBuildingSelected = false;
            isFoundationPlaced = false;
        }
        if (hasSelection)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetHasSelection(false);
            }
        }
    }

    // Getters
    public List<GameObject> GetInstBuildingsList() => instBuildingsList;
    public List<GameObject> GetBuildingsList() => buildingsList;
    public bool GetIsBuildingSelected() => isBuildingSelected;
    public Sprite GetBuildingSprite(int index) => buildingDataList[index].Sprite;


    public void CheckIfHasSelection()
    {
        if (hasSelection)
        {
            if (selectedBuilding != null)
            {
                CheckHasEnoughSpace(buildingPrefab.transform.position, selectedBuilding.GetComponent<Building>().GetStoppingDistance());
                if(hasEnoughSpace)
                {
                    buildingPrefab.GetComponent<MeshRenderer>().materials[0].color = defaultColor;
                }
                else
                {
                    buildingPrefab.GetComponent<MeshRenderer>().materials[0].color = cantPlaceColor;
                }
            }
            MoveFoundationInHand();
        }
        else
        {
            if(building != null)
            {
                if(selectedBuilding != null)
                {
                    Destroy(selectedBuilding);
                }
                isBuildingSelected = false;
            }
        }
    }
    
    private void CheckHasEnoughSpace(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.CompareTag("Ground") && !hitCollider.CompareTag("Untagged"))
            {
                hasEnoughSpace = false;
                return;
            }
        }
        hasEnoughSpace = true;
    }

    /// <summary>
    /// Keeps foundation on mouse hit.point until placed. 
    /// Can be rotated while holding it by pressing 'R' and 'T'.
    /// </summary>
    private void MoveFoundationInHand()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (selectedBuilding != null && Vector3.Distance(player.transform.position, hit.point) < buildingToPlayerDistance)
            {
                Destroy(selectedBuilding);
                isBuildingSelected = false;
                return; 
            }
            if (!isBuildingSelected)
            {
                if(Vector3.Distance(player.transform.position, hit.point) < buildingToPlayerDistance)
                {
                    return;
                }
                selectedBuilding = Instantiate(building, hit.point, Quaternion.identity);
                foundationPrefab = selectedBuilding.transform.Find("FoundationPrefab").gameObject;
                buildingPrefab = selectedBuilding.transform.Find("BuildingPrefab").gameObject;
                defaultColor = buildingPrefab.GetComponent<MeshRenderer>().materials[0].color;
                foundationPrefab.SetActive(false);
                buildingPrefab.SetActive(true);
                isBuildingSelected = true;
            }
            if (!isFoundationPlaced)
            {
                selectedBuilding.transform.position = hit.point;

                // Rotates foundationToPlace by amount when 'R' or 'T' is pressed
                if (Input.GetKey(KeyCode.R))
                {
                    selectedBuilding.transform.RotateAround(building.transform.position, Vector3.up, -buildingRotation * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.T))
                {
                    selectedBuilding.transform.RotateAround(building.transform.position, Vector3.up, buildingRotation * Time.deltaTime);
                }
            }
        }
    }

    /// <summary>
    /// Places selected foundation in hand on current hit.point. 
    /// </summary>
    private void PlaceOnMousePosition()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            else if (isBuildingSelected && !isFoundationPlaced && hasEnoughSpace)
            {
                selectedBuilding.GetComponent<Building>().SetFoundation();
                AddToBuildingsList(selectedBuilding);
                isFoundationPlaced = true;
                hasEnoughSpace = false;
                hasSelection = false;
                SubtractCostFromResources();
                selectedBuilding = null;
            }
        }
    }

    private void SubtractCostFromResources()
    {
        playerManager.SetPlayerResources(ResourceType.Wood, -selectedBuilding.GetComponent<Building>().GetWoodCost());
        playerManager.SetPlayerResources(ResourceType.Stone, -selectedBuilding.GetComponent<Building>().GetStoneCost());
    }

    public void SetBuilding(int index) => building = buildingsList[index];

    public void SetHasSelection(bool isInHand)
    {
        Destroy(selectedBuilding);
        isBuildingSelected = false;
        hasSelection = isInHand;
    }

    public void AddToBuildingsList(GameObject building) => instBuildingsList.Add(building);

    public void RemoveFromBuildingsList(GameObject building) => instBuildingsList.Remove(building);

    private void InitBuildCount()
    {
        for (int i = 0; i < buildingsList.Count; i++)
        {
            buildCountOfBuildings.Add(0);
            maxBuildCountOfBuildings.Add(0);
        }
        for (int i = 0; i < buildingsList.Count; i++)
        {
            maxBuildCountOfBuildings[i] = buildingDataList[i].MaxBuildLimit;
        }
    }
    public void IncrementBuildCount(int index) => buildCountOfBuildings[index] += 1;

    private void InitBuildingDataList()
    {
        buildingDataList.Add(townCenterData);
        buildingDataList.Add(barracksData);
        buildingDataList.Add(mageTowerData);
        buildingDataList.Add(farmData);
        buildingDataList.Add(forgeData);
        buildingDataList.Add(siegeWorkshopData);
        buildingDataList.Add(warehouseData);
    }
    
    public bool CheckIfEnoughResources(int index)
    {
        // Order: Wood, Stone
        List<int> buildingCost = new List<int>() { 0, 0 };

        switch (buildingsList[index].name)
        {
            case "TownCenter":
                buildingCost[0] = townCenterData.WoodCost;
                break;
            case "Farm":
                buildingCost[0] = farmData.WoodCost;
                break;
            case "Warehouse":
                buildingCost[0] = warehouseData.WoodCost;
                break;
            case "Barracks":
                buildingCost[0] = barracksData.WoodCost;
                break;
            case "Forge":
                buildingCost[0] = forgeData.WoodCost;
                buildingCost[1] = forgeData.StoneCost;
                break;
            case "MageTower":
                buildingCost[0] = mageTowerData.WoodCost;
                buildingCost[1] = mageTowerData.StoneCost;
                break;
            case "SiegeWorkshop":
                buildingCost[0] = siegeWorkshopData.WoodCost;
                buildingCost[1] = siegeWorkshopData.StoneCost;
                break;
            default:
                Debug.LogWarning($"Building {buildingsList[index].name} not found");
                break;
        }
        if (buildingCost[0] <= playerManager.GetPlayerResources(ResourceType.Wood) && buildingCost[1]
            <= playerManager.GetPlayerResources(ResourceType.Stone))
        {
            return true;
        }
        return false;
    }

    public bool CheckIfReachedMaxBuildCount(int index)
    {
        if (buildCountOfBuildings[index] >= maxBuildCountOfBuildings[index])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckIfUnlocked(int buildingIndex)
    {
        bool isUnlocked = true;
        switch (buildingIndex)
        {
            case 2: // MageTower
            case 4: // Forge
                if (buildCountOfBuildings[1] <= 0)
                {
                    isUnlocked = false;
                }
                break;
            case 5: // SiegeWorkshop
                if (buildCountOfBuildings[1] <= 0 ||
                    buildCountOfBuildings[4] <= 0)
                {
                    isUnlocked = false;
                }
                break;
            default:
                isUnlocked = true;
                break;
        }
        return isUnlocked;
    }

    public List<int> GetBuildingData(string buildingData)
    {
        // Order: Food, Gold, Iron, Stone, Wood
        List<int> buildingCost = new List<int>() { 0, 0, 0, 0, 0 };

        switch (buildingData)
        {
            case "townCenterData":
                buildingCost[0] = townCenterData.FoodCost;
                buildingCost[1] = townCenterData.GoldCost;
                buildingCost[2] = townCenterData.IronCost;
                buildingCost[3] = townCenterData.StoneCost;
                buildingCost[4] = townCenterData.WoodCost;
                break;
            case "barracksData":
                buildingCost[0] = barracksData.FoodCost;
                buildingCost[1] = barracksData.GoldCost;
                buildingCost[2] = barracksData.IronCost;
                buildingCost[3] = barracksData.StoneCost;
                buildingCost[4] = barracksData.WoodCost;
                break;
            case "forgeData":
                buildingCost[0] = forgeData.FoodCost;
                buildingCost[1] = forgeData.GoldCost;
                buildingCost[2] = forgeData.IronCost;
                buildingCost[3] = forgeData.StoneCost;
                buildingCost[4] = forgeData.WoodCost;
                break;
            case "farmData":
                buildingCost[0] = farmData.FoodCost;
                buildingCost[1] = farmData.GoldCost;
                buildingCost[2] = farmData.IronCost;
                buildingCost[3] = farmData.StoneCost;
                buildingCost[4] = farmData.WoodCost;
                break;
            case "mageTowerData":
                buildingCost[0] = mageTowerData.FoodCost;
                buildingCost[1] = mageTowerData.GoldCost;
                buildingCost[2] = mageTowerData.IronCost;
                buildingCost[3] = mageTowerData.StoneCost;
                buildingCost[4] = mageTowerData.WoodCost;
                break;
            case "siegeWorkshopData":
                buildingCost[0] = siegeWorkshopData.FoodCost;
                buildingCost[1] = siegeWorkshopData.GoldCost;
                buildingCost[2] = siegeWorkshopData.IronCost;
                buildingCost[3] = siegeWorkshopData.StoneCost;
                buildingCost[4] = siegeWorkshopData.WoodCost;
                break;
            case "warehouseData":
                buildingCost[0] = warehouseData.FoodCost;
                buildingCost[1] = warehouseData.GoldCost;
                buildingCost[2] = warehouseData.IronCost;
                buildingCost[3] = warehouseData.StoneCost;
                buildingCost[4] = warehouseData.WoodCost;
                break;
            default:
                Debug.LogWarning($"Unknown building data name: {buildingData}");
                break;
        }
        return buildingCost;
    }
}
