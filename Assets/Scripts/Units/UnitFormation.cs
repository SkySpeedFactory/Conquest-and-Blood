using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFormation : MonoBehaviour
{
    private static UnitFormation _instance;
    public static UnitFormation Instance { get{ return _instance; } }

    private List<Vector3> formationPositionList = new List<Vector3>();
    private List<Vector3> formationPositionListCPU = new List<Vector3>();

    private int[] ringPositionCount = { 5, 10, 20, 30, 34 };
    private float[] ringDistance = { 5f, 10f, 15f, 20f, 25f };

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    public List<Vector3> GetFormationPositionList() => formationPositionList;
    public List<Vector3> GetFormationPositionListCPU() => formationPositionListCPU;

    public List<Vector3> GetSpawnPositionList(Vector3 gatherPoint)
    {
        List<Vector3> SpawnPositionList = new List<Vector3>();
        SpawnPositionList.Add(gatherPoint);
        for (int i = 0; i < ringDistance.Length; i++)
        {
            SpawnPositionList.AddRange(GetRingPosition(gatherPoint, ringDistance[i], ringPositionCount[i]));
        }
        return SpawnPositionList;
    }

    public void GetFormationPosition(Vector3 center, GameObject unit)
    {
        if (unit.GetComponent<Unit>().GetUnitFaction() == Faction.Player_1)
        {
            if (unit == UnitSelections.Instance.GetSelectedUnitsList()[0])
            {
                ClearFormationPositionList();
                formationPositionList.Add(center);
                for (int i = 0; i < ringDistance.Length; i++)
                {
                    formationPositionList.AddRange(GetRingPosition(center, ringDistance[i], ringPositionCount[i]));
                }
            }
        }
        else
        {
            formationPositionListCPU.Clear();
            formationPositionListCPU.Add(center);
            for (int i = 0; i < ringDistance.Length; i++)
            {
                formationPositionListCPU.AddRange(GetRingPosition(center, ringDistance[i], ringPositionCount[i]));
            }
        }
    }

    public List<Vector3> GetRingPosition(Vector3 center, float distance, int positionCount)
    {
        List<Vector3> ringPositionList = new List<Vector3>();
        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * (360f / positionCount);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * new Vector3(1, 0);
            Vector3 position = center + direction * distance;
            ringPositionList.Add(position);
        }

        return ringPositionList;
    }

    public void ClearFormationPositionList()
    {
        formationPositionList.Clear();
    }
}
