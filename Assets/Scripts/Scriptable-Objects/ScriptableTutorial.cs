using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Tutorial Point", menuName = "ScriptableObjects/TutorialPoint")]
public class ScriptableTutorial : ScriptableObject
{
    public string TutorialTitle;
    [TextArea(10,100)]
    public string TutorialTextContent;
    public Sprite TutorialPicture;
}
