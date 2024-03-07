using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleUnitsUI : MonoBehaviour
{
    private static MultipleUnitsUI _instance;
    public static MultipleUnitsUI Instance { get { return _instance; } }

    int worker = 0;
    int swordsman = 0;
    int archer = 0;
    int mage = 0;
    int catapult = 0;

    // Order by Index: Worker, Swordsman, Archer, Mage, Catapult
    List<int> unitTypesCounterList = new List<int>() { 0, 0, 0, 0, 0 };
    List<GameObject> selectedUnitTypesList = new List<GameObject>();


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

    private int CalcNumberOfSelectedUnitTypes()
    {
        worker = 0;
        swordsman = 0;
        archer = 0;
        mage = 0;
        catapult = 0;

        selectedUnitTypesList.Clear();
        for (int i = 0; i < unitTypesCounterList.Count; i++)
        {
            unitTypesCounterList[i] = 0;
        }

        if(UnitSelections.Instance.GetSelectedUnitsList().Count > 1)
        {
            foreach (var unit in UnitSelections.Instance.GetSelectedUnitsList())
            {
                Unit unitScript = unit.GetComponent<Unit>();
                if (unitScript.GetUnitType() == UnitType.Worker)
                {
                    unitTypesCounterList[0]++;
                    if (worker <= 0)
                    {
                        worker = 1;
                        selectedUnitTypesList.Add(unit);
                    }
                }
                if (unitScript.GetUnitType() == UnitType.Swordsman)
                {
                    unitTypesCounterList[1]++;
                    if(swordsman <= 0)
                    {
                        swordsman++;
                        selectedUnitTypesList.Add(unit);
                    }
                }
                if(unitScript.GetUnitType() == UnitType.Archer)
                {
                    unitTypesCounterList[2]++;
                    if(archer <= 0)
                    {
                        archer++;
                        selectedUnitTypesList.Add(unit);
                    }
                }
                if(unitScript.GetUnitType() == UnitType.Mage)
                {
                    unitTypesCounterList[3]++;
                    if(mage <= 0)
                    {
                        mage++;
                        selectedUnitTypesList.Add(unit);
                    }
                }
                if(unitScript.GetUnitType() == UnitType.Catapult)
                {
                    unitTypesCounterList[4]++;
                    if(catapult <= 0)
                    {
                        catapult++;
                        selectedUnitTypesList.Add(unit);
                    }
                }
            }
        }
        return worker + swordsman + archer + mage + catapult;
    }

    public void SetSlotsVisible(bool isVisible)
    {
        if(isVisible)
        {
            for (int i = 0; i < CalcNumberOfSelectedUnitTypes(); i++)
            {
                if(!transform.GetChild(i).gameObject.activeSelf)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
                
                SetSlotData(i, GetUnitCount(i), GetUnitSprite(i));
            }
        }
        else
        {
            for (int i = 0; i < this.gameObject.transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private Sprite GetUnitSprite(int index)
    {
        return selectedUnitTypesList[index].gameObject.GetComponent<Unit>().GetUnitSprite();
    }

    private void SetSlotData(int index, int count, Sprite sprite)
    {
        var child = transform.GetChild(index);
        child.GetComponent<Image>().sprite = sprite;
        child.GetChild(0).GetComponent<Text>().text = count.ToString();
    }

    private int GetUnitCount(int index)
    {
        int count = 0;
        switch (selectedUnitTypesList[index].gameObject.GetComponent<Unit>().GetUnitType())
        {
            case UnitType.Worker:
                count = unitTypesCounterList[0];
                break;
            case UnitType.Swordsman:
                count = unitTypesCounterList[1];
                break;
            case UnitType.Archer:
                count = unitTypesCounterList[2];
                break;
            case UnitType.Mage:
                count = unitTypesCounterList[3];
                break;
            case UnitType.Catapult:
                count = unitTypesCounterList[4];
                break;
            default:
                break;
        }
        return count;
    }
}
