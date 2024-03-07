using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Building : MonoBehaviour
{
    
    // Instances
    private PlaceFoundation placeFoundation;
    private GameManager gameManager;

    // Prefab
    private GameObject buildingPrefab = null;
    private GameObject foundationPrefab = null;
    private GameObject gatherPoint = null;
    private Color defaultColor;

    // Player Unit Queue
    private Queue<GameObject> spawnQueue = new Queue<GameObject>();
    private List<Vector3> spawnPositionList = new List<Vector3>();

    private int unitSpawnPos = 0;
    private int unitsToSpawn = 0;
    private bool hasResourceStorage = false;
    private float storageOffset;
    private bool isSpawnUnitRunning = false;
    private bool isBuildingFinished = false;
    [SerializeField] float stoppingDistance;
    [SerializeField] bool isTownCenter;

    // Building Data
    [SerializeField] ScriptableBuilding buildingData;
    private string buildingName;
    private Sprite sprite;
    private Faction buildingFaction;
    private float maxHealth;
    private float currentHealth;
    private float armor;
    private bool canRecruit;
    private bool hasTechnology;
    private int maxBuildLimit;
    private List<ScriptableUpgrades> upgrades;
    private List<GameObject> availableUnits;
    private int foodCost;
    private int goldCost;
    private int ironCost;
    private int stoneCost;
    private int woodCost;

    // Sound effects
    [SerializeField] AudioClip buildFinishedClip;
    [SerializeField] AudioClip buidlingDamagedClip;
    [SerializeField] ParticleSystem[] buildingDamageEffects;


    private void Awake()
    {
        SetBuildingData();
    }
    private void Start()
    {
        // Prefab
        buildingPrefab = this.transform.Find("BuildingPrefab").gameObject;
        foundationPrefab = this.transform.Find("FoundationPrefab").gameObject;
        gatherPoint = buildingPrefab.transform.Find("GatherPoint").gameObject;
        defaultColor = buildingPrefab.GetComponent<MeshRenderer>().material.color;

        // Instance
        spawnPositionList = UnitFormation.Instance.GetSpawnPositionList(gatherPoint.transform.position);
        placeFoundation = PlaceFoundation.Instance;
        gameManager = GameManager.Instance;

        if(buildingFaction == Faction.CPU)
        {
            CPUBuildingManager.Instance.AddBuildingToBuildingsList(this.gameObject);
        }

        if (isTownCenter)
        {
            FinishBuild();
            if(buildingFaction == Faction.Player_1)
            {
                placeFoundation.IncrementBuildCount(0);
                if (!placeFoundation.GetInstBuildingsList().Contains(this.gameObject))
                {
                    placeFoundation.AddToBuildingsList(this.gameObject);
                }
            }
        }
    }

    // Player Unit Spawning
    public void Spawn(int unit)
    {
        if (!isSpawnUnitRunning)
        {
            unitsToSpawn++;
            spawnQueue.Enqueue(availableUnits[unit]);
            StartCoroutine(SpawnQueue());
            isSpawnUnitRunning = true;
        }
        else
        {
            spawnQueue.Enqueue(availableUnits[unit]);
            unitsToSpawn++;
        }
    }
    private IEnumerator SpawnQueue()
    {
        while(unitsToSpawn > 0)
        {
            SpawnUnit();
            yield return new WaitForSeconds(3f);
        }
        unitSpawnPos = 0;
        isSpawnUnitRunning = false;
        yield break;
    }
    private void SpawnUnit()
    {
        GameObject unit = spawnQueue.Dequeue();
        GameObject spawnedUnit = Instantiate(unit, gatherPoint.transform.position + new Vector3(0, 0, 6), buildingPrefab.transform.rotation);
        UnitMovement unitMovement = spawnedUnit.GetComponent<UnitMovement>();
        unitMovement.enabled = true;
        unitMovement.MoveToTarget(spawnPositionList[unitSpawnPos]);
        unitSpawnPos++;
        unitsToSpawn--;
    }

    // CPU Unit Spawning
    public GameObject SpawnCPUUnit(int unitIndex)
    {
        GameObject unitToSpawn = Instantiate(buildingData.AvailableUnits[unitIndex], transform.position, Quaternion.identity);
        unitToSpawn.GetComponent<CPUUnitMovement>().Move(gatherPoint.transform);
        return unitToSpawn;
    }


    // Setters
    private void SetBuildingData()
    {
        buildingName = buildingData.BuildingName;
        sprite = buildingData.Sprite;
        buildingFaction = buildingData.BuildingFaction;
        maxHealth = buildingData.MaxHealth;
        armor = buildingData.Armor;
        canRecruit = buildingData.CanRecruit;
        hasTechnology = buildingData.HasTechnology;
        hasResourceStorage = buildingData.HasStorage;
        storageOffset = buildingData.StorageOffset;
        maxBuildLimit = buildingData.MaxBuildLimit;
        upgrades = buildingData.Upgrades;
        availableUnits = buildingData.AvailableUnits;
        foodCost = buildingData.FoodCost;
        goldCost = buildingData.GoldCost;
        ironCost = buildingData.IronCost;
        stoneCost = buildingData.StoneCost;
        woodCost = buildingData.WoodCost;
        currentHealth = maxHealth;
    }
    public void SetBuildingFaction(Faction faction) => buildingFaction = faction;
    public void FinishBuild()
    {
        foundationPrefab.SetActive(false);
        buildingPrefab.SetActive(true);
        buildingPrefab.layer = 10;
        if(buildingPrefab.GetComponent<NavMeshObstacle>() != null)
        {
            buildingPrefab.GetComponent<NavMeshObstacle>().enabled = true;
        }
        isBuildingFinished = true;
        gameObject.GetComponent<AudioSource>().PlayOneShot(buildFinishedClip);
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<BuildProgress>().FinishBuildProgress();
        if (buildingPrefab.transform.Find("Interaction") != null)
        {
            buildingPrefab.transform.Find("Interaction").gameObject.SetActive(true);
        }
    }
    public void SetFoundation()
    {
        buildingPrefab.SetActive(false);
        foundationPrefab.SetActive(true);
        SetCurrentHealth(1);
        switch (buildingName)
        {
            case "Town Center":
                PlaceFoundation.Instance.IncrementBuildCount(0);
                break;
            case "Barracks":
                PlaceFoundation.Instance.IncrementBuildCount(1);
                break;
            case "Mage Tower":
                PlaceFoundation.Instance.IncrementBuildCount(2);
                break;
            case "Farm":
                PlaceFoundation.Instance.IncrementBuildCount(3);
                break;
            case "Forge":
                PlaceFoundation.Instance.IncrementBuildCount(4);
                break;
            case "Siege Workshop":
                PlaceFoundation.Instance.IncrementBuildCount(5);
                break;
            case "Warehouse":
                PlaceFoundation.Instance.IncrementBuildCount(6);
                break;
            default:
                Debug.LogWarning($"Building {buildingName} not found");
                break;
        }
    }
    public void DefaultColor() => buildingPrefab.GetComponent<MeshRenderer>().materials[0].color = defaultColor;
    public void SelectedColor() => buildingPrefab.GetComponent<MeshRenderer>().material.color = Color.yellow;

    public void SetCurrentHealth(int amount)
    {
        currentHealth = amount;
        gameObject.GetComponentInChildren<BuildingsHealthbar>().SetHealth(currentHealth);
    }

    // Getters
    public int GetUnitsToSpawn() => unitsToSpawn;
    public float GetArmor() => armor;
    public bool CheckIfBuildingHasResourceStorage() => hasResourceStorage;
    public float GetStorageOffset() => storageOffset;
    public Faction GetBuildingFaction() => buildingFaction;
    public int GetNumberOfUnitTypes() => availableUnits.Count;
    public float GetStoppingDistance() => stoppingDistance;
    public Sprite GetUnitSprite(int unitIndex) => availableUnits[unitIndex].GetComponent<Unit>().GetUnitSprite();
    public string GetUnitName(int unitIndex) => availableUnits[unitIndex].GetComponent<Unit>().GetUnitName();
    public bool CheckIfBuildingIsFinished() => isBuildingFinished;
    public string GetBuildingName() => buildingName;
    public Sprite GetBuildingSprite() => sprite;
    public int GetWoodCost() => woodCost;
    public int GetStoneCost() => stoneCost;
    public int GetUnitFoodCost(int unitIndex) => availableUnits[unitIndex].GetComponent<Unit>().GetFoodCost();
    public List<GameObject> GetAvailableUnits() => availableUnits;
    public int GetMaxBuildLimit() => maxBuildLimit;
    public List<ScriptableUpgrades> GetUpgrades() => upgrades;
    public float GetMaxHealth() => maxHealth;
    public float GetHealth() => currentHealth;


    // Health and Damage related functions
    private void OnDestroy()
    {
        if(PlayerController.Instance.IsPlayerInRTS() && buildingFaction == Faction.Player_1 && buildingName == "Mage Tower")
        {
            PlayerController.Instance.ActvateFPS();
        }
        if (placeFoundation != null && buildingFaction == Faction.Player_1)
        {
            placeFoundation.RemoveFromBuildingsList(this.gameObject);
            gameManager.CheckConquestWinConditions();
        }
    }
    
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if(currentHealth / maxHealth <= 0.3f)
        {
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.Stop();
            audioSource.loop = true;
            audioSource.clip = buidlingDamagedClip;
            audioSource.Play();
            for (int i = 0; i < buildingDamageEffects.Length; i++)
            {
                buildingDamageEffects[i].Play();
            }
        }
        else
        {
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
            for (int i = 0; i < buildingDamageEffects.Length; i++)
            {
                buildingDamageEffects[i].Stop();
            }
        }

        gameObject.GetComponentInChildren<BuildingsHealthbar>().SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            if(buildingFaction == Faction.CPU)
            {
                DisableCPUBuilding();
            }
            else if(buildingFaction == Faction.Player_1)
            {
                Destroy(this.gameObject);
            }
            gameManager.CheckConquestWinConditions();
        }
    }

    public void DisableCPUBuilding()
    {
        buildingPrefab.SetActive(false);
        foundationPrefab.SetActive(true);
        gameObject.GetComponent<MapObject>().DeactivateIcon();
        gameObject.SetActive(false);
        gameObject.GetComponent<BuildProgress>().ResetBuildProgress();
        gameManager.CheckConquestWinConditions();
    }

    /// <summary>
    /// Only for Player Buildings (Just for debug)
    /// </summary>
    public void PrintBuildingData()
    {
        if(buildingFaction == Faction.Player_1)
        {
            print($"Building Name: {buildingName}\n" +
            $"Faction: {buildingFaction}\n" +
            $"Max Health: {maxHealth}\n" +
            $"Armor: {armor}\n" +
            $"Can Recruit: {canRecruit}\n" +
            $"Has Technology: {hasTechnology}\n" +
            $"Number of Available Units: {availableUnits.Count}\n" +
            $"Food Cost: {foodCost}\n" +
            $"Gold Cost: {goldCost}\n" +
            $"Iron Cost: {ironCost}\n" +
            $"Wood Cost: {woodCost}");
        }
    }

    public void RepairBuilding(float repair)
    {
        currentHealth += repair;
        
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
