using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUResourceManager : MonoBehaviour
{
    private static CPUResourceManager _instance;
    public static CPUResourceManager Instance { get { return _instance; } }

    // Start Resources: 200, 100, 0, 0, 100
    private int food = 200;
    private int gold = 100;
    private int iron = 0;
    private int stone = 0;
    private int wood = 100;

    // Private Constructor to prevent creating instance
    private CPUResourceManager() { }

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

    public void DebugGetCurrentAmountOfAllResources() => print($"Food: {food}, Gold: {gold}, Iron: {iron}, Stone: {stone}, Wood: {wood}");


    // Getters
    public int GetResourceFood() => food;
    public int GetResourceGold() => gold;
    public int GetResourceIron() => iron;
    public int GetResourceStone() => stone;
    public int GetResourceWood() => wood;

    // Setters
    private int SetResourceFood(int resourceAmount) => food += resourceAmount;
    private int SetResourceGold(int resourceAmount) => gold += resourceAmount;
    private int SetResourceIron(int resourceAmount) => iron += resourceAmount;
    private int SetResourceStone(int resourceAmount) => stone += resourceAmount;
    private int SetResourceWood(int resourceAmount) => wood += resourceAmount;
    public void SetCPUResources(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                SetResourceFood(amount);
                break;
            case ResourceType.Gold:
                SetResourceGold(amount);
                break;
            case ResourceType.Iron:
                SetResourceIron(amount);
                break;
            case ResourceType.Stone:
                SetResourceStone(amount);
                break;
            case ResourceType.Wood:
                SetResourceWood(amount);
                break;
        }
    }
}
