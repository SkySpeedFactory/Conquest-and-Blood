using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeValues : MonoBehaviour
{
    [SerializeField] List<ScriptableUpgrades> upgrades = new List<ScriptableUpgrades>();

    private int maxUpgradeLevel = 3;

    private int upgradeLevelForgedArrowheads = 0;
    private int upgradeLevelForgedBlades = 0;
    private int upgradeLevelArmorFit = 0;
    private int upgradeLevelBetterBaskets = 0;
    private int upgradeLevelDraftHorses = 0;
    private int upgradeLevelArcaneUnderstanding = 0;

    public void UpgradeForgedArrowheads()
    {
        if (upgradeLevelForgedArrowheads < maxUpgradeLevel)
        {
            upgradeLevelForgedArrowheads++;
        }
    }

    public void UpgradeForgedBlades()
    {
        if (upgradeLevelForgedBlades < maxUpgradeLevel)
        {
            upgradeLevelForgedBlades++;
        }
    }

    public void UpgradeArmorFits()
    {
        if (upgradeLevelArmorFit < maxUpgradeLevel)
        {
            upgradeLevelArmorFit++;
        }
    }

    public void UpgradeDraftHorses()
    {
        if (upgradeLevelDraftHorses < maxUpgradeLevel)
        {
            upgradeLevelDraftHorses++;
        }
    }

    public void UpgradeArcaneUnderstanding()
    {
        if (upgradeLevelArcaneUnderstanding < maxUpgradeLevel)
        {
            upgradeLevelArcaneUnderstanding++;
        }
    }

    public void UpgradeBetterBaskets()
    {
        if (upgradeLevelBetterBaskets < maxUpgradeLevel)
        {
            upgradeLevelBetterBaskets++;
        }
    }

    public float GetUpgradeValues(UpgradeType upgradeType)
    {
        float upgradeLevel = 0;
        switch (upgradeType)
        {
            case UpgradeType.ArcaneUnderstanding:
                upgradeLevel = upgradeLevelArcaneUnderstanding;
                break;
            case UpgradeType.ArmorFit:
                upgradeLevel = upgradeLevelArmorFit;
                break;
            case UpgradeType.DraftHorses:
                upgradeLevel = upgradeLevelDraftHorses;
                break;
            case UpgradeType.ForgedArrowheads:
                upgradeLevel = upgradeLevelForgedArrowheads;
                break;
            case UpgradeType.ForgedBlades:
                upgradeLevel = upgradeLevelForgedBlades;
                break;
            case UpgradeType.BetterBaskets:
                upgradeLevel = upgradeLevelBetterBaskets;
                break;
            default:
                break;
        }
        return upgrades.Find(item => item.TypeOfUpgrade == upgradeType).UpgradeValue * upgradeLevel;
        
    }

    public int GetUpgradeLevel(UpgradeType upgradeType)
    {
        int upgradeLevel = 0;
        switch (upgradeType)
        {
            case UpgradeType.ArcaneUnderstanding:
                upgradeLevel = upgradeLevelArcaneUnderstanding;
                break;
            case UpgradeType.ArmorFit:
                upgradeLevel = upgradeLevelArmorFit;
                break;
            case UpgradeType.DraftHorses:
                upgradeLevel = upgradeLevelDraftHorses;
                break;
            case UpgradeType.ForgedArrowheads:
                upgradeLevel = upgradeLevelForgedArrowheads;
                break;
            case UpgradeType.ForgedBlades:
                upgradeLevel = upgradeLevelForgedBlades;
                break;
            case UpgradeType.BetterBaskets:
                upgradeLevel = upgradeLevelBetterBaskets;
                break;
            default:
                break;
        }
        return upgradeLevel;
    }

    //public 

    public int GetMaxUpgradeLevel() => maxUpgradeLevel;

    public void UpgradeAllTechnologies()
    {
        UpgradeArcaneUnderstanding();
        UpgradeArmorFits();
        UpgradeDraftHorses();
        UpgradeForgedArrowheads();
        UpgradeForgedBlades();
        UpgradeBetterBaskets();
    }
}
