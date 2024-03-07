using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class CPUManager : MonoBehaviour
{
    private static CPUManager _instance;
    public static CPUManager Instance { get { return _instance; } }

    private GameManager gameManager;
    private DataManager dataManager;
    private CPUResourceManager cpuResourceManager;
    private CPUUnitHandler cpuUnitHandler;
    private CPUUnitSpawner cpuUnitSpawner;
    private CPUHighPrioTargetCalculator cpuPrioTargetCalculator;
    private CPUStage currentStage = CPUStage.Stage_1;

    private float checkForIdleVillagerInterval = 20f;
    private float countdownForIdleVillagerInterval;

    private float checkForTargetsInterval = 30f;
    private float countdownForTargetsInterval;

    [SerializeField] List<GameObject> cpuBuildings;
    [SerializeField] Transform archMageSpawner = null;
    [SerializeField] List<ScriptableUnit> listOfUnitData = new List<ScriptableUnit>();
    [SerializeField] List<ScriptableBuilding> listOfBuildingData = new List<ScriptableBuilding>();

    private bool[] hasCurrentUpgradeTier = { false, false, false };

    private bool rebuildTaskTownCenterHasStarted = false;

    private bool isArchMageAlive = true;
    private float baseArchMageRespawnTime = 10f;
    private float archMageRespawnCountdown;

    private float currentResourceScannerRange = 800;

    private Dictionary<CPUStage, ResourceWorkerStageDefinition> workerAmountPerStageForResource = new Dictionary<CPUStage, ResourceWorkerStageDefinition>();

    [SerializeField] Transform highPrioTarget = null;

    // Private Constructor to prevent creating instance
    private CPUManager() { }

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

    private void Start()
    {
        gameManager = GameManager.Instance;
        dataManager = DataManager.Instance;
        cpuResourceManager = CPUResourceManager.Instance;
        cpuUnitHandler = CPUUnitHandler.Instance;
        cpuUnitSpawner = CPUUnitSpawner.Instance;
        cpuPrioTargetCalculator = CPUHighPrioTargetCalculator.Instance;

        countdownForIdleVillagerInterval = checkForIdleVillagerInterval;
        countdownForTargetsInterval = checkForTargetsInterval;
        archMageRespawnCountdown = baseArchMageRespawnTime;

        workerAmountPerStageForResource.Add(CPUStage.Stage_1, new ResourceWorkerStageDefinition(workerForFood: 5, workerForGold: 0, workerForIron: 0, workerForStone: 0, workerForWood: 5));
        workerAmountPerStageForResource.Add(CPUStage.Stage_2, new ResourceWorkerStageDefinition(workerForFood: 5, workerForGold: 5, workerForIron: 5, workerForStone: 0, workerForWood: 5));
        workerAmountPerStageForResource.Add(CPUStage.Stage_3, new ResourceWorkerStageDefinition(workerForFood: 10, workerForGold: 5, workerForIron: 5, workerForStone: 5, workerForWood: 5));
        workerAmountPerStageForResource.Add(CPUStage.Stage_4, new ResourceWorkerStageDefinition(workerForFood: 10, workerForGold: 5, workerForIron: 5, workerForStone: 5, workerForWood: 5));
        workerAmountPerStageForResource.Add(CPUStage.Stage_5, new ResourceWorkerStageDefinition(workerForFood: 10, workerForGold: 5, workerForIron: 5, workerForStone: 5, workerForWood: 5));
        workerAmountPerStageForResource.Add(CPUStage.Stage_6, new ResourceWorkerStageDefinition(workerForFood: 10, workerForGold: 5, workerForIron: 5, workerForStone: 5, workerForWood: 5));
        workerAmountPerStageForResource.Add(CPUStage.Stage_7, new ResourceWorkerStageDefinition(workerForFood: 10, workerForGold: 6, workerForIron: 5, workerForStone: 5, workerForWood: 5));
    }

    private void Update()
    {
        StageHandler();
        if (Input.GetKeyDown(KeyCode.M))
        {
            cpuUnitHandler.CheckIfVillagerIsIdle();
        }
    }

    private void StageHandler()
    {
        switch (currentStage)
        {
            case CPUStage.Stage_1:
                SetHighPrioTarget();
                TownCenterRebuildTask();
                ArchMageRespawnTask(isArchMageAlive);
                
                if (cpuUnitHandler.GetCPUVillagerUnitList().Count <= 5)
                {
                    cpuUnitHandler.SetSpecificUnitCommand(CPUCommandUnit.Collect_Food); // send first 5 workers to food
                }
                else if (cpuUnitHandler.GetCPUVillagerUnitList().Count > 5 && cpuUnitHandler.GetCPUVillagerUnitList().Count <= 10 && cpuUnitHandler.HasIdleVillager())
                {
                    cpuUnitHandler.SetSpecificUnitCommand(CPUCommandUnit.Collect_Wood); // send next workers to wood
                }
                if (cpuResourceManager.GetResourceFood() >= GetCostWithDifficultyMultiplier(listOfUnitData[0].FoodCost) && 
                    cpuUnitHandler.GetCPUVillagerUnitList().Count <= 10 && HasEnoughtPopulation())
                {
                    cpuUnitSpawner.ChooseUnitToSpawn(cpuBuildings[0].GetComponent<Building>(), 0);
                    cpuResourceManager.SetCPUResources(ResourceType.Food, -GetCostWithDifficultyMultiplier(listOfUnitData[0].FoodCost));
                } 
                else if(cpuUnitHandler.GetCPUVillagerUnitList().Count >= 11)
                {
                    BarracksTask();
                }
                if (cpuBuildings[1].transform.Find("BuildingPrefab").gameObject.activeInHierarchy)
                {
                  currentStage = CPUStage.Stage_2;
                }
                break;

            case CPUStage.Stage_2:
                SetHighPrioTarget();
                ArchMageRespawnTask(isArchMageAlive);
                TownCenterRebuildTask();
                if (cpuUnitHandler.GetCPUVillagerUnitList().Count < 20)
                {
                    if (cpuResourceManager.GetResourceFood() >= GetCostWithDifficultyMultiplier(listOfUnitData[0].FoodCost) && HasEnoughtPopulation()) 
                    {
                        VillagerCheckInterval();
                    }
                }
                else
                {
                   currentStage = CPUStage.Stage_3;
                }
                break;

            case CPUStage.Stage_3:
                ArchMageRespawnTask(isArchMageAlive);
                TownCenterRebuildTask();
                if (cpuUnitHandler.GetCPUVillagerUnitList().Count < 30)
                {
                    if (cpuResourceManager.GetResourceFood() >= GetCostWithDifficultyMultiplier(listOfUnitData[0].FoodCost) && HasEnoughtPopulation())
                    {
                        VillagerCheckInterval();
                    }
                }
                else
                {
                    RecruitSwordsman();
                    AttackTask(100, CPUStage.Stage_4);
                }
                break;

            case CPUStage.Stage_4:
                TownCenterRebuildTask();
                SetHighPrioTarget();
                ArchMageRespawnTask(isArchMageAlive);
                
                ForgeBuildingTask();
                if (cpuBuildings[2].transform.Find("BuildingPrefab").gameObject.activeInHierarchy)
                {
                    currentStage = CPUStage.Stage_5;
                }
                break;

            case CPUStage.Stage_5:
                TownCenterRebuildTask();
                SetHighPrioTarget();
                ArchMageRespawnTask(isArchMageAlive);
                UpgradeTask(0);
                if (!hasCurrentUpgradeTier[0])
                {
                    gameObject.GetComponent<UpgradeValues>().UpgradeAllTechnologies();
                    hasCurrentUpgradeTier[0] = true;
                }
                BarracksTask();
                AttackTask(250, CPUStage.Stage_6, true);
                VillagerCheckInterval();
                break;

            case CPUStage.Stage_6:
                TownCenterRebuildTask();
                SetHighPrioTarget();
                ArchMageRespawnTask(isArchMageAlive);
                UpgradeTask(1);
                MageTowerTask();
                BarracksTask();
                AttackTask(500, CPUStage.Stage_7, true);
                VillagerCheckInterval();
                break;

            case CPUStage.Stage_7:
                TownCenterRebuildTask();
                SetHighPrioTarget();
                ArchMageRespawnTask(isArchMageAlive);
                UpgradeTask(2);
                if (Random.Range(0, 11) % 2 == 0)
                {
                    MageTowerTask();
                }
                else
                {
                    BarracksTask();
                }
                SiegeWorkshopTask();
                AttackTask(600, CPUStage.Stage_7, true);
                VillagerCheckInterval();
                break;
            default:
                break;
        }
    }

    private void RecruitSwordsman()
    {
        if (cpuResourceManager.GetResourceFood() >= GetCostWithDifficultyMultiplier(listOfUnitData[1].FoodCost) && 
            cpuResourceManager.GetResourceIron() >= GetCostWithDifficultyMultiplier(listOfUnitData[1].IronCost) && 
            cpuResourceManager.GetResourceGold() >= GetCostWithDifficultyMultiplier(listOfUnitData[1].GoldCost))
        {
            cpuUnitSpawner.ChooseUnitToSpawn(cpuBuildings[1].GetComponent<Building>(), 0);
            cpuUnitHandler.CalcMilitaryStrength();
            cpuResourceManager.SetCPUResources(ResourceType.Food, -GetCostWithDifficultyMultiplier(listOfUnitData[1].FoodCost));
            cpuResourceManager.SetCPUResources(ResourceType.Iron, -GetCostWithDifficultyMultiplier(listOfUnitData[1].IronCost));
            cpuResourceManager.SetCPUResources(ResourceType.Gold, -GetCostWithDifficultyMultiplier(listOfUnitData[1].GoldCost));
        }
    }

    private void RecruitArcher()
    {
        if (cpuResourceManager.GetResourceFood() >= GetCostWithDifficultyMultiplier(listOfUnitData[2].FoodCost) && 
            cpuResourceManager.GetResourceGold() >= GetCostWithDifficultyMultiplier(listOfUnitData[2].GoldCost) && 
            cpuResourceManager.GetResourceWood() >= GetCostWithDifficultyMultiplier(listOfUnitData[2].WoodCost))
        {
            cpuUnitSpawner.ChooseUnitToSpawn(cpuBuildings[1].GetComponent<Building>(), 1);
            cpuUnitHandler.CalcMilitaryStrength();
            cpuResourceManager.SetCPUResources(ResourceType.Food, -GetCostWithDifficultyMultiplier(listOfUnitData[2].FoodCost));
            cpuResourceManager.SetCPUResources(ResourceType.Gold, -GetCostWithDifficultyMultiplier(listOfUnitData[2].GoldCost));
            cpuResourceManager.SetCPUResources(ResourceType.Wood, -GetCostWithDifficultyMultiplier(listOfUnitData[2].WoodCost));
        }
    }

    private void RecruitMage()
    {
        if (cpuResourceManager.GetResourceFood() >= GetCostWithDifficultyMultiplier(listOfUnitData[2].FoodCost) && 
            cpuResourceManager.GetResourceGold() >= GetCostWithDifficultyMultiplier(listOfUnitData[2].GoldCost))
        {
            cpuUnitSpawner.ChooseUnitToSpawn(cpuBuildings[3].GetComponent<Building>(), 0);
            cpuUnitHandler.CalcMilitaryStrength();
            cpuResourceManager.SetCPUResources(ResourceType.Food, -GetCostWithDifficultyMultiplier(listOfUnitData[2].FoodCost));
            cpuResourceManager.SetCPUResources(ResourceType.Gold, -GetCostWithDifficultyMultiplier(listOfUnitData[2].GoldCost));
        }
    }

    private void RecruitCatapult()
    {
        if (cpuResourceManager.GetResourceIron() >= GetCostWithDifficultyMultiplier(listOfUnitData[2].IronCost) && 
            cpuResourceManager.GetResourceWood() >= GetCostWithDifficultyMultiplier(listOfUnitData[2].WoodCost))
        {
            cpuUnitSpawner.ChooseUnitToSpawn(cpuBuildings[4].GetComponent<Building>(), 0);
            cpuUnitHandler.CalcMilitaryStrength();
            cpuResourceManager.SetCPUResources(ResourceType.Iron, -GetCostWithDifficultyMultiplier(listOfUnitData[2].IronCost));
            cpuResourceManager.SetCPUResources(ResourceType.Wood, -GetCostWithDifficultyMultiplier(listOfUnitData[2].WoodCost));
        }
    }

    private void TownCenterRebuildTask()
    {
        if (!cpuBuildings[0].transform.Find("BuildingPrefab").gameObject.activeInHierarchy)
        {
            
            if (cpuResourceManager.GetResourceWood() >= listOfBuildingData[0].WoodCost)
            {
                rebuildTaskTownCenterHasStarted = true;
                cpuUnitHandler.SetSpecificUnitCommand(CPUCommandUnit.Build_TownCenter, cpuUnitHandler.GetCPUVillagerUnitList()[cpuUnitHandler.GetCPUVillagerUnitList().Count - 1]);
            }
            else
            {
                if (!rebuildTaskTownCenterHasStarted)
                {
                    gameManager.CPUSurrender();
                }
            }
        }
        else
        {
            rebuildTaskTownCenterHasStarted = false;
        }
    }

    private void BarracksTask()
    {
        if (cpuBuildings[1].transform.Find("BuildingPrefab").gameObject.activeInHierarchy && HasEnoughtPopulation())
        {
            if (Random.Range(0, 11) %2 == 0)
            {
                RecruitArcher();
            }
            else
            {
                RecruitSwordsman();
            }
        }
        else
        {
            if (cpuResourceManager.GetResourceWood() >= listOfBuildingData[1].WoodCost && 
                cpuUnitHandler.GetCPUVillagerUnitList().Count > 0)
            {
                cpuUnitHandler.SetSpecificUnitCommand(CPUCommandUnit.Build_Barracks, cpuUnitHandler.GetCPUVillagerUnitList()[cpuUnitHandler.GetCPUVillagerUnitList().Count - 1]);
            }
        }
    }
    private void ForgeBuildingTask()
    {
        if (!cpuBuildings[2].transform.Find("BuildingPrefab").gameObject.activeInHierarchy)
        {
            if (cpuResourceManager.GetResourceWood() >= listOfBuildingData[2].WoodCost &&
                cpuResourceManager.GetResourceStone() >= listOfBuildingData[2].StoneCost &&
                cpuUnitHandler.GetCPUVillagerUnitList().Count > 0)
            {
                cpuUnitHandler.SetSpecificUnitCommand(CPUCommandUnit.Build_Forge, cpuUnitHandler.GetCPUVillagerUnitList()[0]);
            }
        }
    }

    private void MageTowerTask()
    {
        if (cpuBuildings[3].transform.Find("BuildingPrefab").gameObject.activeInHierarchy && HasEnoughtPopulation())
        {
            RecruitMage();
        }
        else
        {
            if (cpuResourceManager.GetResourceWood() >= listOfBuildingData[3].WoodCost &&
                cpuResourceManager.GetResourceStone() >= listOfBuildingData[3].StoneCost &&
                cpuUnitHandler.GetCPUVillagerUnitList().Count > 0)
            {
                cpuUnitHandler.SetSpecificUnitCommand(CPUCommandUnit.Build_Magetower, cpuUnitHandler.GetCPUVillagerUnitList()[cpuUnitHandler.GetCPUVillagerUnitList().Count - 1]);
            }
        }
    }

    private void SiegeWorkshopTask()
    {
        if (cpuBuildings[4].transform.Find("BuildingPrefab").gameObject.activeInHierarchy && HasEnoughtPopulation())
        {
            if (Random.Range(0, 2) > 0)
            {
                RecruitCatapult();
            }
        }
        else
        {
            if (cpuResourceManager.GetResourceWood() >= listOfBuildingData[4].WoodCost &&
                cpuResourceManager.GetResourceStone() >= listOfBuildingData[4].StoneCost &&
                cpuUnitHandler.GetCPUVillagerUnitList().Count > 0)
            {
                cpuUnitHandler.SetSpecificUnitCommand(CPUCommandUnit.Build_SiegeWorkshop, cpuUnitHandler.GetCPUVillagerUnitList()[cpuUnitHandler.GetCPUVillagerUnitList().Count - 1]);
            }
        }
    }

    private int GetIndexOfToBuildWarehouses()
    {
        int countNeededStorageBuildings = 0;
        if (currentResourceScannerRange >= 800)
        {
            countNeededStorageBuildings = 8;
        }
        else if (currentResourceScannerRange >= 600)
        {
            countNeededStorageBuildings = 6;
        }
        else if (currentResourceScannerRange >= 400)
        {
            countNeededStorageBuildings = 4;
        }
        else if (currentResourceScannerRange >= 200)
        {
            countNeededStorageBuildings = 2;
        }
        return countNeededStorageBuildings;
    }

    private void StorageBuildingTask()
    {
        int buildingsInQueueCount = 0;
        int costStorageBuilding = listOfBuildingData[5].WoodCost;
        int buildingsInBaseOffset = 5;
        for (int i = 0; i < GetIndexOfToBuildWarehouses(); i++)
        {
            if (!cpuBuildings[i + buildingsInBaseOffset].transform.Find("BuildingPrefab").gameObject.activeInHierarchy)
            {
                if (cpuUnitHandler.GetCPUVillagerUnitList().Count > i)
                {
                    GameObject villager = cpuUnitHandler.GetCPUVillagerUnitList()[cpuUnitHandler.GetCPUVillagerUnitList().Count - 1 - i];
                    if (villager.GetComponent<CPUGatherer>().GetCPUGathererStage() != GathererState.Build)
                    {
                        if (cpuResourceManager.GetResourceWood() >= costStorageBuilding + (buildingsInQueueCount * costStorageBuilding) &&
                            cpuUnitHandler.GetCPUVillagerUnitList().Count > 0)
                        {
                            buildingsInQueueCount++;
                            cpuUnitHandler.SetSpecificUnitCommand(CPUCommandUnit.Build_Depot, villager, i);
                        }
                    }
                }
            }
        }
    }

    private void AttackTask(int minMilitaryPower, CPUStage newStage, bool sendArchmage = false)
    {
        if (cpuUnitHandler.GetCPUMilitaryStrength() >= minMilitaryPower) // check military power
        {
            SetHighPrioTarget();
            cpuUnitHandler.SendCommandToMilitaryUnits(highPrioTarget);
            if (sendArchmage)
            {
                ArchmageAttackTask();
            }
            currentStage = newStage;
        }
    }

    private void ArchmageAttackTask()
    {
        if (isArchMageAlive && dataManager.GetGameModeData() != 1)
        {
            cpuUnitHandler.SendCommandToArchMage(highPrioTarget);
        }
    }

    private bool HasEnoughtPopulation()
    {
        if (cpuUnitHandler.GetCPUVillagerUnitList().Count + cpuUnitHandler.GetCPUMilitaryUnitList().Count >= GameManager.Instance.GetMaxPopulation())
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void UpgradeTask(int index)
    {
        if (!hasCurrentUpgradeTier[index])
        {
            gameObject.GetComponent<UpgradeValues>().UpgradeAllTechnologies();
            foreach (var villagerUnit in cpuUnitHandler.GetCPUVillagerUnitList())
            {
                villagerUnit.GetComponent<Unit>().UpdateStatsAfterUpgradeIsFinished();
            }
            foreach (var militaryUnit in cpuUnitHandler.GetCPUMilitaryUnitList())
            {
                militaryUnit.GetComponent<Unit>().UpdateStatsAfterUpgradeIsFinished();
            }
            hasCurrentUpgradeTier[index] = true;
        }
    }

    public void SetArchMageStateForManager(bool state)
    {
        isArchMageAlive = state;
    }
    public bool GetArchMageStateInManager()
    {
        return isArchMageAlive;
    }

    public Transform GetArchMageSpawner()
    {
        return archMageSpawner;
    }

    private void ArchMageRespawnTask(bool archMageStatus)
    {
        if (!archMageStatus)
        {
            archMageRespawnCountdown -= Time.deltaTime;
            if (archMageRespawnCountdown <= 0)
            {
                cpuUnitHandler.RespawnArchMage(archMageSpawner);
                archMageRespawnCountdown = baseArchMageRespawnTime;
                isArchMageAlive = true;
            }
        }
    }

    public bool AllEnemyBuildingsDestroyed()
    {
        if (cpuBuildings != null)
        {
            for (int i = 0; i < cpuBuildings.Count; i++)
            {
                if (cpuBuildings[i].transform.Find("BuildingPrefab") != null)
                {
                    if (cpuBuildings[i].transform.Find("BuildingPrefab").gameObject.activeInHierarchy)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        return false;
    }

    private void SetHighPrioTarget()
    {
        countdownForTargetsInterval -= Time.deltaTime;
        if (countdownForTargetsInterval <= 0)
        {
            if (cpuPrioTargetCalculator.CalcCPUHighPrioTarget(dataManager.GetGameModeData()).Count > 0)
            {
                highPrioTarget = cpuPrioTargetCalculator.CalcCPUHighPrioTarget(dataManager.GetGameModeData())[0];
            }
            countdownForTargetsInterval = checkForTargetsInterval;
        }
    }

    private int GetCostWithDifficultyMultiplier(int baseValue)
    {
        float calculatedValue = 1;
        if (dataManager.GetDifficultyData() == 2)
        {
            calculatedValue = 0.5f;
        }
        else if (dataManager.GetDifficultyData() == 0)
        {
            calculatedValue = 2f;
        }
        return (int)Mathf.Round(baseValue * calculatedValue);
    }

    private void CheckWorkerAmountTask(ResourceType resourceType, int amountOfNeededWorkers)
    {
        if (!cpuUnitHandler.HasEnoughVillagerOnResource(resourceType, amountOfNeededWorkers))
        {
            if (cpuResourceManager.GetResourceFood() >= GetCostWithDifficultyMultiplier(listOfUnitData[0].FoodCost) && HasEnoughtPopulation())
            {
                GameObject newVillager = cpuUnitSpawner.ChooseUnitToSpawn(cpuBuildings[0].GetComponent<Building>(), 0);
                cpuResourceManager.SetCPUResources(ResourceType.Food, -GetCostWithDifficultyMultiplier(listOfUnitData[0].FoodCost));
                newVillager.GetComponent<CPUGatherer>().OverrideJob(resourceType);
            }
        }
         cpuUnitHandler.CheckIfVillagerIsIdle(); // check for idle workers
    }

    private void VillagerCheckInterval()
    {
        countdownForIdleVillagerInterval -= Time.deltaTime;
        if (countdownForIdleVillagerInterval <= 0)
        {
            currentResourceScannerRange = cpuUnitHandler.GetCurrentResourceScanerRange();
            CheckWorkerAmountTask(ResourceType.Food, workerAmountPerStageForResource[currentStage].GetWorkersAmountForFood());
            CheckWorkerAmountTask(ResourceType.Gold, workerAmountPerStageForResource[currentStage].GetWorkersAmountForGold());
            CheckWorkerAmountTask(ResourceType.Iron, workerAmountPerStageForResource[currentStage].GetWorkersAmountForIron());
            CheckWorkerAmountTask(ResourceType.Stone, workerAmountPerStageForResource[currentStage].GetWorkersAmountForStone());
            CheckWorkerAmountTask(ResourceType.Wood, workerAmountPerStageForResource[currentStage].GetWorkersAmountForWood());
            StorageBuildingTask();
            countdownForIdleVillagerInterval = checkForIdleVillagerInterval;
        }
    }

    public GameObject GetTownCenter() => cpuBuildings[0];
}
