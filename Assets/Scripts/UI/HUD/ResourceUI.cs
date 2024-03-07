using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ResourceUI : MonoBehaviour
{
    private PlayerManager playerManager;
    private GameManager gameManager;

    // Text Fields
    [SerializeField] TMP_Text unitPopulationText;
    [SerializeField] TMP_Text foodText;
    [SerializeField] TMP_Text goldText;
    [SerializeField] TMP_Text ironText;
    [SerializeField] TMP_Text stoneText;
    [SerializeField] TMP_Text woodText;

    void Start()
    {
        playerManager = PlayerManager.Instance;
        gameManager = GameManager.Instance;

        foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
        {
            UpdateResourceUI(resource);
        }
    }
    public void UpdateResourceUI(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                foodText.text = $"{playerManager.GetPlayerResources(resourceType):N0}";
                break;
            case ResourceType.Gold:
                goldText.text = $"{playerManager.GetPlayerResources(resourceType):N0}";
                break;
            case ResourceType.Iron:
                ironText.text = $"{playerManager.GetPlayerResources(resourceType):N0}";
                break;
            case ResourceType.Stone:
                stoneText.text = $"{playerManager.GetPlayerResources(resourceType):N0}";
                break;
            case ResourceType.Wood:
                woodText.text = $"{playerManager.GetPlayerResources(resourceType):N0}";
                break;
        }
    }
    public void UpdatePopulation(int value)
    {
        if (gameManager != null)
        {
            unitPopulationText.text = $"{value}/{gameManager.GetMaxPopulation()}";
        }
    }
    
}
