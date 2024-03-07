using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] Sprite icon = null;
    [SerializeField] Color32 iconColor = Color.white;
    [SerializeField] bool isNatureObject = true;
    public Sprite Icon { get => icon; }

    [SerializeField] Vector2 offsetOnMap = Vector2.zero;
    public Vector2 OffsetOnMap { get => offsetOnMap; }

    [SerializeField] bool allowRotationOnMap = false;
    public bool AllowRotationOnMap { get => allowRotationOnMap; }

    [SerializeField] bool updatePosition = false;
    public bool UpdatePosition { get => updatePosition; }

    // Size on Map
    // Enum -> states (Active, disabled, new, ...)

    // Start is called before the first frame update
    void Start()
    {
        SetFactionMinimapColor();
        Minimap.Instance.AddObjectToMap(this);
    }

    private void OnDestroy()
    {
        Minimap.Instance.RemoveObjectFromMap(this);
    }

    public Color32 GetIconColor() => iconColor;

    private void SetFactionMinimapColor()
    {
        if (!isNatureObject)
        {
            gameManager = GameManager.Instance;
            if (gameObject.GetComponent<Unit>() != null)
            {
                if (gameObject.GetComponent<Unit>().GetUnitFaction() == Faction.Player_1)
                {
                    iconColor = gameManager.GetPlayerFactionColor32();
                }
                else if (gameObject.GetComponent<Unit>().GetUnitFaction() == Faction.CPU)
                {
                    iconColor = gameManager.GetEnemyFactionColor32();
                }
            }
            else if (gameObject.GetComponent<Building>() != null)
            {
                if (gameObject.GetComponent<Building>().GetBuildingFaction() == Faction.Player_1)
                {
                    iconColor = gameManager.GetPlayerFactionColor32();
                }
                else if (gameObject.GetComponent<Building>().GetBuildingFaction() == Faction.CPU)
                {
                    iconColor = gameManager.GetEnemyFactionColor32();
                }
            }
        }
    }

    public void ActivateIcon()
    {
        SetFactionMinimapColor();
        Minimap.Instance.AddObjectToMap(this);
    }
    public void DeactivateIcon()
    {
        Minimap.Instance.RemoveObjectFromMap(this);
    }

}
