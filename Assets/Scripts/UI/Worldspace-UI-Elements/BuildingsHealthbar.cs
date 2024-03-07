using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsHealthbar : MonoBehaviour
{
    [SerializeField] private Slider healthBar;

    private CameraUIRotationFPS cameraUIRotationFPS;
    private CameraUIRotation cameraUIRotationRTS;
    private Building buildingStats;

    private void Start()
    {
        buildingStats = gameObject.GetComponentInParent<Building>();
        SetMaxHealth(buildingStats.GetMaxHealth());
        SetUnitName(buildingStats.GetBuildingName());
    }

    private void LateUpdate()
    {
        if (cameraUIRotationFPS != null)
        {
            if (cameraUIRotationFPS.transform.parent.gameObject.activeInHierarchy == true)
            {
                transform.LookAt(transform.position + cameraUIRotationFPS.transform.forward);
            }
        }
        else
        {
            cameraUIRotationFPS = CameraUIRotationFPS.Instance;
        }
        if (cameraUIRotationRTS != null)
        {
            if (cameraUIRotationRTS.gameObject.activeInHierarchy == true)
            {
                transform.LookAt(transform.position + cameraUIRotationRTS.transform.forward);
            }
        }
        else
        {
            cameraUIRotationRTS = CameraUIRotation.Instance;
        }
    }

    public void SetMaxHealth(float maxHealth)
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
    }
    public void SetHealth(float currentHealth) => healthBar.value = currentHealth;

    public void SetUnitName(string name)
    {
        transform.GetComponentInChildren<Text>().text = name;

        if (buildingStats.GetBuildingFaction() == Faction.Player_1)
        {
            transform.GetComponentInChildren<Text>().color = GameManager.Instance.GetPlayerFactionColor32();
        }
        else if (buildingStats.GetBuildingFaction() == Faction.CPU)
        {
            transform.GetComponentInChildren<Text>().color = GameManager.Instance.GetEnemyFactionColor32();
        }
    }
}
