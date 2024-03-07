using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAnimation : MonoBehaviour
{
    Animator unitAnimator;
    NavMeshAgent unitAgent;

    [SerializeField] GameObject[] gathererTools;
    [SerializeField] GameObject attackEffect;
    [SerializeField] GameObject arrow;

    // Start is called before the first frame update
    void Start()
    {
        unitAnimator = gameObject.GetComponent<Animator>();
        unitAgent = gameObject.GetComponentInParent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponentInParent<PlayerController>() == null)
        {
            Walking();
        }
    }

    public void Walking()
    {
        if (unitAgent.velocity != Vector3.zero)
        {
            unitAnimator.SetBool("walking",true);
        }
        else
        {
            unitAnimator.SetBool("walking", false);
        }
    }

    public void GatherResources(bool isHarvesting, ResourceType resource)
    {
        if (isHarvesting)
        {
            unitAnimator.SetTrigger("harvesting");
            if(resource == ResourceType.Wood)
            {
                unitAnimator.SetFloat("Blend", 0);
                ActivateTool(0);
            }
            else if(resource == ResourceType.Food)
            {
                unitAnimator.SetFloat("Blend", 1);
                ActivateTool(1);
            }
            else
            {
                unitAnimator.SetFloat("Blend", 2);
                ActivateTool(2);
            }
        }
        else
        {
            unitAnimator.SetTrigger("deliverResources");
            DeactivateAllTools();
        }
    }

    private void ActivateTool(int toolIndex)
    {
        if (!gathererTools[toolIndex].activeInHierarchy)
        {
            for (int i = 0; i < gathererTools.Length; i++)
            {
                if (i == toolIndex)
                {
                    gathererTools[i].SetActive(true);
                }
                else
                {
                    if (gathererTools[i].activeInHierarchy)
                    {
                        gathererTools[i].SetActive(false);
                    }
                }
            }
        }
    }

    public void DeactivateAllTools()
    {
        foreach (GameObject tool in gathererTools)
        {
            if (tool.activeInHierarchy)
            {
                tool.SetActive(false);
            }
        }
    }

    public void Build()
    {
        unitAnimator.SetTrigger("build");
        ActivateTool(3);
    }

    public void Attack()
    {
        unitAnimator.SetTrigger("attack");
    }

    public void AttackEffect()
    {
        attackEffect.GetComponent<ParticleSystem>().Play();
        if (gameObject.GetComponentInParent<Unit>().GetUnitType() == UnitType.Archer)
        {
            arrow.SetActive(false);
        }
    }

    public void SpawnArrow()
    {
        arrow.SetActive(true);
    }

    public void NoTarget()
    {
        unitAnimator.SetTrigger("noTarget");
    }

    public void DealDamage()
    {
        UnitOffensiveBehaviour unitOffensive = gameObject.GetComponentInParent<UnitOffensiveBehaviour>();
        unitOffensive.AttackDamage();
        if(unitOffensive.GetAttackClip() != null)
        {
            gameObject.GetComponentInParent<AudioSource>().PlayOneShot(unitOffensive.GetAttackClip());
        }
    }

    public void HitRock()
    {
        AudioSource audioSource = gameObject.GetComponentInParent<AudioSource>();

        if (gameObject.GetComponentInParent<Unit>().GetUnitFaction() == Faction.CPU)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(gameObject.GetComponentInParent<CPUGatherer>().GetHitRockClip());
        }
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(gameObject.GetComponentInParent<Gatherer>().GetHitRockClip());
        }
    }

    public void HitTree()
    {
        AudioSource audioSource = gameObject.GetComponentInParent<AudioSource>();

        if (gameObject.GetComponentInParent<Unit>().GetUnitFaction() == Faction.CPU)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(gameObject.GetComponentInParent<CPUGatherer>().GetHitTreeClip());
        }
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(gameObject.GetComponentInParent<Gatherer>().GetHitTreeClip());
        }
    }

    public void Footstep()
    {
        if(gameObject.GetComponentInParent<Unit>().GetUnitFaction() == Faction.CPU)
        {
            gameObject.GetComponentInParent<AudioSource>().PlayOneShot(gameObject.GetComponentInParent<CPUUnitMovement>().GetRandomMovementClip());
        }
        else
        {
            if(gameObject.GetComponentInParent<PlayerController>() != null)
            {
                gameObject.GetComponentInParent<AudioSource>().PlayOneShot(gameObject.GetComponentInParent<PlayerController>().GetRandomFootstepClip());
            }
            else
            {
                gameObject.GetComponentInParent<AudioSource>().PlayOneShot(gameObject.GetComponentInParent<UnitMovement>().GetRandomMovementClip());
            }
        }
    }

    public void CatapultMove()
    {
        if(gameObject.GetComponent<Unit>().GetUnitType() == UnitType.Catapult)
        {
            if(gameObject.GetComponent<Unit>().GetUnitFaction() == Faction.CPU)
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(gameObject.GetComponent<CPUUnitMovement>().GetFirstMovementClip());
            }
            else
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(gameObject.GetComponent<UnitMovement>().GetFirstMovementClip());
            }
        }
    }

    public void Death()
    {
        unitAnimator.SetBool("death", true);
        unitAgent.isStopped = true;
    }

    public void IsDead()
    {
        gameObject.GetComponentInParent<Unit>().DestroyUnit();
    }

}
