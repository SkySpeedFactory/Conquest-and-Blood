using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionSystem : MonoBehaviour
{
    private static SelectionSystem _instance;
    public static SelectionSystem Instance { get { return _instance; } }

    // Instances
    UnitSelections unitSelections;
    PlaceFoundation placeFoundation;
    BuildingSelection buildingSelection;
    ActionBarManager actionBarManager;

    // Layers
    [SerializeField] LayerMask clickable;
    [SerializeField] LayerMask buildingLayer;
    [SerializeField] LayerMask groundLayer;

    // Raycast
    public RaycastHit hit;

    // Private Constructor to prevent creating instance
    private SelectionSystem() { }

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
        unitSelections = UnitSelections.Instance;
        placeFoundation = PlaceFoundation.Instance;
        buildingSelection = BuildingSelection.Instance;
        actionBarManager = ActionBarManager.Instance;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (placeFoundation.GetIsBuildingSelected())
            {
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                if(unitSelections.GetSelectedUnitsList().Count < 2)
                {
                    MultipleUnitsUI.Instance.SetSlotsVisible(false);
                }
                
                switch (hit.transform.tag)
                {
                    case "Unit":
                        buildingSelection.DeselectBuilding();
                        if(hit.transform.GetComponent<Unit>().GetUnitFaction() == Faction.CPU)
                        {
                            return;
                        }
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            unitSelections.ShiftClickSelect(hit.collider.gameObject);
                            if(unitSelections.GetSelectedUnitsList().Count > 1)
                            {
                                MultipleUnitsUI.Instance.SetSlotsVisible(true);
                            }
                        }
                        else
                        {
                            unitSelections.ClickSelect(hit.collider.gameObject);
                            actionBarManager.ActivateButton();
                            FactionObjectUI.Instance.UpdateFactionObjectUI();
                            return;
                        }
                        break;
                    case "Building":
                        MultipleUnitsUI.Instance.SetSlotsVisible(false);
                        if (hit.transform.GetComponent<Building>().GetBuildingFaction() == Faction.CPU)
                        {
                            return;
                        }
                        if (unitSelections.GetSelectedUnitsList().Count > 0)
                        {
                            unitSelections.DeselectAll();
                            actionBarManager.DeactiveAllButtonsGO();
                        }
                        if (buildingSelection.SelectedBuilding != null)
                        {
                            buildingSelection.DeselectBuilding();
                            if (hit.transform.CompareTag("Building") && buildingSelection.CheckIfFinished(hit))
                            {
                                buildingSelection.SelectBuilding();
                            }
                        }
                        else if (buildingSelection.SelectedBuilding == null)
                        {
                            if (hit.transform.CompareTag("Building") && buildingSelection.CheckIfFinished(hit))
                            {
                                buildingSelection.SelectBuilding();
                            }
                        }
                        break;
                    case "Ground":
                        buildingSelection.DeselectBuilding();
                        unitSelections.DeselectAll();
                        actionBarManager.DeactiveAllButtonsGO();
                        MultipleUnitsUI.Instance.SetSlotsVisible(false);
                        break;
                    default:
                        buildingSelection.DeselectBuilding();
                        unitSelections.DeselectAll();
                        actionBarManager.DeactiveAllButtonsGO();
                        MultipleUnitsUI.Instance.SetSlotsVisible(false);
                        break;
                }
            }
        }       
    }

    public RaycastHit GetRayCastHit() => hit;
}
