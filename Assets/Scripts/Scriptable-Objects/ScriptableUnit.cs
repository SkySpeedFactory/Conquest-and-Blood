using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "ScriptableObjects/Units")]
public class ScriptableUnit : ScriptableObject
{
    public UnitType UnitType;
    public string UnitName;
    public Sprite Sprite;

    public float MaxHealth;
    public float RegenerationRate;
    public float RegenerationAmount;
    public float MeleeDamage;
    public float MeleeAttackRange;
    public float RangeDamage;
    public float RangeAttackRange;
    public float RangeMinAttackRange;
    public float Armor;
    public float Speed;
    public float Visibility;

    public bool CanHarvest;
    public bool CanBuild;

    public int FoodCost;
    public int GoldCost;
    public int IronCost;
    public int WoodCost;

    public int UnitMilitaryPower;

}
