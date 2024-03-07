using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionObjectUI : MonoBehaviour
{
    private static FactionObjectUI _instance;
    public static FactionObjectUI Instance { get { return _instance; } }

    [SerializeField] Image factionObjectUIPanel;

    [SerializeField] Text factionObjectName;
    [SerializeField] Image factionObjectImage;
    [SerializeField] Slider factionObjectHealth;
    [SerializeField] Text factionObjectMeleeDamage;
    [SerializeField] Text factionObjectRangedDamage;
    [SerializeField] Text factionObjectDefense;

    [SerializeField] Sprite damageSprite;
    [SerializeField] Sprite rangedDamageSprite;
    [SerializeField] Sprite defenseSprite;

    // Private Constructor to prevent creating instance
    private FactionObjectUI() { }
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

    public void UpdateFactionObjectUI()
    {
        // if building was clicked
        if (BuildingSelection.Instance.SelectedBuilding != null)
        {
            SetFactionObjectUIVisibility(true);
            factionObjectName.text = BuildingSelection.Instance.SelectedBuilding.GetComponent<Building>().GetBuildingName();
            factionObjectImage.sprite = BuildingSelection.Instance.SelectedBuilding.GetComponent<Building>().GetBuildingSprite();
            factionObjectHealth.value = 1;
            factionObjectMeleeDamage.gameObject.SetActive(false);
            factionObjectMeleeDamage.text = null;
            factionObjectMeleeDamage.transform.GetChild(0).GetComponent<Image>().sprite = damageSprite;
            factionObjectRangedDamage.gameObject.SetActive(false);
            factionObjectRangedDamage.text = null;
            factionObjectRangedDamage.transform.GetChild(0).GetComponent<Image>().sprite = rangedDamageSprite;
            factionObjectDefense.text = BuildingSelection.Instance.SelectedBuilding.GetComponent<Building>().GetArmor().ToString();
            factionObjectDefense.transform.GetChild(0).GetComponent<Image>().sprite = defenseSprite;
        }
        // if unit was clicked
        else if (UnitSelections.Instance.GetSelectedUnitsList().Count == 1)
        {
            SetFactionObjectUIVisibility(true);
            Unit unit = UnitSelections.Instance.GetSelectedUnitsList()[0].GetComponent<Unit>();
            factionObjectName.text = unit.GetUnitName();
            factionObjectImage.sprite = unit.GetUnitSprite();
            factionObjectHealth.maxValue = unit.GetMaxHelath();
            factionObjectHealth.value = unit.GetCurrentHealth();
            factionObjectMeleeDamage.gameObject.SetActive(true);
            factionObjectMeleeDamage.text = unit.GetMeleeDamage().ToString();
            factionObjectMeleeDamage.transform.GetChild(0).GetComponent<Image>().sprite = damageSprite;
            factionObjectRangedDamage.gameObject.SetActive(true);
            factionObjectRangedDamage.text = unit.GetRangeDamage().ToString();
            factionObjectRangedDamage.transform.GetChild(0).GetComponent<Image>().sprite = rangedDamageSprite;
            factionObjectDefense.gameObject.SetActive(true);
            factionObjectDefense.text = unit.GetArmor().ToString();
            factionObjectDefense.transform.GetChild(0).GetComponent<Image>().sprite = defenseSprite;
        }
    }

    public void SetFactionObjectHealthbarSliderValue(float currentHealth)
    {
        factionObjectHealth.value = currentHealth;
    }

    public void ResetFactionObjectUI()
    {
        factionObjectName.text = null;
        factionObjectImage.sprite = null;
        factionObjectHealth.maxValue = 1;
        factionObjectHealth.value = 1;
        factionObjectMeleeDamage.text = null;
        factionObjectMeleeDamage.transform.GetChild(0).GetComponent<Image>().sprite = null;
        factionObjectRangedDamage.text = null;
        factionObjectRangedDamage.transform.GetChild(0).GetComponent<Image>().sprite = null;
        factionObjectDefense.text = null;
        factionObjectDefense.transform.GetChild(0).GetComponent<Image>().sprite = null;
        SetFactionObjectUIVisibility(false);
    }

    public void SetFactionObjectUIVisibility(bool isVisible)
    {
        if(factionObjectUIPanel != null)
        {
            for (int i = 0; i < factionObjectUIPanel.transform.childCount; i++)
            {
                if(factionObjectUIPanel.transform.GetChild(0).gameObject != null)
                {
                    factionObjectUIPanel.transform.GetChild(i).gameObject.SetActive(isVisible);
                }
            }
        }
    }
}
