using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;


[CustomEditor(typeof(Tutorial))]
[CanEditMultipleObjects]
public class TutorialEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        
        DrawDefaultInspector();
        Tutorial tutorial = (Tutorial)target;
        
        if (GUILayout.Button("Cледующий шаг"))
        {
            tutorial.NextStep();
        }

        if (GUILayout.Button("Сбросить"))
        {
            foreach (TutorialStep step in tutorial.steps)
            {
                step.ResetStates();
            }
        }

        CheckTheIntegrity();
    }

    void CheckTheIntegrity()
    {
        Tutorial tutorial = (Tutorial)target;
        int i = 0;
        foreach (TutorialStep step in tutorial.steps)
        {
            if (i == 0)
                step.Start();
            if (!step.name.Equals(step.phase.ToString("g")))
                step.name = step.phase.ToString("g");

            i++;
        }
    }
}
