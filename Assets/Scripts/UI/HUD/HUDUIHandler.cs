using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDUIHandler : MonoBehaviour
{
    private static HUDUIHandler _instance;
    public static HUDUIHandler Instance { get { return _instance; } }

    [SerializeField] GameObject quitPanel;
    [SerializeField] Text interactText;

    // Private Constructor to prevent creating instance
    private HUDUIHandler() { }

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
    }
    public void OpenQuitPanel() => quitPanel.SetActive(true);
    public void LeaveCurrentGame() 
    {
        Minimap.Instance.RemoveAllObjectsFromMap();
        SceneManager.LoadScene(0);
    } 
    public void Cancel() => quitPanel.SetActive(false);
    public Text GetInteractText() => interactText;
}
