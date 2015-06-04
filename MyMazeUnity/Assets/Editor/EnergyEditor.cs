using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

[CustomEditor(typeof(Energy))]
[CanEditMultipleObjects]
public class EnergyEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Energy energy = (Energy)target;

        if (!Application.isPlaying)
            return;

        EditorGUILayout.Separator();

        GUILayout.Label("Доступные энергоблоки: " + energy.Units.ToString());

        GUILayout.Label("Текущее время: " + Timers.Instance.UnixTimestamp.ToString());
        GUILayout.Label("Время следующего: " + energy.GetNextBlockRegenerationTime().ToString());
        GUILayout.Label("Время полного восст-я: " + energy.GetLastBlockRegenerationTime().ToString());

        if (GUILayout.Button("Обновить редактор"))
        {
            //просто заставлет отрисовать кишки заного
        }

        if (GUILayout.Button("Использовать"))
            energy.Use();

        if (GUILayout.Button("Восстановить еденицу"))
            energy.RestoreOneUnit();
    }
}
