using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarManager : MonoBehaviour
{
    private static ActionBarManager _instance;
    public static ActionBarManager Instance { get { return _instance; } }

    private BuildingSelection buildingSelection;

    [SerializeField] List<Button> buttonsList = new List<Button>();
    [SerializeField] List<Button> behaviourList = new List<Button>();

    [SerializeField] Image behaviourPanel = null;

    private bool isActionBarActive = false;
    private int numOfActiveButtons = 0;

    // Private Constructor to prevent creating instance
    private ActionBarManager() { }

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
        buildingSelection = BuildingSelection.Instance;
    }

    // Used to update all active buttons at once
    public void CheckIfEnoughResources()
    {
        if (isActionBarActive)
        {
            // if building selected
            if (BuildingSelection.Instance.SelectedBuilding != null)
            {
                for (int i = 0; i < numOfActiveButtons; i++)
                {
                    if (!buildingSelection.CheckIfEnoughResources(i))
                    {
                        ButtonEnabled(buttonsList[i], false);
                    }
                    else
                    {
                        ButtonEnabled(buttonsList[i], true);
                    }
                }
            }
            // if worker selected
            else if (UnitSelections.Instance.GetSelectedUnitsList().Count > 0)
            {
                for (int i = 0; i < numOfActiveButtons; i++)
                {
                    if (!PlaceFoundation.Instance.CheckIfEnoughResources(i))
                    {
                        ButtonEnabled(buttonsList[i], false);
                    }
                    else
                    {
                        ButtonEnabled(buttonsList[i], true);
                    }
                }
            }
        }
    }

    // Used to update specific buttons
    public bool CheckIfEnoughResources(int buttonIndex)
    {
        // if building is selected
        if(buildingSelection.SelectedBuilding != null)
        {
            if (!buildingSelection.CheckIfEnoughResources(buttonIndex))
            {
                ButtonEnabled(buttonsList[buttonIndex], false);
                return false;
            }
            else
            {
                ButtonEnabled(buttonsList[buttonIndex], true);
                return true;
            }
        }
        // if worker selected
        else if(UnitSelections.Instance.GetSelectedUnitsList().Count > 0)
        {
            if (!PlaceFoundation.Instance.CheckIfEnoughResources(buttonIndex))
            {
                ButtonEnabled(buttonsList[buttonIndex], false);
                return false;
            }
            else
            {
                ButtonEnabled(buttonsList[buttonIndex], true);
                return true;
            }
        }
        return false;
    }

    public void CheckIfEnoughResourcesUpgrade(int buttonIndex, int upgradeIndex)
    {
        if (!buildingSelection.CheckIfEnoughResourcesUpgrade(upgradeIndex))
        {
            ButtonEnabled(buttonsList[buttonIndex], false);
        }
        else
        {
            ButtonEnabled(buttonsList[buttonIndex], true);
        }
    }

    public void CheckIfEnoughResourcesUpgrade(int index)
    {
        if(!buildingSelection.CheckIfEnoughResourcesUpgrade(index - numOfActiveButtons))
        {
            ButtonEnabled(buttonsList[index], false);
        }
        else
        {
            ButtonEnabled(buttonsList[index], true);
        }
    }

    public void CheckIfUnitSpawnLimitReached(int index)
    {
        if(UnitSelections.Instance.GetUnitList().Count + CheckHowManyUnitsToSpawnInQueue() >= 100)
        {
            ButtonEnabled(buttonsList[index], false);
        }
        else
        {
            ButtonEnabled(buttonsList[index], true);
        }
    }

    public int CheckHowManyUnitsToSpawnInQueue()
    {
        List<GameObject> instBuildingsList = PlaceFoundation.Instance.GetInstBuildingsList();
        int totalUnitsToSpawn = 0;
        foreach (var building in instBuildingsList)
        {
            if(building != null)
            {
                totalUnitsToSpawn += building.GetComponent<Building>().GetUnitsToSpawn();
            }
        }
        return totalUnitsToSpawn;
    }

    public void ActivateButton()
    {
        DeactiveAllButtonsGO();
        isActionBarActive = true;
        if(buildingSelection.SelectedBuilding != null)
        {
            BehaviourPanelEnabled(false);
            int numberOfUpgrades = buildingSelection.SelectedBuilding.GetComponent<Building>().GetUpgrades().Count;
            int NumOfAvailableUnits = buildingSelection.SelectedBuilding.GetComponent<Building>().GetNumberOfUnitTypes();
            // Unit Spawn Buttons
            for (int i = 0; i < NumOfAvailableUnits; i++)
            {
                buttonsList[i].gameObject.SetActive(true);
                Building building = buildingSelection.SelectedBuilding.GetComponent<Building>();
                SetButtonData(i, building.GetUnitSprite(i), building.GetUnitName(i));
                CheckIfEnoughResources(i);
                CheckIfUnitSpawnLimitReached(i);
            }
            // Upgrade Buttons
            for (int i = NumOfAvailableUnits; i < NumOfAvailableUnits + numberOfUpgrades; i++)
            {
                buttonsList[i].gameObject.SetActive(true);
                var upgrade = buildingSelection.SelectedBuilding.GetComponent<Building>().GetUpgrades()[i - NumOfAvailableUnits];
                string upgradeName = upgrade.UpgradeName;
                SetButtonData(i, upgrade.Icon, upgradeName);
                CheckIfEnoughResourcesUpgrade(i, i - NumOfAvailableUnits);
            }
            numOfActiveButtons = NumOfAvailableUnits;
        }
        // Unit Selected
        else if(UnitSelections.Instance.GetSelectedUnitsList().Count > 0)
        {
            switch (UnitSelections.Instance.GetSelectedUnitsList()[0].GetComponent<Unit>().GetUnitType())
            {
                case UnitType.Worker:
                    BehaviourPanelEnabled(false);
                    var buildingsList = PlaceFoundation.Instance.GetBuildingsList();
                    numOfActiveButtons = buildingsList.Count;
                    for (int i = 0; i < buildingsList.Count; i++)
                    {
                        buttonsList[i].gameObject.SetActive(true);
                        SetButtonData(i, PlaceFoundation.Instance.GetBuildingSprite(i), buildingsList[i].name);
                        bool enoughResources = CheckIfEnoughResources(i);
                        bool maxBuildCountReached = PlaceFoundation.Instance.CheckIfReachedMaxBuildCount(i);
                        bool isUnlocked = PlaceFoundation.Instance.CheckIfUnlocked(i);

                        if (enoughResources && !maxBuildCountReached && isUnlocked)
                        {
                            ButtonEnabled(buttonsList[i], true);
                        }
                        else
                        {
                            ButtonEnabled(buttonsList[i], false);
                        }
                    }
                    break;
                case UnitType.Swordsman:
                    BehaviourPanelEnabled(true);
                    ActivateBehaviourButtons();
                    break;
                case UnitType.Archer:
                    BehaviourPanelEnabled(true);
                    ActivateBehaviourButtons();
                    break;
                case UnitType.Mage:
                    BehaviourPanelEnabled(true);
                    ActivateBehaviourButtons();
                    break;
                case UnitType.Catapult:
                    BehaviourPanelEnabled(true);
                    ActivateBehaviourButtons();
                    break;
                default:
                    break;
            }
        }
    }
    
    private void SetButtonData(int buttonIndex, Sprite sprite, string text)
    {
        buttonsList[buttonIndex].GetComponent<Image>().sprite = sprite;
        buttonsList[buttonIndex].GetComponentInChildren<Text>().text = text;
    }

    public void DeactiveAllButtonsGO()
    {
        isActionBarActive = false;
        if(buttonsList.Count > 0)
        {
            for (int i = 0; i < buttonsList.Count; i++)
            {
                if (i < buttonsList.Count)
                {
                    buttonsList[i].gameObject.SetActive(false);
                }
                
                if(i < behaviourList.Count)
                {
                    behaviourList[i].gameObject.SetActive(false);
                }
            }
        }
        numOfActiveButtons = 0;
    }

    public void ButtonEnabled(Button button, bool isEnabled) => button.interactable = isEnabled;

    // shows unit behaviour
    public void ActivateBehaviourButtons()
    {
        numOfActiveButtons += behaviourList.Count;
        var selectedUnitList = UnitSelections.Instance.GetSelectedUnitsList();
        int firstUnitBehaviour = selectedUnitList[0].GetComponent<UnitOffensiveBehaviour>().GetUnitBehaviour();
        bool isSameBehaviour = false;
        int differentBehaviour = 0;
        // Checks if all behaviours are the same
        foreach (var unit in selectedUnitList)
        {
            if (unit.GetComponent<UnitOffensiveBehaviour>().GetUnitBehaviour() != firstUnitBehaviour)
            {
                differentBehaviour++;
            }
        }

        if (differentBehaviour == 0)
        {
            isSameBehaviour = true;
        }

        // only shows active behaviour if its the same for all units
        for (int i = 0; i < behaviourList.Count; i++)
        {
            behaviourList[i].gameObject.SetActive(true);

            if(firstUnitBehaviour != i || !isSameBehaviour)
            {
                behaviourList[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else if(isSameBehaviour)
            {
                behaviourList[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public int GetNumOfActiveButtons() => numOfActiveButtons;

    public List<Button> GetButtonList() => buttonsList;

    private void BehaviourPanelEnabled(bool active)
    {
        behaviourPanel.gameObject.SetActive(active);
    }
}
