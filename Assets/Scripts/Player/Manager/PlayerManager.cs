using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance { get { return _instance; } }

    private PlayerResources playerResources;
    private UpgradeValues upgradeValues;

    [SerializeField] Canvas resourceUI;

    [SerializeField] GameObject playerCharacter;

    // Private Constructor to prevent creating instance
    private PlayerManager() { }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            playerResources = _instance.GetComponent<PlayerResources>();
            upgradeValues = _instance.GetComponent<UpgradeValues>();
        }
    }

    public int GetPlayerResources(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                return playerResources.GetResourceFood();
            case ResourceType.Gold:
                return playerResources.GetResourceGold();
            case ResourceType.Iron:
                return playerResources.GetResourceIron();
            case ResourceType.Stone:
                return playerResources.GetResourceStone();
            case ResourceType.Wood:
                return playerResources.GetResourceWood();
            default:
                return 0;
        }
    }

    public void SetPlayerResources(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                playerResources.SetResourceFood(amount);
                break;
            case ResourceType.Gold:
                playerResources.SetResourceGold(amount);
                break;
            case ResourceType.Iron:
                playerResources.SetResourceIron(amount);
                break;
            case ResourceType.Stone:
                playerResources.SetResourceStone(amount);
                break;
            case ResourceType.Wood:
                playerResources.SetResourceWood(amount);
                break;
        }
        resourceUI.GetComponent<ResourceUI>().UpdateResourceUI(resourceType);
        ActionBarManager.Instance.CheckIfEnoughResources();
    }

    public void UpgradeTechnology(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.ArcaneUnderstanding:
                upgradeValues.UpgradeArcaneUnderstanding();
                break;
            case UpgradeType.ArmorFit:
                upgradeValues.UpgradeArmorFits();
                break;
            case UpgradeType.DraftHorses:
                upgradeValues.UpgradeDraftHorses();
                break;
            case UpgradeType.ForgedArrowheads:
                upgradeValues.UpgradeForgedArrowheads();
                break;
            case UpgradeType.ForgedBlades:
                upgradeValues.UpgradeForgedBlades();
                break;
            case UpgradeType.BetterBaskets:
                upgradeValues.UpgradeBetterBaskets();
                break;
            default:
                Debug.LogWarning("Upgrade does not exist");
                break;
        }
    }

    public int GetUpgradeLevel(UpgradeType upgradeType) => upgradeValues.GetUpgradeLevel(upgradeType);
    public int GetMaxUpgradeLevel() => upgradeValues.GetMaxUpgradeLevel();
    public bool CheckIfUpgradeMaxLevelReached(UpgradeType upgradeType) => GetUpgradeLevel(upgradeType) >= (GetMaxUpgradeLevel());
}

