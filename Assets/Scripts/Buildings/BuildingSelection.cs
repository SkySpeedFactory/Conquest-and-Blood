using System.Collections.Generic;
using UnityEngine;

public class BuildingSelection : MonoBehaviour
{
    private static BuildingSelection _instance;
    public static BuildingSelection Instance { get { return _instance; } }

    // Instances
    private ActionBarManager actionBarManager;
    private FactionObjectUI factionObjectUI;
    private PlayerManager playerManager;
    public GameObject SelectedBuilding { get; private set; } = null;

    [SerializeField] LayerMask buildingLayer;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask clickableLayer;

    //private RaycastHit hit;

    [SerializeField] ScriptableUnit workerData;
    [SerializeField] ScriptableUnit swordsmanData;
    [SerializeField] ScriptableUnit archerData;
    [SerializeField] ScriptableUnit mageData;
    [SerializeField] ScriptableUnit catapultData;

    // Private Constructor to prevent creating instance
    private BuildingSelection() { }

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
        actionBarManager = ActionBarManager.Instance;
        factionObjectUI = FactionObjectUI.Instance;
        playerManager = PlayerManager.Instance;
    }

    public bool CheckIfFinished(RaycastHit hitBuilding)
    {
        if (hitBuilding.transform.gameObject.GetComponent<Building>().CheckIfBuildingIsFinished())
        {
            return true;
        }
        return false;
    }

    public void DeselectBuilding()
    {
        if (SelectedBuilding != null)
        {
            SelectedBuilding.GetComponent<Building>().DefaultColor();
            actionBarManager.DeactiveAllButtonsGO();
            factionObjectUI.ResetFactionObjectUI();
            SelectedBuilding = null;
        }
    }

    public void SelectBuilding()
    {
        SelectedBuilding = SelectionSystem.Instance.GetRayCastHit().transform.gameObject;
        SelectedBuilding.GetComponent<Building>().SelectedColor();
        actionBarManager.ActivateButton();
        factionObjectUI.UpdateFactionObjectUI();
    }

    public void Spawn(int index)
    {
        List<int> unitCost = GetUnitCost(index);
        bool hasEnoughResources = CheckIfEnoughResources(index);
        if (hasEnoughResources)
        {
            SubractUnitCostFromResources(unitCost);
            SelectedBuilding.GetComponent<Building>().Spawn(index);
        }
    }

    private void SubractUnitCostFromResources(List<int> unitCost)
    {
        playerManager.SetPlayerResources(ResourceType.Food, -unitCost[0]);
        playerManager.SetPlayerResources(ResourceType.Gold, -unitCost[1]);
        playerManager.SetPlayerResources(ResourceType.Iron, -unitCost[2]);
        playerManager.SetPlayerResources(ResourceType.Wood, -unitCost[3]);
    }

    public void SubractUpgradeCostFromResources(int index)
    {
        int upgradeIndex = index - SelectedBuilding.GetComponent<Building>().GetAvailableUnits().Count;
        UpgradeType upgradeType = SelectedBuilding.GetComponent<Building>().GetUpgrades()[upgradeIndex].TypeOfUpgrade;
        
        if (playerManager.GetUpgradeLevel(upgradeType) < playerManager.GetMaxUpgradeLevel())
        {
            List<int> upgradeCost = GetUpgradeCost(upgradeIndex);
            int upgradeLevel = playerManager.GetUpgradeLevel(upgradeType);
            playerManager.SetPlayerResources(ResourceType.Food, -upgradeCost[0]);
            playerManager.SetPlayerResources(ResourceType.Gold, -upgradeCost[1]);
            playerManager.SetPlayerResources(ResourceType.Iron, -upgradeCost[2]);
            playerManager.SetPlayerResources(ResourceType.Wood, -upgradeCost[3]);

            if (upgradeLevel + 1 >= playerManager.GetMaxUpgradeLevel())
            {
                actionBarManager.ButtonEnabled(actionBarManager.GetButtonList()[index], false);
            }
        }
    }

    private int CalculateUpgradeMultiplier(int upgradeLevel)
    {
        int multiplier = 0;

        switch (upgradeLevel)
        {
            case 0:
                multiplier = 1;
                break;
            case 1:
                multiplier = 2;
                break;
            case 2:
                multiplier = 4;
                break;
            case 3:
                multiplier = 8;
                break;
        }
        return multiplier;
    }

    public bool CheckIfEnoughResourcesUpgrade(int index)
    {
        List<int> upgradeCost = GetUpgradeCost(index);
        return upgradeCost[0] <= playerManager.GetPlayerResources(ResourceType.Food) 
            && upgradeCost[1] <= playerManager.GetPlayerResources(ResourceType.Gold) 
            && upgradeCost[2] <= playerManager.GetPlayerResources(ResourceType.Iron) 
            && upgradeCost[3] <= playerManager.GetPlayerResources(ResourceType.Wood);
    }

    public bool CheckIfEnoughResources(int unitIndex)
    {
        List<int> unitCost = GetUnitCost(unitIndex);
        if (unitIndex < SelectedBuilding.GetComponent<Building>().GetAvailableUnits().Count)
        {
            return unitCost[0] <= playerManager.GetPlayerResources(ResourceType.Food)
                && unitCost[1] <= playerManager.GetPlayerResources(ResourceType.Gold)
                && unitCost[2] <= playerManager.GetPlayerResources(ResourceType.Iron)
                && unitCost[3] <= playerManager.GetPlayerResources(ResourceType.Wood);
        }
        return false;
    }

    private List<int> GetUpgradeCost(int index)
    {
        List<ScriptableUpgrades> upgradesList = SelectedBuilding.GetComponent<Building>().GetUpgrades();
        // Order: Food, Gold, Iron, Wood
        List<int> upgradeCost = new List<int>() { 0, 0, 0, 0 };
        int upgradeLevel = playerManager.GetUpgradeLevel(upgradesList[index].TypeOfUpgrade);
        upgradeCost[0] = upgradesList[index].CostFood * CalculateUpgradeMultiplier(upgradeLevel);
        upgradeCost[1] = upgradesList[index].CostGold * CalculateUpgradeMultiplier(upgradeLevel);
        upgradeCost[2] = upgradesList[index].CostIron * CalculateUpgradeMultiplier(upgradeLevel);
        upgradeCost[3] = upgradesList[index].CostWood * CalculateUpgradeMultiplier(upgradeLevel);
        return upgradeCost;
    }


    private List<int> GetUnitCost(int unitIndex)
    {
        // Order: Food, Gold, Iron, Wood
        List<int> unitCost = new List<int>() { 0, 0, 0, 0 };
        switch (SelectedBuilding.GetComponent<Building>().GetAvailableUnits()[unitIndex].name)
        {
            case "Villager":
            case "Worker":
                unitCost[0] = workerData.FoodCost;
                break;
            case "Swordsman":
                unitCost[0] = swordsmanData.FoodCost;
                unitCost[1] = swordsmanData.GoldCost;
                unitCost[2] = swordsmanData.IronCost;
                break;
            case "Archer":
                unitCost[0] = archerData.FoodCost;
                unitCost[1] = archerData.GoldCost;
                unitCost[3] = archerData.WoodCost;
                break;
            case "Mage":
                unitCost[0] = mageData.FoodCost;
                unitCost[1] = mageData.GoldCost;
                break;
            case "Catapult":
                unitCost[2] = catapultData.IronCost;
                unitCost[3] = catapultData.WoodCost;
                break;
            default:
                break;
        }
        return unitCost;
    }

}
