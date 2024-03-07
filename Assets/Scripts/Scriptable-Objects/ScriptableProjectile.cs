using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "ScriptableObjects/Projectile")]
public class ScriptableProjectile : ScriptableObject
{
    public string ProjectileName;
    public DamageType damageType;

    public float DamageMultiplier;
    public float Speed;
}
