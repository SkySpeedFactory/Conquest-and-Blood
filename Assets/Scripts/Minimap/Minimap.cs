using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    private static Minimap _instance;
    public static Minimap Instance { get { return _instance; } }

    [SerializeField] Transform ground = null; // needed to calculate minimap movement
    [SerializeField] Texture2D minimapTexture = null; // needed to auire texture resulution for math
    [SerializeField] RectTransform minimapTransform = null; // the actual ui element that will move when th player walks around
    [SerializeField] Vector3 levelExtend = Vector3.zero; // Extend to upper right corner of the level (from top view)
    [SerializeField] float iconMultiplier = 1f;
    [SerializeField] Image uiObjectPrefab = null;

    Dictionary<MapObject, RectTransform> mapObjects = new Dictionary<MapObject, RectTransform>();

    float pixelsPerUnit = 0f;

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
        pixelsPerUnit = minimapTexture.width / (levelExtend.x * 2);
    }
    
    void FixedUpdate()
    {
        minimapTransform.anchoredPosition = -CalculatePositionOnMap(ground.position);

        foreach (var key in mapObjects.Keys)
        {
            if (key.UpdatePosition)
            {
                mapObjects.TryGetValue(key, out RectTransform rectTransform);
                rectTransform.anchoredPosition = CalculatePositionOnMap(key.transform.position);
                rectTransform.localRotation = Quaternion.Euler(0, 0, -key.transform.rotation.y);
            }
        }

    }

    // Assumes level is centered at 0/0 -> needs Vector3 with center coordiantes otherwise
    public Vector2 CalculatePositionOnMap(Vector3 worldPosition)
    {
        // if not centered at 0/0 -> subtract MapCenter from worldPosition
        return new Vector2(worldPosition.x * pixelsPerUnit * iconMultiplier, worldPosition.z * pixelsPerUnit * iconMultiplier);
    }

    public void AddObjectToMap(MapObject mapObject)
    {
        if (!mapObjects.ContainsKey(mapObject))
        {
            Image image = Instantiate(uiObjectPrefab, minimapTransform);
            mapObjects.Add(mapObject, image.rectTransform);
            // set icon
            image.sprite = mapObject.Icon;
            image.color = mapObject.GetIconColor();
            // set position on map
            image.rectTransform.anchoredPosition = CalculatePositionOnMap(mapObject.transform.position);
        }
    }

    public void RemoveObjectFromMap(MapObject mapObject)
    {
        if (mapObjects != null)
        {
            if (mapObjects.TryGetValue(mapObject, out RectTransform uiElement))
            {
                if(uiElement != null)
                {
                    if (uiElement.gameObject)
                    {
                        Destroy(uiElement.gameObject);
                    }
                }
            }
            mapObjects.Remove(mapObject);
        }
    }

    public void RemoveAllObjectsFromMap()
    {
        mapObjects = null;
    }
}
