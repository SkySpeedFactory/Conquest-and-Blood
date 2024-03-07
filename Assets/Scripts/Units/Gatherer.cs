using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gatherer : MonoBehaviour
{
    // Unit infos
    private Unit unit;
    // Harvest path
    private Transform townCenter;
    private Transform resourceNode;
    Transform buildingToBuild;

    // Harvest settings
    private GathererState gathererState = GathererState.Idle;
    private float autoHarvestRange = 50f;
    private float detectStorageBuildingRange = 500f;
    private float baseHarvestingSpeed = 3f;
    private int baseMaxPortableResources = 10;
    private int maxPortableResources;
    private ResourceType resourceType;
    private int currentAmountOfResources = 0;

    // Build settings
    private float baseBuildSpeed = 0.5f;
    private float buildCountdown;


    // Timer for harvest interval
    private float countdown;

    [SerializeField] AudioClip hitRockClip;
    [SerializeField] AudioClip hitTreeClip;

    private void Awake()
    {
        maxPortableResources = baseMaxPortableResources;
    }

    // Start is called before the first frame update
    void Start()
    {
        unit = gameObject.GetComponent<Unit>();
        countdown = baseHarvestingSpeed;
        buildCountdown = baseBuildSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGathererTask();
    }

    public Transform GetCurrentResourceNode() => resourceNode;

    public void SetResourceNodeToNull()
    {
        resourceNode = null;
    }

    public AudioClip GetHitRockClip() => hitRockClip;
    public AudioClip GetHitTreeClip() => hitTreeClip;

    private void GatherResource()
    {
        if (currentAmountOfResources < maxPortableResources)
        {
            
            countdown -= Time.deltaTime;
            if (countdown * resourceNode.GetComponent<Resource>().GetResourceMalus() <= 0)
            {
                if (resourceNode.GetComponent<Resource>().GetResourceAmount() > 0)
                {
                    resourceType = resourceNode.GetComponent<Resource>().GetResourceType();
                    gameObject.GetComponentInChildren<UnitAnimation>().GatherResources(true, resourceType);
                    // collect resource
                    currentAmountOfResources++;
                    resourceNode.GetComponent<Resource>().DecreaseResourceAmount();
                    countdown = baseHarvestingSpeed;
                }
            }
            if (resourceNode.GetComponent<Resource>().GetResourceAmount() <= 0 && currentAmountOfResources < maxPortableResources)
            {
                resourceNode.GetComponent<Resource>().DecreaseAmountOfWorkersOnNode();
                AutoFindResourceNode(transform.position, autoHarvestRange);
            }
        }
        else
        {
            gameObject.GetComponentInChildren<UnitAnimation>().GatherResources(false, resourceType);
            countdown = baseHarvestingSpeed;
            AutoFindStorageBuilding(transform.position, detectStorageBuildingRange);
        }
    }



    private void WalkBackToResourceNode()
    {
        if (townCenter != null)
        {
            if (Vector3.Distance(transform.position, townCenter.position) <= 25f)
            {
                PlayerManager.Instance.SetPlayerResources(resourceType, currentAmountOfResources);

                currentAmountOfResources = 0;
                gameObject.GetComponent<UnitMovement>().GetUnitAgent().SetDestination(resourceNode.position);
            }
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
                if (CheckIfBuildingIsClose())
                {
                    BuildBuilding(buildingToBuild);
                }
                break;
            case GathererState.Idle:
                break;
            case GathererState.Harvest:
                if (gathererState == GathererState.Harvest)
                {
                    if (Vector3.Distance(transform.position, resourceNode.position) <= 5f)
                    {
                        GatherResource();
                    }
                    if (currentAmountOfResources >= maxPortableResources)
                    {
                        WalkBackToResourceNode();
                    }
                }
                break;
        }
    }

    private void BuildBuilding(Transform target)
    {
        if(buildingToBuild == null ||
            buildingToBuild.GetComponent<Building>().GetHealth() == buildingToBuild.GetComponent<Building>().GetMaxHealth())
        {
            return;
        }
        gameObject.GetComponentInChildren<UnitAnimation>().Build();
        buildCountdown -= Time.deltaTime;
        if(buildCountdown <= 0)
        {
            Building building = buildingToBuild.GetComponent<Building>();
            if (!building.CheckIfBuildingIsFinished())
            {
                BuildProgress progress = target.GetComponent<BuildProgress>();
                progress.SetBuildProgress(5);
                building.SetCurrentHealth((int)building.GetMaxHealth() / 100 * buildingToBuild.GetComponent<BuildProgress>().GetBuildProgress());
                buildCountdown = baseBuildSpeed;
                if (progress.CheckIfBuildIsFinished())
                {
                    target.GetComponent<Building>().FinishBuild();
                    ChangeGathererState(GathererState.Idle);
                }
            }
            else if(building.CheckIfBuildingIsFinished() && building.GetHealth() < building.GetMaxHealth())
            {
                building.RepairBuilding(building.GetMaxHealth()/20f);
                buildCountdown = baseBuildSpeed;
                if(building.GetHealth() == building.GetMaxHealth())
                {
                    ChangeGathererState(GathererState.Idle);
                }
            }
        }
    }

    public Transform GetBuildingToBuild() => buildingToBuild;

    public void SetBuildingToBuild(Transform building) => buildingToBuild = building;

    public bool CheckIfBuildingIsClose()
    {
        if(buildingToBuild != null)
        {
            if (Vector3.Distance(this.gameObject.transform.position, buildingToBuild.position) 
                <= buildingToBuild.GetComponent<Building>().GetStoppingDistance())
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public void SetResourceNode(Transform selectedResourceNode) 
    {
        if(resourceNode != null && resourceNode.GetInstanceID() != selectedResourceNode.GetInstanceID())
        {
            resourceNode.GetComponent<Resource>().DecreaseAmountOfWorkersOnNode(); // FUNCTIONS
        }
        Resource resource = selectedResourceNode.GetComponent<Resource>();
        resourceNode = selectedResourceNode;
        if (!resource.IsCapacityReached())
        {
            resource.IncreaseAmountOfWorkersOnNode();
        }
        else
        {
            AutoFindResourceNode(resourceNode.position, autoHarvestRange);
        }
    }

    private void OnDrawGizmos()
    {
        // Auto Harvest
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, autoHarvestRange);

        // Detect storagebuilding
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectStorageBuildingRange);
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
                if (resource.GetResourceType() == resourceNode.GetComponent<Resource>().GetResourceType() 
                    && resource.GetResourceAmount() > 0 && !resource.IsCapacityReached())
                {
                    resource.IncreaseAmountOfWorkersOnNode();
                    resourceNode = hitCollider.transform;
                    gameObject.GetComponent<UnitMovement>().GetUnitAgent().SetDestination(resourceNode.position);

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

    void AutoFindStorageBuilding(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        Transform nearestStorageBuilding = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Building") && hitCollider.GetComponent<Building>().GetBuildingFaction() == gameObject.GetComponent<Unit>().GetUnitFaction())
            {
                if (hitCollider.GetComponent<Building>().CheckIfBuildingHasResourceStorage())
                {
                    Vector3 directionToTarget = hitCollider.transform.position - transform.position;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        nearestStorageBuilding = hitCollider.transform;
                    }
                }
            }
        }
        if (nearestStorageBuilding != null)
        {
            townCenter = nearestStorageBuilding;
            gameObject.GetComponent<UnitMovement>().GetUnitAgent().SetDestination(townCenter.position);
        }
    }

    public void CarryingcapacityWithUpgrades(int upgradeValue)
    {
        maxPortableResources = baseMaxPortableResources + upgradeValue;
    }
}
