using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "ScriptableObjects/Upgrades")]
public class ScriptableUpgrades : ScriptableObject
{
    public string UpgradeName;
    public string Description;
    public Sprite Icon;
    public UpgradeType TypeOfUpgrade;

    public float UpgradeValue;

    public int CostFood;
    public int CostGold;
    public int CostIron;
    public int CostStone;
    public int CostWood;
}
