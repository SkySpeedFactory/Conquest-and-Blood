using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnButton : MonoBehaviour
{
    BuildingSelection buildingSelection;
    UnitSelections unitSelection;
    PlaceFoundation placeFoundation;

    void Start()
    {
        buildingSelection = BuildingSelection.Instance;
        unitSelection = UnitSelections.Instance;
        placeFoundation = PlaceFoundation.Instance;
    }

    /// <summary>
    /// Enter index of unit in list (starting from 0).
    /// This function is called on unit spawning buttons.
    /// </summary>
    /// <param name="indexOfUnit"></param>
    public void ClickButton(int indexOfUnit)
    {
        // Button reacts on buildings
        if(buildingSelection.SelectedBuilding != null)
        {
            Building buildingComponent = buildingSelection.SelectedBuilding.GetComponent<Building>();
            int numberOfUnitTypes = buildingComponent.GetNumberOfUnitTypes();
            if(numberOfUnitTypes -1 >= indexOfUnit)
            {
                buildingSelection.Spawn(indexOfUnit);
                ActionBarManager.Instance.CheckIfUnitSpawnLimitReached(indexOfUnit);
            }
            // When Upgrade Button is pressed
            else
            {
                // Executes the upgrade function
                buildingSelection.SubractUpgradeCostFromResources(indexOfUnit);
                var upgrade = buildingComponent.GetUpgrades()[indexOfUnit
                    - buildingComponent.GetNumberOfUnitTypes()];
                PlayerManager.Instance.UpgradeTechnology(upgrade.TypeOfUpgrade);
                foreach (var unit in unitSelection.GetUnitList())
                {
                    unit.GetComponent<Unit>().UpdateStatsAfterUpgradeIsFinished();
                }

                if (!PlayerManager.Instance.CheckIfUpgradeMaxLevelReached(upgrade.TypeOfUpgrade))
                {
                    ActionBarManager.Instance.CheckIfEnoughResourcesUpgrade(indexOfUnit);
                }
                return;
            }
            ActionBarManager.Instance.CheckIfEnoughResources(indexOfUnit);
            ActionBarManager.Instance.CheckIfUnitSpawnLimitReached(indexOfUnit);
        }

        // Button reacts on units
        else
        {
            if(UnitSelections.Instance.GetSelectedUnitsList()[0].GetComponent<Unit>().GetUnitType() 
                == UnitType.Worker)
            {
                placeFoundation.SetBuilding(indexOfUnit);
                if (placeFoundation.CheckIfEnoughResources(indexOfUnit))
                {
                    placeFoundation.SetHasSelection(true);
                    ActionBarManager.Instance.DeactiveAllButtonsGO();
                }
                else
                {
                    placeFoundation.SetHasSelection(false);
                }
            }
            
        }

    }
    // behaviourIndex gets button number and sets the behaviour. Example: Index number 1 = Offensive behaviour
    public void ClickBehaviourButton(int behaviourIndex)
    {
        if(unitSelection != null)
        {
            foreach (var unit in unitSelection.GetSelectedUnitsList())
            {
                unit.GetComponent<UnitOffensiveBehaviour>().SetUnitBehavior(behaviourIndex);
            }
        }
    }
}
