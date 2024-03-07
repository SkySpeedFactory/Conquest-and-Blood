using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    private GameManager gameManager;
    private UnitSelections unitSelections;
    [SerializeField] ScriptableUnit unitData;

    [SerializeField] Faction unitFaction;
    private UpgradeValues upgradeValues = null;

    private UnitType unitType;
    private string unitName;
    [SerializeField] Sprite sprite;

    private float maxHealth;
    private float regenerationRate;
    private float regenerationAmount;
    private float regenerationCooldown;
    private float currentHealth;
    private float meleeDamage;
    private float meleeAttackRange;
    private float rangeDamage;
    private float rangeAttackRange;
    private float rangeMinAttackRange;
    private float armor;
    private float speed;
    private float visibility;

    private bool canHarvest;
    private bool canBuild;

    private int foodCost;
    private int goldCost;
    private int ironCost;
    private int woodCost;

    private int militaryStrength;

    private bool allUnitDataInitialized = false;

    private void Awake()
    {
        SetUnitData();
    }
    void Start()
    {
        if (unitFaction != Faction.CPU)
        {
            if (gameObject.GetComponent<PlayerController>() == null)
            {
                UnitSelections.Instance.AddToUnitList(this.gameObject);
            }
            else
            {
                gameManager = GameManager.Instance;
            }
            unitSelections = UnitSelections.Instance;
            upgradeValues = PlayerManager.Instance.gameObject.GetComponent<UpgradeValues>();
        }
        else
        {
            upgradeValues = CPUManager.Instance.gameObject.GetComponent<UpgradeValues>();
            if (unitType == UnitType.Worker)
            {
                CPUUnitHandler.Instance.AddToCPUVillagerUnitList(this.gameObject);
            }
            else
            {
                if (unitType != UnitType.Archmage)
                {
                    CPUUnitHandler.Instance.AddToCPUMilitaryUnitList(this.gameObject);
                }
            }
        }
        SetStatsWithUpgrade(unitType);
        regenerationCooldown = regenerationRate;
        
    }

    private void Update()
    {
        regenerationCooldown -= Time.deltaTime;
        RegenerateHealth();
    }

    private void Respawn()
    {
        CharacterController cc = gameObject.GetComponent<CharacterController>();
        cc.enabled = false;
        transform.GetChild(2).gameObject.SetActive(false);
        transform.position = PlayerSpawnPoint.Instance.transform.position;
        cc.enabled = true;
        transform.GetChild(2).gameObject.SetActive(true);
        gameObject.GetComponent<PlayerController>().SetMaxHealth();
        currentHealth = maxHealth;
        HUDUIHandler.Instance.GetInteractText().gameObject.SetActive(false);
        if(gameObject.GetComponent<PlayerController>().IsPlayerInRTS())
        {
            gameObject.GetComponent<PlayerController>().ActvateFPS();
        }
    }

    private void OnDestroy()
    {
        if (unitFaction != Faction.CPU)
        {
            UnitSelections.Instance.RemoveFromUnitList(this.gameObject);
            if (unitSelections.GetSelectedUnitsList().Contains(this.gameObject))
            {
                unitSelections.GetSelectedUnitsList().Remove(this.gameObject);
                if (unitSelections.GetSelectedUnitsList().Count > 0)
                {
                    FactionObjectUI.Instance.UpdateFactionObjectUI();
                    MultipleUnitsUI.Instance.SetSlotsVisible(true);
                }
                else
                {
                    FactionObjectUI.Instance.ResetFactionObjectUI();
                    MultipleUnitsUI.Instance.SetSlotsVisible(false);
                }
            }
        }
        else
        {
            if (unitType == UnitType.Worker)
            {
                CPUUnitHandler.Instance.RemoveFromCPUVillagerUnitList(this.gameObject);
            }
            else
            {
                CPUUnitHandler.Instance.RemoveFromCPUMilitaryUnitList(this.gameObject);
            }
        }
        GameManager.Instance.CheckConquestWinConditions();
    }

    public void PrintUnitData()
    {
        print(
            $"Unit Faction ID: {unitFaction}, \n" +
            $"Type: {unitType}, Name {unitName}, MaxHealth: {maxHealth}, \n" +
            $"Melee Damage: {meleeDamage}, Range Damage: {rangeDamage}, \n" +
            $"Melee Attack Range: {meleeAttackRange}, Range Attack Range: {rangeAttackRange}, \n" +
            $"Armor: {armor}, Speed: {speed}, Visibility: {visibility} \n" +
            $"can Harvest {canHarvest}, can Build: {canBuild}"
            );
    }

    // Setter
    private void SetUnitData()
    {
        unitType = unitData.UnitType;
        unitName = unitData.UnitName;
        maxHealth = unitData.MaxHealth;
        regenerationRate = unitData.RegenerationRate;
        regenerationAmount = unitData.RegenerationAmount;
        meleeDamage = unitData.MeleeDamage;
        meleeAttackRange = unitData.MeleeAttackRange;
        rangeDamage = unitData.RangeDamage;
        rangeAttackRange = unitData.RangeAttackRange;
        rangeMinAttackRange = unitData.RangeMinAttackRange;
        armor = unitData.Armor;
        speed = unitData.Speed;
        visibility = unitData.Visibility;
        canHarvest = unitData.CanHarvest;
        canBuild = unitData.CanBuild;
        foodCost = unitData.FoodCost;
        goldCost = unitData.GoldCost;
        ironCost = unitData.IronCost;
        woodCost = unitData.WoodCost;
        militaryStrength = unitData.UnitMilitaryPower;
        currentHealth = maxHealth;
        allUnitDataInitialized = true;
    }
    public void SetUnitFaction(Faction faction) => unitFaction = faction;
    public void UpdateStatsAfterUpgradeIsFinished() => SetStatsWithUpgrade(unitType);

    // Getter
    public UnitType GetUnitType() => unitType;
    public Sprite GetUnitSprite() => sprite;
    public string GetUnitName() => unitData.UnitName;
    public Faction GetUnitFaction() => unitFaction;
    public float GetMaxHelath() => maxHealth;
    public float GetMeleeDamage() => meleeDamage;
    public float GetMeleeAttackRange() => meleeAttackRange;
    public float GetRangeDamage() => rangeDamage;
    public float GetRangeAttackRange() => rangeAttackRange;
    public float GetRangeMinAttackRange() => rangeMinAttackRange;
    public float GetArmor() => armor;
    public float GetUnitMovementSpeed() => speed;
    public float GetUnitVisibility() => visibility;
    public bool CheckIfUnitCanHarvest() => canHarvest;
    public bool CheckIfUnitCanBuild() => canBuild;
    public int GetFoodCost() => foodCost;
    public int GetMilitaryStrength() => militaryStrength;
    public float GetCurrentHealth() => currentHealth;
    public ScriptableUnit GetUnitData() => unitData;
    public bool CheckIfAllUnitDataInizialized() => allUnitDataInitialized;

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(gameObject.GetComponent<PlayerController>() != null)
        {
            gameObject.GetComponent<PlayerController>().SetHealth(currentHealth);
            if (currentHealth <= 0f)
            {
                gameManager.CheckKingslayerWinConditions(this.gameObject);
                Respawn();
            }
        }
        else
        {
            gameObject.GetComponentInChildren<UnitsHealthBar>().SetHealth(currentHealth);

            if (unitFaction == Faction.Player_1)
            {
                if (unitSelections.GetSelectedUnitsList().Contains(this.gameObject))
                {
                    if (gameObject == unitSelections.GetSelectedUnitsList()[0])
                    {
                        FactionObjectUI.Instance.SetFactionObjectHealthbarSliderValue(currentHealth);
                    }
                }
            }
            if (currentHealth <= 0f)
            {
                if (gameObject.GetComponent<CPUArchMage>() != null)
                {
                    UnitSelections.Instance.DeselectAll();
                    gameObject.GetComponent<CPUArchMage>().ChangeAliveState(false);
                }
                else
                {
                    gameObject.GetComponentInChildren<UnitAnimation>().Death();
                }
            }
        }
    }

    public void DestroyUnit()
    {
        Destroy(this.gameObject);
    }

    private void RegenerateHealth()
    {
        if(unitType == UnitType.Archmage)
        {
            if(regenerationCooldown <= 0 && currentHealth < maxHealth)
            {
                currentHealth += regenerationAmount;
                if(currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }

                if (unitFaction == Faction.Player_1)
                {
                    gameObject.GetComponent<PlayerController>().SetHealth(currentHealth);
                }
                else
                {
                    gameObject.GetComponentInChildren<UnitsHealthBar>().SetHealth(currentHealth);
                }

                regenerationCooldown = regenerationRate;
            }
        }
    }

    private void SetStatsWithUpgrade(UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.Archer:
                armor = unitData.Armor + upgradeValues.GetUpgradeValues(UpgradeType.ArmorFit);
                rangeDamage = unitData.RangeDamage + upgradeValues.GetUpgradeValues(UpgradeType.ForgedArrowheads);
                
                break;
            case UnitType.Catapult:
                speed = unitData.Speed + upgradeValues.GetUpgradeValues(UpgradeType.DraftHorses);
                break;
            case UnitType.Mage:
                armor = unitData.Armor + upgradeValues.GetUpgradeValues(UpgradeType.ArmorFit);
                rangeDamage = unitData.RangeDamage + upgradeValues.GetUpgradeValues(UpgradeType.ArcaneUnderstanding);
                break;
            case UnitType.Swordsman:
                armor = unitData.Armor + upgradeValues.GetUpgradeValues(UpgradeType.ArmorFit);
                meleeDamage = unitData.MeleeDamage + upgradeValues.GetUpgradeValues(UpgradeType.ForgedBlades);
                break;
            case UnitType.Worker:
                armor = unitData.Armor + upgradeValues.GetUpgradeValues(UpgradeType.ArmorFit);
                if (unitFaction == Faction.CPU)
                {
                    gameObject.GetComponent<CPUGatherer>().CarryingcapacityWithUpgrades((int)upgradeValues.GetUpgradeValues(UpgradeType.BetterBaskets));
                }
                else
                {
                    gameObject.GetComponent<Gatherer>().CarryingcapacityWithUpgrades((int)upgradeValues.GetUpgradeValues(UpgradeType.BetterBaskets));
                }
                break;
            default:
                break;
        }
    }


}
