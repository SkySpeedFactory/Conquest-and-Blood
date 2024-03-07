using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private DataManager dataManager;
    private CPUManager cpuManager;
    private CPUUnitHandler cpuUnitHandler;
    private UnitSelections unitSelections;
    private PlaceFoundation placeFoundation;

    // private int gameDifficulty = 1; // Default value

    private int maxPopulation = 100;

    private Color32 playerFactionColor;
    private Color32 enemyFactionColor;

    // Private Constructor to prevent creating instance
    private GameManager() { }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            playerFactionColor = ConvertValueToColor(DataManager.Instance.GetPlayerColor());
            enemyFactionColor = ConvertValueToColor(DataManager.Instance.GetEnemyColor());
        }
    }
    void Start()
    {
        dataManager = DataManager.Instance;
        cpuManager = CPUManager.Instance;
        cpuUnitHandler = CPUUnitHandler.Instance;
        unitSelections = UnitSelections.Instance;
        placeFoundation = PlaceFoundation.Instance;
        
    }

    public int GetMaxPopulation() => maxPopulation;

    public void CheckConquestWinConditions()
    {
        if (dataManager.GetGameModeData() == 0 && DataManager.Instance.GetWinLoseMessage() == "")
        {
            if (cpuUnitHandler.GetCPUMilitaryUnitList().Count + cpuUnitHandler.GetCPUVillagerUnitList().Count <= 0 && cpuManager.AllEnemyBuildingsDestroyed())
            {
                DataManager.Instance.SetWinLoseMessage("Victory! You managed to kill all Enemy Units!");
                SceneManager.LoadScene(2);
            }
            else if (unitSelections.GetUnitList().Count + RemoveFoundationFromList(placeFoundation.GetInstBuildingsList()).Count <= 0)
            {
                DataManager.Instance.SetWinLoseMessage("Defeated! You lost your Empire!");
                Minimap.Instance.RemoveAllObjectsFromMap();
                SceneManager.LoadScene(2);
            }
        }
    }

    private List<GameObject> RemoveFoundationFromList(List<GameObject> list)
    {
        List<GameObject> newList = new List<GameObject>();
        foreach (var item in list)
        {
            if (item != null)
            {
                if (item.transform.Find("BuildingPrefab").gameObject != null)
                {
                    if (item.transform.Find("BuildingPrefab").gameObject.activeSelf == true)
                    {
                        newList.Add(item);
                    }
                }
            }
        }
        return newList;
    }

    public void CheckKingslayerWinConditions(GameObject defeatedArchMage)
    {
        if (dataManager.GetGameModeData() == 1) {
            string winLoseText;
            if (defeatedArchMage.GetComponent<Unit>().GetUnitFaction() == Faction.CPU)
            {
                winLoseText = "Victory! You managed to kill the Enemies Arch-Mage!";
            }
            else
            {
                winLoseText = "Defeated! You are dead!";
            }
            DataManager.Instance.SetWinLoseMessage(winLoseText);
            Minimap.Instance.RemoveAllObjectsFromMap();
            SceneManager.LoadScene(2);
        }
    }

    public void CPUSurrender()
    {
        DataManager.Instance.SetWinLoseMessage("Victory! The Enemy has surrendered!");
        Minimap.Instance.RemoveAllObjectsFromMap();
        SceneManager.LoadScene(2);
    }

    private Color32 ConvertValueToColor(int colorIndex)
    {
        Color32 factionColor;
        switch (colorIndex)
        {
            case 0:
                factionColor = new Color32(0, 0, 255, 255);
                break;
            case 1:
                factionColor = new Color32(255, 0, 0, 255);
                break;
            case 2:
                factionColor = new Color32(0, 255, 0, 255);
                break;
            case 3:
                factionColor = new Color32(255, 0, 255, 255);
                break;
            default:
                factionColor = new Color32(255, 0, 0, 255);
                break;
        }

        return factionColor;
    }

    public Color32 GetPlayerFactionColor32() => playerFactionColor;
    public Color32 GetEnemyFactionColor32() => enemyFactionColor;

    // Add player color

    // Add cpu color

    // Add BGM (Sound)

    // Add SFX (Sound)
}
