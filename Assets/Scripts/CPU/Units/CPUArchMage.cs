using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CPUArchMage : MonoBehaviour
{
    private bool isAlive = true;

    private static CPUArchMage _instance;
    public static CPUArchMage Instance { get { return _instance; } }

    private Unit unitData;
    private CPUUnitMovement cpuUnitMovement;

    private Transform saveSpot = null;

    private bool isRetreating = false;

    // Private Constructor to prevent creating instance
    private CPUArchMage() { }

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
        unitData = this.GetComponent<Unit>();
        cpuUnitMovement = this.GetComponent<CPUUnitMovement>();
        saveSpot = CPUManager.Instance.GetArchMageSpawner();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            gameObject.GetComponent<NavMeshAgent>().velocity = Vector3.zero;
            CPUManager.Instance.SetArchMageStateForManager(false);
            GameManager.Instance.CheckKingslayerWinConditions(this.gameObject);
            Destroy(this.gameObject);
        }

        if (!isRetreating && unitData.GetCurrentHealth() <= unitData.GetMaxHelath() / 2)
        {
            isRetreating = true;
            
            if (gameObject.GetComponent<Unit>().GetCurrentHealth() > 0)
            {
                ReturnToBase(saveSpot);
            }
        }
    }

    public void ChangeAliveState(bool state) => isAlive = state;
    public void ReturnToBase(Transform target) => cpuUnitMovement.Move(target);
}
