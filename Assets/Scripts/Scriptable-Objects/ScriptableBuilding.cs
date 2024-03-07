using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Building", menuName = "ScriptableObjects/Building")]
public class ScriptableBuilding : ScriptableObject
{
    public string BuildingName;
    public Sprite Sprite;
    public Faction BuildingFaction;

    public float MaxHealth;
    public float Armor;

    public bool CanRecruit;
    public bool HasTechnology;
    public bool HasStorage;
    public float StorageOffset;
    public int MaxBuildLimit;

    public List<ScriptableUpgrades> Upgrades;
    public List<GameObject> AvailableUnits;

    public int FoodCost;
    public int GoldCost;
    public int IronCost;
    public int StoneCost;
    public int WoodCost;
}
