using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour, ITutorial {

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
        int startedStepsCount = 0;
        int damagedStepsCount = 0;

        foreach (TutorialStep step in steps)
        {
            if (step.IsStarted && step.IsComplete)
                endedStepsCount++;
            if (step.IsStarted && !step.IsComplete)
                startedStepsCount++;
            if (!step.IsStarted && step.IsComplete)
                damagedStepsCount++;
        }

        if (startedStepsCount == 0)
            Debug.LogWarning("Отсутсвуют запущенные шаги в обучении");
        if(startedStepsCount > 1)
            Debug.LogWarning("Обнаружены шаги в обучении которые стартовали дважды, но так и не закончились");
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
    /// Возвращает обучающий шаг у которого есть пометка (!первая из всего массива) о том что шаг стартовал
    /// </summary>
    /// <returns></returns>
    public TutorialStep GetCurrentStep()
    {
        foreach (TutorialStep step in steps)
            if (step.IsStarted && !step.IsComplete)
                return step;

        Debug.LogWarning("Не найден текущий шаг в обучении");
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
    /// Начать следующий шаг обучения, который занимает место в массиве после GetCurrentStep метода
    /// </summary>
    public void StartNextStep()
    {
        TutorialStep currentStep = GetCurrentStep();
            
        bool hook = false;
        foreach (TutorialStep step in steps)
        {
            if (hook)
            {
                step.Start();
                break;
            }
            if (step.phase == currentStep.phase)
                hook = true;
        }
    }

    /// <summary>
    /// Заканчивает текущий шиг обучения и начинает следующий
    /// </summary>
    public void NextStep()
    {
        TutorialStep step = GetCurrentStep();
        StartNextStep();
        step.Complete();
    }
}
