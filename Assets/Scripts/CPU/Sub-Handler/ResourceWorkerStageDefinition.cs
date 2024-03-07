using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceWorkerStageDefinition
{
    private int workerForFood;
    private int workerForGold;
    private int workerForIron;
    private int workerForStone;
    private int workerForWood;

    public ResourceWorkerStageDefinition(int workerForFood, int workerForGold, int workerForIron, int workerForStone, int workerForWood)
    {
        this.workerForFood = workerForFood;
        this.workerForGold = workerForGold;
        this.workerForIron = workerForIron;
        this.workerForStone = workerForStone;
        this.workerForWood = workerForWood;
    }
    
    public int GetWorkersAmountForFood()
    {
        return workerForFood;
    }

    public int GetWorkersAmountForGold()
    {
        return workerForGold;
    }

    public int GetWorkersAmountForIron()
    {
        return workerForIron;
    }

    public int GetWorkersAmountForStone()
    {
        return workerForStone;
    }

    public int GetWorkersAmountForWood()
    {
        return workerForWood;
    }

}
