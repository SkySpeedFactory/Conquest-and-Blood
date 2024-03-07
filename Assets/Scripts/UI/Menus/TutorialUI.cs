using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] ScriptableTutorial[] scriptableTutorials = null;

    [SerializeField] TMP_Text paginatorTextField = null;
    [SerializeField] TMP_Text tutorialPointTitleTextField = null;
    [SerializeField] TMP_Text tutorialPointContentTextField = null;

    [SerializeField] Image tutorialImage;

    private int tutorialPointCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        ShowTutorialPointContent();
    }

    public void GoToNextTutorialPoint()
    {
        tutorialPointCounter++;
        ShowTutorialPointContent();
    }
    public void GoToPreviousTutorialPoint()
    {
        tutorialPointCounter--;
        if (tutorialPointCounter < 0)
        {
            tutorialPointCounter = scriptableTutorials.Length;
            tutorialPointCounter--;
        }
        ShowTutorialPointContent();
    }

    public void ShowTutorialPointContent()
    {
        int tutorialIndex = tutorialPointCounter % scriptableTutorials.Length;
        ScriptableTutorial currentTutorialPoint = scriptableTutorials[tutorialIndex];
        SetCurrentTutorialPoint(tutorialIndex, currentTutorialPoint);
    }

    public void SetCurrentTutorialPoint(int index, ScriptableTutorial tutorialContent)
    {
        paginatorTextField.SetText($"Page {index + 1} / {scriptableTutorials.Length}");

        tutorialPointTitleTextField.SetText(tutorialContent.TutorialTitle);
        tutorialPointContentTextField.SetText(tutorialContent.TutorialTextContent);
        if (tutorialContent.TutorialPicture != null)
        {
            tutorialImage.sprite = tutorialContent.TutorialPicture;
        }
        
    }
}
