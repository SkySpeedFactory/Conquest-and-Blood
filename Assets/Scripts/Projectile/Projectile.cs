using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] ScriptableProjectile projectileData;
    private bool allProjectileDataInitialized = false;

    // Projectile Data
    private string projectileName;
    private DamageType damageType;
    private float damageMultiplier;
    private float speed;

    private Vector3 startPos;
    private float destroyTimer = 8f;
    
    private float damage = 0;
    private float range = 15;
    
    void Start()
    {
        SetProjectileData();
        startPos = transform.position;
    }

    void Update()
    {
        MoveProjectile();
    }

    private void MoveProjectile()
    {
        float projectileSpeed = speed * Time.deltaTime;
        transform.Translate(Vector3.forward.normalized * projectileSpeed, Space.Self);

        if(Vector3.Distance(transform.position, startPos) >= range * 2)
        {
            Destroy(gameObject);
        }
    }

    private void SetProjectileData()
    {
        projectileName = projectileData.ProjectileName;
        damageType = projectileData.damageType;
        damageMultiplier = projectileData.DamageMultiplier;
        speed = projectileData.Speed;
        allProjectileDataInitialized = true;
    }

    public bool CheckIfAllDataIsInitialized() => allProjectileDataInitialized;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Unit>() != null)
        {
            Unit unit = other.GetComponent<Unit>();
            if(unit.GetUnitFaction() == Faction.CPU)
            {
                unit.TakeDamage(damage);
            }
        }
        else if(other.GetComponent<Building>() != null)
        {
            Building building = other.GetComponent<Building>();
            if(building.GetBuildingFaction() == Faction.CPU)
            {
                building.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
    public void SetDamage(float damage) => this.damage = damage;
    public void SetRange(float range) => this.range = range;
}
