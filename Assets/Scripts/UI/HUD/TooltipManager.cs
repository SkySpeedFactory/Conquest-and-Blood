using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    private static TooltipManager _instance;
    public static TooltipManager Instance { get { return _instance; } }

    [SerializeField] Image tooltipBox;
    [SerializeField] Text tooltipText;

    [SerializeField] LayerMask uiLayer;

    // Private Constructor to prevent creating instance
    private TooltipManager() { }

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
    
    void Start()
    {
        tooltipBox.gameObject.SetActive(false);
    }

    void Update()
    {
        tooltipBox.transform.position = Input.mousePosition;
    }

    public void HideTooltip()
    {
        tooltipBox.gameObject.SetActive(false);
        tooltipText.text = string.Empty;
    }
    public void SetAndShowTooltip(int buttonIndex)
    {
        
        if(BuildingSelection.Instance.SelectedBuilding != null)
        {
            if(buttonIndex < ActionBarManager.Instance.GetNumOfActiveButtons())
            {
                tooltipBox.gameObject.SetActive(true);
                ScriptableUnit unitData = BuildingSelection.Instance.SelectedBuilding.
                    GetComponent<Building>().GetAvailableUnits()[buttonIndex].GetComponent<Unit>().GetUnitData();
                List<int> unitcost = new List<int> { unitData.FoodCost, unitData.GoldCost, unitData.IronCost, 0, unitData.WoodCost };
                tooltipText.text = SetTooltipText(unitcost, unitData.UnitName);
            }
            else
            {
                tooltipBox.gameObject.SetActive(true);
                ScriptableUpgrades upgradeData = BuildingSelection.Instance.SelectedBuilding.
                    GetComponent<Building>().GetUpgrades()[buttonIndex - ActionBarManager.Instance.GetNumOfActiveButtons()];
                UpgradeType upgradeType = upgradeData.TypeOfUpgrade;
                List<int> upgradeCost = new List<int>() 
                { 
                    upgradeData.CostFood * (CalculateUpgradeMultiplier(PlayerManager.Instance.GetUpgradeLevel(upgradeType))), 
                    upgradeData.CostGold * (CalculateUpgradeMultiplier(PlayerManager.Instance.GetUpgradeLevel(upgradeType))), 
                    upgradeData.CostIron * (CalculateUpgradeMultiplier(PlayerManager.Instance.GetUpgradeLevel(upgradeType))), 
                    0, 
                    upgradeData.CostWood * (CalculateUpgradeMultiplier(PlayerManager.Instance.GetUpgradeLevel(upgradeType))) 
                };
                if (PlayerManager.Instance.CheckIfUpgradeMaxLevelReached(upgradeType))
                {
                    tooltipText.text = SetTooltipText(upgradeType, upgradeData.UpgradeName);
                    return;
                }
                tooltipText.text = SetTooltipText(upgradeCost, upgradeType, upgradeData.UpgradeName);
            }
        }
        else if(UnitSelections.Instance.GetSelectedUnitsList().Count > 0)
        {
            tooltipBox.gameObject.SetActive(true);
            if(UnitSelections.Instance.GetSelectedUnitsList()[0].GetComponent<Unit>().GetUnitType() == UnitType.Worker)
            {
                List<int> buildingCost = new List<int>();
                string buildingName = string.Empty;
                
                switch (buttonIndex)
                {
                    case 0:
                        buildingCost = PlaceFoundation.Instance.GetBuildingData("townCenterData");
                        buildingName = "Town Center";
                        break;
                    case 1:
                        buildingCost = PlaceFoundation.Instance.GetBuildingData("barracksData");
                        buildingName = "Barracks";
                        break;
                    case 2:
                        buildingCost = PlaceFoundation.Instance.GetBuildingData("mageTowerData");
                        buildingName = "Mage Tower";
                        break;
                    case 3:
                        buildingCost = PlaceFoundation.Instance.GetBuildingData("farmData");
                        buildingName = "Farm";
                        break;
                    case 4:
                        buildingCost = PlaceFoundation.Instance.GetBuildingData("forgeData");
                        buildingName = "Forge";
                        break;
                    case 5:
                        buildingCost = PlaceFoundation.Instance.GetBuildingData("siegeWorkshopData");
                        buildingName = "Siege Workshop";
                        break;
                    case 6:
                        buildingCost = PlaceFoundation.Instance.GetBuildingData("warehouseData");
                        buildingName = "Warehouse";
                        break;
                    default:
                        break;
                }
                tooltipText.text = SetTooltipText(buildingCost, buildingName);
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
    private string SetTooltipText(List<int> cost, string name)
    {
        RemoveElementsWithZeroInList(cost, out List<string> newValues, out List<int> newList);
        string tooltip = $"Name:\t\t{name}";
        for (int i = 0; i < newValues.Count; i++)
        {
            tooltip += newValues[i];
            tooltip += newList[i];
        }
        return tooltip;
    }

    public string SetTooltipText(List<int> cost, UpgradeType upgradeType, string name)
    {
        RemoveElementsWithZeroInList(cost, out List<string> newValues, out List<int> newList);
        string tooltip = $"Name:\t\t{name}\nLevel:\t\t{PlayerManager.Instance.GetUpgradeLevel(upgradeType) + 1}";
        for (int i = 0; i < newValues.Count; i++)
        {
            tooltip += newValues[i];
            tooltip += newList[i];
        }
        return tooltip;
    }

    private string SetTooltipText(UpgradeType upgradeType, string name)
    {
        return $"Name:\t\t{name}\nLevel:\t\tMax";
    }

    private void RemoveElementsWithZeroInList(List<int> list, out List<string> newValues, out List<int> newList)
    {
        List<string> values = new List<string>() { "\nFood:\t\t", "\nGold:\t\t", "\nIron:\t\t\t", "\nStone:\t\t", "\nWood:\t\t" };
        int listLength = list.Count;

        newList = new List<int>();
        newValues = new List<string>();

        for (int i = 0; i < listLength; i++)
        {
            if(list[i] != 0)
            {
                newList.Add(list[i]);
                newValues.Add(values[i]);
            }
        }
    }
}
