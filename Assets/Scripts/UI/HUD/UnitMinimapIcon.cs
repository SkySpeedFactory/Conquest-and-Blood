using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMinimapIcon : MonoBehaviour
{
    private Unit unitStats;

    
    void Start()
    {
        unitStats = gameObject.GetComponentInParent<Unit>();
        SetUnitMinimapIconColor();
    }

    private void SetUnitMinimapIconColor()
    {
        if (unitStats.GetUnitFaction() == Faction.Player_1)
        {
            gameObject.GetComponent<SpriteRenderer>().color = GameManager.Instance.GetPlayerFactionColor32();
        }
        else if (unitStats.GetUnitFaction() == Faction.CPU)
        {
            gameObject.GetComponent<SpriteRenderer>().color = GameManager.Instance.GetEnemyFactionColor32();
        }
    }
}
