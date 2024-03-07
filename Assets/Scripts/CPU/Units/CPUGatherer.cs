using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUGatherer : MonoBehaviour
{
    // Instances
    private CPUResourceManager cpuRessourceManager;

    // Harvest path
    private Transform storageBuilding = null;
    private Transform resourceNode = null;

    // Harvest settings
    private GathererState gathererState = GathererState.Idle;
    private float autoHarvestRange = 50f;
    private float baseHarvestingSpeed = 3f;
    private int baseMaxPortableResources = 10;
    private int maxPortableResources;
    private ResourceType resourceType;
    private int currentAmountOfResources = 0;
    private bool foundStorageBuildingComplete = true;

    // Build Settings
    private float baseBuildSpeed = 2f;


    // Timers for interval
    private float countdownHarvest;
    private float countdownBuild;

    [SerializeField] AudioClip hitRockClip;
    [SerializeField] AudioClip hitTreeClip;

    private void Awake()
    {
        maxPortableResources = baseMaxPortableResources;
    }

    // Start is called before the first frame update
    void Start()
    {
        cpuRessourceManager = CPUResourceManager.Instance;
        countdownHarvest = baseHarvestingSpeed;
        countdownBuild = baseBuildSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGathererTask();
    }

    public AudioClip GetHitRockClip() => hitRockClip;
    public AudioClip GetHitTreeClip() => hitTreeClip;

    private void GatherResource()
    {
        if (currentAmountOfResources < maxPortableResources)
        {
            gameObject.GetComponentInChildren<UnitAnimation>().GatherResources(true, resourceType);
            foundStorageBuildingComplete = false;
            countdownHarvest -= Time.deltaTime;
            if (countdownHarvest <= 0)
            {
                if (resourceNode.GetComponent<Resource>().GetResourceAmount() > 0)
                {
                    resourceType = resourceNode.GetComponent<Resource>().GetResourceType();
                    // collect resource
                    currentAmountOfResources++;
                    resourceNode.GetComponent<Resource>().DecreaseResourceAmount();
                    countdownHarvest = baseHarvestingSpeed;
                }
            }
            if (resourceNode.GetComponent<Resource>().GetResourceAmount() <= 0 && currentAmountOfResources < maxPortableResources)
            {
                AutoFindResourceNode(transform.position, autoHarvestRange);
            }
        }
        else
        {
            if (!foundStorageBuildingComplete)
            {
                countdownHarvest = baseHarvestingSpeed;
                AutoFindStorageBuilding();
            }
            gameObject.GetComponentInChildren<UnitAnimation>().GatherResources(false, resourceType);
        }
    }

    private void WalkBackToResourceNode()
    {
        if (storageBuilding != null)
        {
            if (Vector3.Distance(transform.position, storageBuilding.position) <= storageBuilding.GetComponent<Building>().GetStorageOffset())
            {
                cpuRessourceManager.SetCPUResources(resourceType, currentAmountOfResources);
                currentAmountOfResources = 0;
                gameObject.GetComponent<CPUUnitMovement>().GetUnitAgent().SetDestination(resourceNode.position);
            }
        }
    }

    private void BuildBuilding(Transform target)
    {
        target.GetChild(0).gameObject.SetActive(true);
        gameObject.GetComponentInChildren<UnitAnimation>().Build();
        countdownBuild -= Time.deltaTime;
        if (countdownBuild <= 0)
        {
            if (target.GetChild(0).GetComponent<BuildProgress>().GetBuildProgress() == 0)
            {
                cpuRessourceManager.SetCPUResources(ResourceType.Wood, -target.GetChild(0).GetComponent<Building>().GetWoodCost());
                cpuRessourceManager.SetCPUResources(ResourceType.Stone, -target.GetChild(0).GetComponent<Building>().GetStoneCost());
                target.GetChild(0).GetComponent<Building>().SetCurrentHealth(1);
            }
            target.GetChild(0).GetComponent<BuildProgress>().SetBuildProgress(5);
            target.GetChild(0).GetComponent<Building>().SetCurrentHealth((int)target.GetChild(0).GetComponent<Building>().GetMaxHealth() / 100 * target.GetChild(0).GetComponent<BuildProgress>().GetBuildProgress());
            countdownBuild = baseBuildSpeed;
        }
        if (target.GetChild(0).GetComponent<BuildProgress>().CheckIfBuildIsFinished())
        {
            target.GetChild(0).Find("FoundationPrefab").gameObject.SetActive(false);
            target.GetChild(0).Find("BuildingPrefab").gameObject.SetActive(true);
            target.GetChild(0).GetComponent<MapObject>().ActivateIcon();
            ChangeGathererState(GathererState.Idle);
        }
    }

    public void ChangeGathererState(GathererState state)
    {
        gathererState = state;
        if (gathererState == GathererState.Idle)
        {
            gameObject.GetComponentInChildren<UnitAnimation>().DeactivateAllTools();
        }
    }

    private void CheckGathererTask()
    {
        switch (gathererState)
        {
            case GathererState.Build:
                // Start build task
                if (Vector3.Distance(transform.position, resourceNode.position) <= 15f)
                {
                    BuildBuilding(resourceNode);
                }
                break;
            case GathererState.Idle:
                countdownBuild = baseBuildSpeed;
                countdownHarvest = baseHarvestingSpeed;
                break;
            case GathererState.Harvest:
                    if (Vector3.Distance(transform.position, resourceNode.position) <= 5f)
                    {
                        GatherResource();
                    }
                    if (currentAmountOfResources >= maxPortableResources)
                    {
                        WalkBackToResourceNode();
                    }
                break;
        }
    }

    public void SetResourceNode(Transform selectedResourceNode) => resourceNode = selectedResourceNode;

    private void OnDrawGizmos()
    {
        // Auto Harvest
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, autoHarvestRange);
    }

    void AutoFindResourceNode(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        bool foundSameHarvestableResource = false;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Resource"))
            {
                Resource resource = hitCollider.GetComponent<Resource>();
                if (resource.GetResourceType() == resourceNode.GetComponent<Resource>().GetResourceType() && 
                    resource.GetResourceAmount() > 0 && !resource.IsCapacityReached())
                {
                    if(resourceNode != null)
                    {
                        resourceNode.GetComponent<Resource>().DecreaseAmountOfWorkersOnNode();
                    }
                    resourceNode = hitCollider.transform;
                    resourceNode.GetComponent<Resource>().IncreaseAmountOfWorkersOnNode();
                    gameObject.GetComponent<CPUUnitMovement>().GetUnitAgent().SetDestination(resourceNode.position);
                    foundSameHarvestableResource = true;
                    return;
                }
            }
        }
        if (hitColliders.Length > 0 && !foundSameHarvestableResource)
        {
            gathererState = GathererState.Idle;
        }
    }
    void AutoFindStorageBuilding()
    {
        Transform nearestStorageBuilding = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (var storageBuilding in CPUBuildingManager.Instance.GetStorageBuildings())
        {
            Vector3 directionToTarget = storageBuilding.transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                nearestStorageBuilding = storageBuilding.transform;
            }
        }
        if (nearestStorageBuilding != null)
        {
            storageBuilding = nearestStorageBuilding;
            gameObject.GetComponent<CPUUnitMovement>().GetUnitAgent().SetDestination(storageBuilding.position);
        }
        foundStorageBuildingComplete = true;
    }

    public GathererState GetCPUGathererStage() => gathererState;

    public ResourceType GetLastJobResource() => resourceType;

    public void OverrideJob(ResourceType overrideJob)
    {
        resourceType = overrideJob;
    }

    public void CarryingcapacityWithUpgrades(int upgradeValue)
    {
        maxPortableResources = baseMaxPortableResources + upgradeValue;
    }
}
