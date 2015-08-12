using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MyMaze))]
public class Tutorial : MonoBehaviour, ITutorial, ISavingElement
{

    public TutorialStep[] steps;

    void Start()
    {
        CheckTheIntegrity();
    }

    /// <summary>
    /// Проверить целостность информации о обучении
    /// </summary>
    void CheckTheIntegrity()
    {
        int endedStepsCount = 0;
        int damagedStepsCount = 0;

        foreach (TutorialStep step in steps)
        {
            if (step.IsStarted && step.IsComplete)
                endedStepsCount++;
            if (!step.IsStarted && step.IsComplete)
                damagedStepsCount++;
        }

        if(damagedStepsCount > 0)
            Debug.LogWarning("Обнаружены шаги в обучении которые помечены как законченные, но они никогда не стартовали");

        foreach (TutorialStep step in steps)
        {
            int duplicate = 0;
            foreach (TutorialStep step2 in steps)
            {
                if (step.phase == step2.phase)
                    duplicate++;
            }
            if (duplicate > 1) {
                Debug.LogWarning("В обучении дублируется фаза " + step.phase.ToString("g"));
                break;
            }
        }

    }

    /// <summary>
    /// Получить шаг туториала исходя из указанной фазы
    /// </summary>
    /// <param name="phase">Нумератор фазы</param>
    /// <returns></returns>
    public TutorialStep GetStep(TutorialPhase phase)
    {
        foreach (TutorialStep step in steps)
            if (step.phase == phase)
                return step;

        return null;
    }

    /// <summary>
    /// Закончить шаг обучения исходя из указанной фазы
    /// </summary>
    /// <param name="phase">Нумератор фазы</param>
    public void CompleteStep(TutorialPhase phase)
    {
        TutorialStep step = GetStep(phase);
        step.Complete();
    }

    /// <summary>
    /// Начать шаг обучения исходя из указанной фазы
    /// </summary>
    /// <param name="phase">Нумератор фазы</param>
    public void StartStep(TutorialPhase phase)
    {
        TutorialStep step = GetStep(phase);
        step.Start();
    }

    /// <summary>
    /// Сохраним состояние туториала в PlayerPrefs
    /// </summary>
    public void Save()
    {
        foreach (TutorialStep step in steps)
        {
            PlayerPrefs.SetInt("TutorialPhase#" + step.stepName + "#started", System.Convert.ToInt32(step.IsStarted));
            PlayerPrefs.SetInt("TutorialPhase#" + step.stepName + "#complete", System.Convert.ToInt32(step.IsComplete));
        }
    }

    /// <summary>
    /// Загрузим из PlayerPrefs состояние туториала
    /// </summary>
    public void Load()
    {
        foreach (TutorialStep step in steps)
        {
            if (PlayerPrefs.HasKey("TutorialPhase#" + step.stepName + "#started"))
            {
                step.IsStarted = System.Convert.ToBoolean(PlayerPrefs.GetInt("TutorialPhase#" + step.stepName + "#started"));
            }
            if (PlayerPrefs.HasKey("TutorialPhase#" + step.stepName + "#complete"))
                step.IsComplete = System.Convert.ToBoolean(PlayerPrefs.GetInt("TutorialPhase#" + step.stepName + "#complete"));
        }
    }
    
    public void ResetSaves()
    {
        throw new System.NotImplementedException();
    }
}
