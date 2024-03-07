using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitOffensiveBehaviour : MonoBehaviour
{
    private Unit unitStats;

    private Transform target = null;

    private NavMeshAgent unitAgent;

    private bool isCoroutineRunning = false;
    private UnitBehaviour unitBehaviour = UnitBehaviour.Offensive;

    private float attackRange;

    private float rotationSpeed = 5f;
    private float attackSpeed = 2f;

    [SerializeField] AudioClip attackClip = null;

    // Start is called before the first frame update
    void Start()
    {
        unitStats = gameObject.GetComponent<Unit>();
        unitAgent = gameObject.GetComponent<NavMeshAgent>();
        if (unitStats.GetUnitType() == UnitType.Worker)
        {
            unitBehaviour = UnitBehaviour.Escape;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (target != null && Vector3.Distance(transform.position, target.transform.position) > unitStats.GetUnitVisibility())
        {
            SetAsTarget(null);
            unitAgent.SetDestination(transform.position);
        }
        else if (target == null && !isCoroutineRunning && unitBehaviour != UnitBehaviour.Passive && unitStats.GetUnitType() != UnitType.Worker)
        {
            isCoroutineRunning = true;
            StartCoroutine(LookForTarget());
        }
        else if (target != null && unitBehaviour == UnitBehaviour.Defensive || target != null && unitBehaviour == UnitBehaviour.Offensive)
        {
            Attack();
        }
    }

    private void OnDrawGizmos()
    {
        // Detection Range
        Gizmos.color = Color.red;
        if (unitStats != null && unitStats.CheckIfAllUnitDataInizialized())
        {
            Gizmos.DrawWireSphere(transform.position, unitStats.GetUnitVisibility());
        }
    }

    public AudioClip GetAttackClip() => attackClip;

    private IEnumerator LookForTarget()
    {
        while(target == null)
        {
            yield return new WaitForSeconds(3f);
            if (unitAgent.velocity == Vector3.zero)
            {
                DetectionArea(transform.position, unitStats.GetUnitVisibility());
            }
        }
        isCoroutineRunning = false;
        yield break;
    }

    void DetectionArea(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        Transform nearestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.GetComponent<Unit>() != null)
            {
                
                if (hitCollider.gameObject.GetComponent<Unit>().GetUnitFaction() != unitStats.GetUnitFaction())
                {
                    Vector3 directionToTarget = hitCollider.transform.position - transform.position;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;

                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        nearestTarget = hitCollider.transform;
                    }
                }
                
            }
            else if(hitCollider.gameObject.GetComponent<Building>() != null)
            {
                if (hitCollider.gameObject.GetComponent<Building>().GetBuildingFaction() != unitStats.GetUnitFaction())
                {
                    Vector3 directionToTarget = hitCollider.transform.position - transform.position;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;

                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        nearestTarget = hitCollider.transform;
                    }
                }
            }
        }

        if (nearestTarget != null && unitStats.GetUnitType() != UnitType.Worker)
        {
            SetAsTarget(nearestTarget);     // only sets target
        }
    }

    public void Attack()
    {
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);
        if (unitStats.GetUnitType() != UnitType.Catapult)
        {
            if (targetDistance <= attackRange && gameObject.GetComponent<NavMeshAgent>().velocity == Vector3.zero)
            {
                gameObject.GetComponentInChildren<UnitAnimation>().Attack();
                LookAtTarget();
            }
            else if (targetDistance > attackRange && unitBehaviour == UnitBehaviour.Offensive)
            {
                Vector3 movePos = target.transform.position;
                movePos = Vector3.MoveTowards(movePos, transform.position, 0f);
                unitAgent.stoppingDistance = attackRange;
                unitAgent.SetDestination(movePos);
            }
        }
        else if (unitStats.GetUnitType() == UnitType.Catapult )
        {
            if (targetDistance <= attackRange && targetDistance >= unitStats.GetRangeMinAttackRange() && gameObject.GetComponent<NavMeshAgent>().velocity == Vector3.zero)
            {
                gameObject.GetComponent<UnitAnimation>().Attack();
                LookAtTarget();
            }
            else if (targetDistance > attackRange && unitBehaviour == UnitBehaviour.Offensive)
            {
                Vector3 moveDir = transform.position - target.position;
                Vector3 movePos = transform.position - (moveDir.normalized * 5f);
                unitAgent.SetDestination(movePos);
            }
            else if (targetDistance < unitStats.GetRangeMinAttackRange() && unitBehaviour == UnitBehaviour.Offensive)
            {
                Vector3 moveDir = transform.position - target.position;
                Vector3 movePos = transform.position + (moveDir.normalized * 5f);
                unitAgent.SetDestination(movePos);
            }
        }
    }

    private void EscapeBehaviour(Transform enemy)
    {
        if (unitBehaviour == UnitBehaviour.Escape)
        {
            Vector3 moveDir = transform.position - enemy.position;
            Vector3 movePos = transform.position + (moveDir.normalized*enemy.GetComponent<Unit>().GetUnitVisibility());
            unitAgent.SetDestination(movePos);
        }
    }

    public void AttackDamage()
    {
        if (target != null)
        {
            if (target.gameObject.GetComponent<Unit>() != null)
            {
                if (target.GetComponent<PlayerController>() == null)
                {
                    target.gameObject.GetComponent<UnitOffensiveBehaviour>().EscapeBehaviour(transform);
                }
                target.gameObject.GetComponent<Unit>().TakeDamage(GetUnitAttackDamage() * FriendlyBuffActive() * (1 / target.GetComponent<Unit>().GetArmor() * 8 * GetEffectivnessAgainstArmor()));
            }
            else if (target.gameObject.GetComponent<Building>() != null)
            {
                target.gameObject.GetComponent<Building>().TakeDamage(GetUnitAttackDamage() * FriendlyBuffActive() * GetEffectivnessAgainstArmor());
            }
        }
        
        
    }

    public void SetAsTarget(Transform selectedTarget)
    {
        target = selectedTarget;
        if(target != null)
        {
            if (target.GetComponent<Building>() != null &&
                unitStats.GetUnitFaction() != target.GetComponent<Building>().GetBuildingFaction() &&
                target.GetComponent<Building>().GetStoppingDistance() > GetUnitAttackRange())
            {
                attackRange = target.GetComponent<Building>().GetStoppingDistance();
            }
            else
            {
                attackRange = GetUnitAttackRange();
            }
        }
    }

    private void LookAtTarget()
    {
        Vector3 direction = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        
    }
    
    public void SetUnitBehavior(int newBehaviour)
    {
        if (unitStats.GetUnitType() != UnitType.Worker)
        {
            switch (newBehaviour)
            {
                case 0:
                    unitBehaviour = UnitBehaviour.Defensive;
                    break;
                case 1:
                    unitBehaviour = UnitBehaviour.Offensive;
                    break;
                case 2:
                    unitBehaviour = UnitBehaviour.Passive;
                    break;
                case 3:
                    unitBehaviour = UnitBehaviour.Escape;
                    break;
            }
        }
    }

    public int GetUnitBehaviour()
    {
        int buttonIndex = 0;
        switch (unitBehaviour)
        {
            case UnitBehaviour.Defensive:
                buttonIndex = 0;
                break;
            case UnitBehaviour.Offensive:
                buttonIndex = 1;
                break;
            case UnitBehaviour.Passive:
                buttonIndex = 2;
                break;
            case UnitBehaviour.Escape:
                buttonIndex = 3;
                break;
        }
        return buttonIndex;
    }

    public float GetUnitAttackRange()
    {
        if (unitStats.GetMeleeDamage() > unitStats.GetRangeDamage())
        {
            return unitStats.GetMeleeAttackRange();
        }
        else
        {
            return unitStats.GetRangeAttackRange();
        }
    }

    private float GetUnitAttackDamage()
    {
        float targetDistance = Vector3.Distance(transform.position, target.transform.position);

        if (target.GetComponent<Building>() != null)
        {
            targetDistance -= target.GetComponent<Building>().GetStoppingDistance();
        }
        
        if (targetDistance <= unitStats.GetMeleeAttackRange())
        {
            return unitStats.GetMeleeDamage();
        }
        else if (targetDistance <= unitStats.GetRangeAttackRange() &&
            targetDistance > unitStats.GetMeleeAttackRange() || unitStats.GetUnitType() == UnitType.Catapult)
        {
            return unitStats.GetRangeDamage();
        }
        else
        {
            return 0;
        }
    }

    private float GetEffectivnessAgainstArmor()
    {
        float damageBuff = 2f;
        float damageNerf = 0.5f;
        
        if(target.GetComponent<Unit>() != null && unitStats.GetUnitType() != UnitType.Catapult)
        {
            if (unitStats.GetUnitType() == UnitType.Swordsman)
            {
                switch (target.GetComponent<Unit>().GetUnitType())
                {
                    case UnitType.Archer:
                        return damageBuff;
                    case UnitType.Catapult:
                        return damageBuff;
                    case UnitType.Mage:
                        return damageNerf;
                }
            }
            else if (unitStats.GetUnitType() == UnitType.Archer)
            {
                switch (target.GetComponent<Unit>().GetUnitType())
                {
                    case UnitType.Mage:
                        return damageBuff;
                    case UnitType.Catapult:
                        return damageBuff;
                    case UnitType.Swordsman:
                        return damageNerf;
                }
            }
            else if (unitStats.GetUnitType() == UnitType.Mage)
            {
                switch (target.GetComponent<Unit>().GetUnitType())
                {
                    case UnitType.Swordsman:
                        return damageBuff;
                    case UnitType.Catapult:
                        return damageBuff;
                    case UnitType.Archer:
                        return damageNerf;
                }
            }
        }
        else if(unitStats.GetUnitType() == UnitType.Catapult)
        {
            if (target.GetComponent<Unit>() != null &&
                target.GetComponent<PlayerController>() == null &&
                target.GetComponent<NavMeshAgent>().velocity != Vector3.zero)
            {
                return 0.01f;
            }
            else if (target.GetComponent<Building>() != null)
            {
                return damageBuff;
            }
        }
        return 1f;
        
    }

    public float FriendlyBuffActive()
    {
        if (unitStats.GetUnitFaction() == Faction.Player_1)
        {
            if (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) < 30f && unitStats.GetUnitType() != UnitType.Archmage)
            {
                return 1.5f;
            }
        }
        else
        {
            if(CPUArchMage.Instance != null)
            {
                if (Vector3.Distance(CPUArchMage.Instance.transform.position, transform.position) < 30f && unitStats.GetUnitType() != UnitType.Archmage)
                {
                    return 1.5f;
                }
            }
        }
        return 1f;
    }
}
