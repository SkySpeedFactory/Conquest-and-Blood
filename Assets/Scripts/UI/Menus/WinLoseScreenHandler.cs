using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinLoseScreenHandler : MonoBehaviour
{
    [SerializeField] Text winLoseMessageUIElement = null;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        DisplayWinLoseMessage(DataManager.Instance.GetWinLoseMessage());
    }

    public void NavigateToMainMenu() => SceneManager.LoadScene(0);
    private void DisplayWinLoseMessage(string message) => winLoseMessageUIElement.text = message;
}
