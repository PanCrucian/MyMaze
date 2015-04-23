using UnityEngine;
using System.Collections;

[System.Serializable]
public class TutorialStep {

    public string stepName;
    public TutorialPhase phase;
    public bool IsComplete;
    public bool IsStarted;

    public void Complete()
    {
        if (phase == TutorialPhase.NotTeach)
        {
            Debug.Log("Обучение завершено");
            return;
        }
        if (!IsStarted)
        {
            Debug.LogWarning("Вы пытаетесь закончить шаг с фазой " + phase.ToString("g") + ", но он еще не стратовал");
            return;
        }
        IsComplete = true;
    }

    public void Start()
    {
        IsStarted = true;
    }

    public void ResetStates()
    {
        IsStarted = false;
        IsComplete = false;
    }
}
