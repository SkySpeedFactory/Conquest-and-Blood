using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    private Camera cam;
    private NavMeshAgent unitAgent;

    private Camera cameraRTS;
    private Camera cameraFPS;

    private Unit unitStats;
    private UnitOffensiveBehaviour unitOffensiveBehaviour;
    private Gatherer gatherer;

    // Layers
    [SerializeField] LayerMask ground;

    private float rotationSpeed = 5f;

    [SerializeField] List<AudioClip> movementAudioClipList = new List<AudioClip>();

    private void Awake()
    {
        unitAgent = GetComponent<NavMeshAgent>();
        unitStats = gameObject.GetComponent<Unit>();
        unitOffensiveBehaviour = gameObject.GetComponent<UnitOffensiveBehaviour>();
        unitAgent.speed = unitStats.GetUnitMovementSpeed();
        gatherer = gameObject.GetComponent<Gatherer>();
        
    }

    void Update()
    {
        ChangeCameraForRaycast();
        InterractAndMove();
    }

    public AudioClip GetRandomMovementClip() => movementAudioClipList[Random.Range(0, movementAudioClipList.Count)];
    public AudioClip GetFirstMovementClip() => movementAudioClipList[0];

    public void MoveToTarget(Vector3 targetPos)
    {
        unitAgent.SetDestination(targetPos);
        unitAgent.stoppingDistance = 0f;
        this.enabled = false;
    }

    private void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    private Vector3 FormationWalk()
    {
        if (UnitSelections.Instance.GetSelectedUnitsList().Count > 1)
        {
            for (int i = 0; i < UnitSelections.Instance.GetSelectedUnitsList().Count; i++)
            {
                if (UnitSelections.Instance.GetSelectedUnitsList()[i] == this.gameObject)
                {
                    return UnitFormation.Instance.GetFormationPositionList()[i];
                }
            }
        }

        return UnitFormation.Instance.GetFormationPositionList()[0];
    }

    private void InterractAndMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // change cam
            Ray ray = GetCameraMouseRay();
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Ground"))
                {
                    UnitFormation.Instance.GetFormationPosition(hit.point, this.gameObject);
                    unitOffensiveBehaviour.SetAsTarget(null);
                    unitAgent.stoppingDistance = 0f;
                    unitAgent.speed = unitStats.GetUnitMovementSpeed();
                    unitAgent.SetDestination(FormationWalk());
                    LookAtTarget(hit.point);
                                        
                    if (unitStats.CheckIfUnitCanHarvest())
                    {
                        gatherer.ChangeGathererState(GathererState.Idle);
                        if(gatherer.GetCurrentResourceNode())
                        {
                            gatherer.GetCurrentResourceNode().GetComponent<Resource>().DecreaseAmountOfWorkersOnNode();
                            gatherer.SetResourceNodeToNull();
                        }
                    }
                }
                else if (hit.transform.CompareTag("Resource") && unitStats.CheckIfUnitCanHarvest())
                {
                    unitAgent.stoppingDistance = 7f;
                    unitAgent.speed = unitStats.GetUnitMovementSpeed();
                    unitAgent.SetDestination(hit.transform.position);
                    gatherer.SetResourceNode(hit.transform);
                    gatherer.ChangeGathererState(GathererState.Harvest);
                }
                else if (hit.transform.gameObject.GetComponent<Unit>() != null &&
                    Vector3.Distance(transform.position, hit.transform.position) < unitStats.GetUnitVisibility())
                {
                    if (hit.transform.gameObject.GetComponent<Unit>().GetUnitFaction() == Faction.CPU)
                    {
                        unitOffensiveBehaviour.SetUnitBehavior(1);
                        unitOffensiveBehaviour.SetAsTarget(hit.transform);
                    }
                }
                else if (hit.transform.CompareTag("Building"))
                {
                    if (hit.transform.GetComponent<Building>().GetBuildingFaction() == Faction.Player_1 &&
                        unitStats.GetUnitType() == UnitType.Worker)
                    {
                        if (hit.transform.Find("BuildingPrefab").gameObject.activeInHierarchy && 
                            hit.transform.Find("BuildingPrefab").GetComponent<Resource>() != null)
                        {
                            // is farm
                            unitAgent.stoppingDistance = 5f;
                            unitAgent.speed = unitStats.GetUnitMovementSpeed();
                            unitAgent.SetDestination(hit.transform.position);
                            gatherer.SetResourceNode(hit.transform.Find("BuildingPrefab"));
                            gatherer.ChangeGathererState(GathererState.Harvest);
                        }
                        else
                        {
                            gatherer.SetBuildingToBuild(hit.transform);
                            gatherer.ChangeGathererState(GathererState.Build);
                            unitAgent.stoppingDistance = hit.transform.GetComponent<Building>().GetStoppingDistance();
                            unitAgent.speed = unitStats.GetUnitMovementSpeed();
                            unitAgent.SetDestination(hit.transform.position);

                            if (gatherer.GetCurrentResourceNode())
                            {
                                gatherer.GetCurrentResourceNode().GetComponent<Resource>().DecreaseAmountOfWorkersOnNode();
                                gatherer.SetResourceNodeToNull();
                            }
                        }
                    }
                    else if(hit.transform.GetComponent<Building>().GetBuildingFaction() == Faction.CPU &&
                        unitStats.GetUnitType() != UnitType.Worker)
                    {
                        unitOffensiveBehaviour.SetUnitBehavior(1);
                        unitOffensiveBehaviour.SetAsTarget(hit.transform);
                    }
                }
            }
        }
    }

    public NavMeshAgent GetUnitAgent() => unitAgent;

    public Ray GetCameraMouseRay() => cam.ScreenPointToRay(Input.mousePosition);

    private void ChangeCameraForRaycast()
    {
        if (cameraRTS != null)
        {
            if (cameraRTS.transform.parent.gameObject.activeInHierarchy == true)
            {
                cam = cameraRTS;
            }
        }
        else
        {
            if (CameraUIRotation.Instance)
            {
                cameraRTS = CameraUIRotation.Instance.GetComponent<Camera>();
            }
            
        }

        if (cameraFPS != null)
        {
            if (cameraFPS.gameObject.activeInHierarchy == true)
            {
                cam = cameraFPS;
            }
        }
        else
        {
            cameraFPS = CameraUIRotationFPS.Instance.GetComponent<Camera>();
        }

        
    }
    
}
