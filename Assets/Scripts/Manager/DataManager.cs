using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager Instance { get { return _instance; } }

    private int gameModeData = 0;
    private int difficultyData = 0;

    private int playerColorData = 0;
    private int enemyColorData = 1;

    public string winLoseMessageData = "";

    // Private Constructor to prevent creating instance
    private DataManager() { }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Setter
    public void SetGameModeData(int gameMode) => gameModeData = gameMode;
    public void SetDifficultyData(int difficulty) => difficultyData = difficulty;
    public void SetWinLoseMessage(string message) => winLoseMessageData = message;
    public void SetPlayerColor(int playerColor) => playerColorData = playerColor;
    public void SetEnemyColor(int enemyColor) => enemyColorData = enemyColor;

    // Getter
    public int GetGameModeData() => gameModeData;
    public int GetDifficultyData() => difficultyData;
    public string GetWinLoseMessage() => winLoseMessageData;
    public int GetPlayerColor() => playerColorData;
    public int GetEnemyColor() => enemyColorData;
}
