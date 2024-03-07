using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NewGameMenuHandler : MonoBehaviour
{
    [SerializeField] TMP_Text modeText;
    [SerializeField] TMP_Dropdown playerColorDropdown;
    [SerializeField] TMP_Dropdown enemyColorDropdown;

    [SerializeField] TMP_Dropdown mapDropdown;
    [SerializeField] TMP_Dropdown modeDropdown;
    [SerializeField] TMP_Dropdown diffDropdown;

    [SerializeField] Sprite[] mapPreviews;
    [SerializeField] Image currentMapPreview;

    private int mapIndex = 0;

    // Start is called before the first frame update
    void Start()
    {

        //Naming of Maps Dropdown
        #region Maps Dropdown
        mapDropdown.ClearOptions();
        List<string> mapOptions = new List<string>();
        mapOptions.Add(Maps.Forest.ToString());
        mapOptions.Add(Maps.Fields.ToString());
        mapOptions.Add(Maps.SnowValley.ToString());
        mapDropdown.AddOptions(mapOptions);
        #endregion

        //Naming of Modes Dropdown
        #region Mode Dropdown
        modeDropdown.ClearOptions();
        List<string> modeOptions = new List<string>();
        modeOptions.Add(Modes.Conquest.ToString());
        modeOptions.Add(Modes.Kingslayer.ToString());
        modeDropdown.AddOptions(modeOptions);
        modeDropdown.value = DataManager.Instance.GetGameModeData();
        #endregion

        //Naming of the Difficulty Dropdown
        #region Difficulty Dropdown 
        diffDropdown.ClearOptions();

        List<string> diffOptions = new List<string>();
        diffOptions.Add(Difficulty.Easy.ToString());
        diffOptions.Add(Difficulty.Medium.ToString());
        diffOptions.Add(Difficulty.Hard.ToString());
        diffDropdown.AddOptions(diffOptions);
        diffDropdown.value = DataManager.Instance.GetDifficultyData();
        #endregion


        List<string> colorOptions = new List<string>();
        colorOptions.Add("Blue");
        colorOptions.Add("Red");
        colorOptions.Add("Green");
        colorOptions.Add("Magenta");
        playerColorDropdown.AddOptions(colorOptions);
        playerColorDropdown.value = DataManager.Instance.GetPlayerColor();
        enemyColorDropdown.AddOptions(colorOptions);
        enemyColorDropdown.value = DataManager.Instance.GetEnemyColor();
        //Assigned on Value changed Event Maps
        playerColorDropdown.onValueChanged.AddListener(delegate { PlayerDropdownValueChangedHappened(playerColorDropdown); });
        //Assigned on Value changed Event Maps
        enemyColorDropdown.onValueChanged.AddListener(delegate { EnemyDropdownValueChangedHappened(enemyColorDropdown); });

        //Assigned on Value changed Event Maps
        mapDropdown.onValueChanged.AddListener(delegate { MapDropdownValueChangedHappened(mapDropdown); });
        //Assigned on Value changed Event Modes
        modeDropdown.onValueChanged.AddListener(delegate { ModeDropdownValueChangedHappened(modeDropdown); });
        //Assigned on Value changed Event Difficulty
        diffDropdown.onValueChanged.AddListener(delegate { DiffDropdownValueChangeHappened(diffDropdown); });

        //Start of TextUI: shows the default mode
        UpdateModeTextUI();

        DataManager.Instance.SetWinLoseMessage("");

        currentMapPreview.sprite = mapPreviews[0];
    }


    private void PlayerDropdownValueChangedHappened(TMP_Dropdown playercolor)
    {
        DataManager.Instance.SetPlayerColor(playercolor.value);
    }

    private void EnemyDropdownValueChangedHappened(TMP_Dropdown enemyColor)
    {
        DataManager.Instance.SetEnemyColor(enemyColor.value);
    }

    public void MapDropdownValueChangedHappened(TMP_Dropdown map)
    {
        currentMapPreview.sprite = mapPreviews[map.value];
        mapIndex = map.value;
    }

    public void ModeDropdownValueChangedHappened(TMP_Dropdown mode)
    {
        DataManager.Instance.SetGameModeData(mode.value);
        UpdateModeTextUI();
    }

    public void DiffDropdownValueChangeHappened(TMP_Dropdown difficulty)
    {
        DataManager.Instance.SetDifficultyData(difficulty.value);
    }

    public void UpdateModeTextUI()
    {
        // if gamemode was chosen
        if (modeDropdown.value == 0)
        {
            modeText.text = modeDropdown.value.ToString("Conquest");
        }
        else if (modeDropdown.value == 1)
        {
            modeText.text = modeDropdown.value.ToString("Kingslayer");
        }
    }

    private Color32 ChangeFactionNameTextColor(int colorIndex)
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

    // Change scene functions

    public void PlayGame() 
    {
        SceneManager.LoadScene(mapIndex + 3);
    } 
    public void NavigateToMainMenu() => SceneManager.LoadScene(0);
}
