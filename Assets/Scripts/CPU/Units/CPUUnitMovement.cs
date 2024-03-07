using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CPUUnitMovement : MonoBehaviour
{
    private NavMeshAgent unitAgent;
    private Unit unitStats;

    private float rotationSpeed = 5f;

    [SerializeField] List<AudioClip> movementAudioClipList = new List<AudioClip>();

    void Awake()
    {
        unitAgent = GetComponent<NavMeshAgent>();
        unitStats = gameObject.GetComponent<Unit>();
    }

    public AudioClip GetRandomMovementClip() => movementAudioClipList[Random.Range(0, movementAudioClipList.Count)];
    public AudioClip GetFirstMovementClip() => movementAudioClipList[0];

    public void Move(Transform target, bool isAttackPosition = false)
    { 
        if (target != null)
        {
            unitAgent.speed = unitStats.GetUnitMovementSpeed();
            if (isAttackPosition)
            {
                unitAgent.stoppingDistance = (unitStats.GetUnitVisibility() / 2);
                unitAgent.SetDestination(FormationWalk());
                LookAtTarget(FormationWalk());
            }
            else
            {
                unitAgent.SetDestination(target.position); // HERE
                LookAtTarget(target.position);
            }
            
            if (unitStats.CheckIfUnitCanHarvest() && !isAttackPosition)
            {
                // Rebuild to switch
                if (target.CompareTag("Resource"))
                {   
                    gameObject.GetComponent<CPUGatherer>().ChangeGathererState(GathererState.Harvest);
                    gameObject.GetComponent<CPUGatherer>().SetResourceNode(target);
                    unitAgent.stoppingDistance = 5f;
                }
                if (target.CompareTag("Foundation"))
                {
                    gameObject.GetComponent<CPUGatherer>().ChangeGathererState(GathererState.Build);
                    gameObject.GetComponent<CPUGatherer>().SetResourceNode(target);
                    unitAgent.stoppingDistance = 5f;
                }
            }
        }
    }

    private void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }
    public NavMeshAgent GetUnitAgent() => unitAgent;

    private Vector3 FormationWalk()
    {
        for (int i = 0; i < CPUUnitHandler.Instance.GetCPUMilitaryUnitList().Count; i++)
        {
            if (CPUUnitHandler.Instance.GetCPUMilitaryUnitList()[i] == this.gameObject)
            {
                return UnitFormation.Instance.GetFormationPositionListCPU()[i];
            }
        }
        return UnitFormation.Instance.GetFormationPositionListCPU()[0];
    }
}
