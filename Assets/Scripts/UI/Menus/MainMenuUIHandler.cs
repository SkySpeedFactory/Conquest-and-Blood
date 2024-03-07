using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : MonoBehaviour
{
    #region General Panels
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] GameObject helpPanel;
    #endregion
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] GameObject tutorialBackground;
    [SerializeField] GameObject settingsBackground;

    Resolution[] resolutions;

    private float animationTime = 0.5f;

    // Private Constructor to prevent creating instance
    private MainMenuUIHandler() { }
    private void Awake()
    {
        resolutions = Screen.resolutions;
    }
    void Start()
    {
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
    }

    public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = isFullscreen;
    public void SetVolumeMaster(float volume) => audioMixer.SetFloat("Volume", volume);
    public void SetVolumeEffects(float volume) => audioMixer.SetFloat("Effects", volume);
    public void SetQuality(int qualityIndex) => QualitySettings.SetQualityLevel(qualityIndex);
    public void StartNewPartie() => SceneManager.LoadScene(1);
    public void PressButtonSound(int index) => SoundManager.Instance.PlaySound(index);
    public void QuitGame() => Application.Quit();
    public void MainMenu() => SceneManager.LoadScene(0);

    #region Panel Management
    public void SettingsPanelOn() 
    {
        settingsPanel.SetActive(true);
        LeanTween.moveLocalX(settingsBackground, 0f, animationTime);

    }

    public void SettingsPanelOff() 
    {
        LeanTween.moveLocalX(settingsBackground, -((RectTransform)settingsBackground.transform).rect.width, animationTime).setOnComplete(() => tutorialPanel.SetActive(false));
    }

    public void TutorialPanelOn() 
    {
        tutorialPanel.SetActive(true);
        LeanTween.moveLocalX(tutorialBackground, 0f, animationTime);
    }

    public void TutorialPanelOff() 
    {
        LeanTween.moveLocalX(tutorialBackground, -((RectTransform)tutorialBackground.transform).rect.width, animationTime).setOnComplete(() => tutorialPanel.SetActive(false));
    } 
    #endregion
}
