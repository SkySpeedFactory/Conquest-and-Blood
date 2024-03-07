using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "ScriptableObjects/Resource")]
public class ScriptableResource : ScriptableObject
{
    public ResourceType TypeOfResource;
    public Sprite ResourceSprite;
    public string ResourceName;
    public int ResourceAmount;
    public int ResourceMalus;
    public float RegenerationRate;

    public bool InfinityResource;
    public int WorkerCapacity;
}
