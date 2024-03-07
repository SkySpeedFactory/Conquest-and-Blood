using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    // Start Resources: 200, 100, 0, 0, 100
    private int food = 200;
    private int gold = 100;
    private int iron = 0;
    private int stone = 0;
    private int wood = 100;

    // Getters
    public int GetResourceFood() => food;
    public int GetResourceGold() => gold;
    public int GetResourceIron() => iron;
    public int GetResourceStone() => stone;
    public int GetResourceWood() => wood;

    // Setters
    public int SetResourceFood(int resourceAmount) => food += resourceAmount;
    public int SetResourceGold(int resourceAmount) => gold += resourceAmount;
    public int SetResourceIron(int resourceAmount) => iron += resourceAmount;
    public int SetResourceStone(int resourceAmount) => stone += resourceAmount;
    public int SetResourceWood(int resourceAmount) => wood += resourceAmount;
}
