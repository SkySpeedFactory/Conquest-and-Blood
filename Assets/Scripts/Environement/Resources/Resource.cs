using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{

    [SerializeField] ScriptableResource resourceData;
    private ResourceType resourceType;
    private int resourceAmount;
    private int resourceMalus;
    private bool isResourceInfinity;
    private int workerCapacity;
    private GameObject visualResourceStatus;
    private int workersOnNode;

    void Start()
    {
        resourceType = resourceData.TypeOfResource;
        resourceAmount = resourceData.ResourceAmount;
        resourceMalus = resourceData.ResourceMalus;
        isResourceInfinity = resourceData.InfinityResource;
        workerCapacity = resourceData.WorkerCapacity;
        visualResourceStatus = transform.GetChild(0).gameObject;
        workersOnNode = 0;
    }

    public int GetResourceMalus() => resourceMalus;
    public int GetAmountOfWorkersOnNode() => workersOnNode;
    public bool IsCapacityReached() => workersOnNode < workerCapacity ? false : true;
    public ResourceType GetResourceType() => resourceType;
    public int GetResourceAmount() => resourceAmount;
    private void ChangeVisualResourceNodeStatus(bool state) => visualResourceStatus.SetActive(state);

    public void IncreaseAmountOfWorkersOnNode()
    {
        if (workersOnNode < workerCapacity)
        {
            workersOnNode++;
        }
    }

    public void DecreaseAmountOfWorkersOnNode()
    {
        if(workersOnNode > 0)
        {
            workersOnNode--;
        }
    }
    public void DecreaseResourceAmount()
    {
        if (!isResourceInfinity)
        {
            resourceAmount--;
            if (resourceAmount <= 0)
            {
                ChangeVisualResourceNodeStatus(false);
                // Start co-routine if resource can regenerate
            }
        }
    }


}
