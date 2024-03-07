using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDrag : MonoBehaviour
{
    // Instances
    BuildingSelection buildingSelection;
    UnitSelections unitSelection;

    private Camera cam;

    // Graphical
    [SerializeField] RectTransform boxVisual;


    // Logical
    Rect selectionBox;
    Vector2 startPosition;
    Vector2 endPosition;

    void Start()
    {
        cam = Camera.main;
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        DrawVisual();
        buildingSelection = BuildingSelection.Instance;
        unitSelection = UnitSelections.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Start click
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            selectionBox = new Rect();
        }

        // Drag
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
            DrawVisual();
            DrawSelection();
        }

        // End Click
        if (Input.GetMouseButtonUp(0))
        {
            SelectUnits();
            startPosition = Vector2.zero;
            endPosition = Vector2.zero;
            DrawVisual();
            
            if (unitSelection.GetSelectedUnitsList().Count == 1)
            {
                buildingSelection.DeselectBuilding();
                ActionBarManager.Instance.ActivateButton();
                FactionObjectUI.Instance.UpdateFactionObjectUI();
                MultipleUnitsUI.Instance.SetSlotsVisible(false);
            }
            // Check if all units are same type
            else if(unitSelection.GetSelectedUnitsList().Count > 1)
            {
                MultipleUnitsUI.Instance.SetSlotsVisible(true);
                FactionObjectUI.Instance.SetFactionObjectUIVisibility(false);
                buildingSelection.DeselectBuilding();
                bool isSameType = true;

                if(unitSelection.GetSelectedUnitsList()[0].GetComponent<Unit>().GetUnitType() == UnitType.Worker)
                {
                    for (int i = 0; i < unitSelection.GetSelectedUnitsList().Count; i++)
                    {
                        if(unitSelection.GetSelectedUnitsList()[i].GetComponent<Unit>().GetUnitType() != UnitType.Worker)
                        {
                            isSameType = false;
                        }
                    }
                    if(isSameType)
                    {
                        ActionBarManager.Instance.ActivateButton();
                    }
                }
                else
                {
                    bool noWorkerPresent = true;
                    for (int i = 0; i < unitSelection.GetSelectedUnitsList().Count; i++)
                    {
                        if(unitSelection.GetSelectedUnitsList()[i].GetComponent<Unit>().GetUnitType() == UnitType.Worker)
                        {
                            noWorkerPresent = false;
                        }
                    }
                    if(noWorkerPresent)
                    {
                        ActionBarManager.Instance.ActivateBehaviourButtons();
                    }
                }

            }
        }
    }

    private void DrawVisual()
    {
        Vector2 boxStartPosition = startPosition;
        Vector2 boxEndPosition = endPosition;

        Vector2 boxCenter = (boxStartPosition + boxEndPosition) / 2;
        boxVisual.position = boxCenter;

        Vector2 boxSize = new Vector2(Mathf.Abs(boxStartPosition.x - boxEndPosition.x), Mathf.Abs(boxStartPosition.y - boxEndPosition.y));
        boxVisual.sizeDelta = boxSize;
    }

    private void DrawSelection()
    {
        // X calculation
        if (Input.mousePosition.x < startPosition.x)
        {
            // Drag left
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPosition.x;
        }
        else
        {
            // Drag right
            selectionBox.xMin = startPosition.x;
            selectionBox.xMax = Input.mousePosition.x;
        }

        // Y calculation

        if (Input.mousePosition.y < startPosition.y)
        {
            // Drag left
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPosition.y;
        }
        else
        {
            // Drag right
            selectionBox.yMin = startPosition.y;
            selectionBox.yMax = Input.mousePosition.y;
        }
    }

    private void SelectUnits()
    {
        foreach (var unit in UnitSelections.Instance.GetUnitList())
        {
            if (selectionBox.Contains(cam.WorldToScreenPoint(unit.transform.position)))
            {
                UnitSelections.Instance.DragSelect(unit);
            }
        }
    }

    public void ChangeCameraForDrag(Camera newCamera) => cam = newCamera;
}
