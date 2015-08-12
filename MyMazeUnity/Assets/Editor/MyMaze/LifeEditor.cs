using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Life))]
[CanEditMultipleObjects]
public class LifeEditor : Editor {

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        if (!Application.isPlaying)
            return;
        Life life = (Life)target;
        GUILayout.Label("Количество жизней: " + life.Units.ToString());
        GUILayout.Label("Регенерация в: " + life.GetNextBlockRegenerationTime().ToString());

        if (GUILayout.Button("Восстановить все"))
            life.RestoreAllUnits();

        if (GUILayout.Button("Потратить 1 жизнь"))
            life.Use();

        if (GUILayout.Button("Восстановить 1 жизнь"))
            life.RestoreOneUnit();
    }
}
