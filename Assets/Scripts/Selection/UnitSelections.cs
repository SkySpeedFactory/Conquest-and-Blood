using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelections : MonoBehaviour
{
    [SerializeField] Canvas resourceUI;
    private List<GameObject> unitList = new List<GameObject>();
    private List<GameObject> selectedUnitsList = new List<GameObject>();

    private static UnitSelections _instance;
    public static UnitSelections Instance { get { return _instance; } }

    // Private Constructor to prevent creating instance
    private UnitSelections() { }

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

    public List<GameObject> GetUnitList() => unitList;
    public List<GameObject> GetSelectedUnitsList() => selectedUnitsList;
    public void AddToUnitList(GameObject unit)
    {
        unitList.Add(unit);
        resourceUI.GetComponent<ResourceUI>().UpdatePopulation(unitList.Count);
    }
    public void RemoveFromUnitList(GameObject unit)
    { 
        unitList.Remove(unit);
        if (resourceUI != null)
        {
            resourceUI.GetComponent<ResourceUI>().UpdatePopulation(unitList.Count);
        }
    }

    public void ClickSelect(GameObject unitToAdd)
    {
        DeselectAll();
        selectedUnitsList.Add(unitToAdd);
        unitToAdd.transform.GetChild(2).gameObject.SetActive(true);
        unitToAdd.GetComponent<UnitMovement>().enabled = true;
    }

    public void ShiftClickSelect(GameObject unitToAdd)
    {
        if (!selectedUnitsList.Contains(unitToAdd))
        {
            selectedUnitsList.Add(unitToAdd);
            unitToAdd.transform.GetChild(2).gameObject.SetActive(true);
            unitToAdd.GetComponent<UnitMovement>().enabled = true;
        }
        else
        {
            selectedUnitsList.Remove(unitToAdd);
            unitToAdd.GetComponent<UnitMovement>().enabled = false;
            unitToAdd.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    public void DragSelect(GameObject unitToAdd)
    {
        if (!selectedUnitsList.Contains(unitToAdd))
        {
            selectedUnitsList.Add(unitToAdd);
            unitToAdd.transform.GetChild(2).gameObject.SetActive(true);
            unitToAdd.GetComponent<UnitMovement>().enabled = true;
        }
    }

    public void DeselectAll()
    {
        foreach (var unit in selectedUnitsList)
        {
            unit.GetComponent<UnitMovement>().enabled = false;
            unit.transform.GetChild(2).gameObject.SetActive(false);
        }
        selectedUnitsList.Clear();
        FactionObjectUI.Instance.ResetFactionObjectUI();
    }
}
