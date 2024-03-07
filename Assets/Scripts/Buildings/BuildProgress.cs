using UnityEngine;

public class BuildProgress : MonoBehaviour
{
    private readonly int maxBuildProgress = 100;
    private int buildProgress = 0;
    private bool buildProgressIsFinished = false;

    public void SetBuildProgress(int progress)
    {
        buildProgress += progress;
        if (buildProgress >= maxBuildProgress)
        {
            buildProgressIsFinished = true;
        }
    }

    public void FinishBuildProgress()
    {
        buildProgress = maxBuildProgress;
        buildProgressIsFinished = true;
    }

    public bool CheckIfBuildIsFinished() => buildProgressIsFinished;

    public void ResetBuildProgress()
    {
        buildProgress = 0;
        buildProgressIsFinished = false;
    }

    public int GetBuildProgress() => buildProgress;
}
