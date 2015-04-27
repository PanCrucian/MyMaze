using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;


[CustomEditor(typeof(Tutorial))]
[CanEditMultipleObjects]
public class TutorialEditor : Editor
{

    GUIStyle textStyle01 = new GUIStyle();
    public override void OnInspectorGUI()
    {
        textStyle01.normal.textColor = Color.yellow;
        textStyle01.alignment = TextAnchor.UpperLeft;
        DrawDefaultInspector();
        Tutorial tutorial = (Tutorial)target;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Текущая фаза обучения: ");
        if (tutorial.GetCurrentStep() != null)
            GUILayout.Label(tutorial.GetCurrentStep().phase.ToString("g"), textStyle01);
        else
            GUILayout.Label("Ошибка!", textStyle01);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Cледующий шаг"))
        {
            tutorial.NextStep();
        }

        if (GUILayout.Button("Сбросить"))
        {
            int i = 0;
            foreach (TutorialStep step in tutorial.steps)
            {
                step.ResetStates();
                if (i == 0)
                    step.IsStarted = true;
                i++;
            }
        }

        SetupNames();
        if (!Application.isPlaying)
            EditorUtility.SetDirty(tutorial);
    }

    void SetupNames()
    {
        Tutorial tutorial = (Tutorial)target;
        foreach (TutorialStep step in tutorial.steps)
        {
            if (!step.stepName.Equals(step.phase.ToString("g")))
                step.stepName = step.phase.ToString("g");
        }
    }
}
