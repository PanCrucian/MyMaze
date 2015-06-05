using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MyMaze))]
[CanEditMultipleObjects]
public class MyMazeEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MyMaze myMaze = (MyMaze)target;
        if (Application.isPlaying)
        {
            EditorGUILayout.Separator();
            if (GUILayout.Button("Сохранить состояние игры"))
            {
                myMaze.Save();
            }
            if (GUILayout.Button("Загрузить последнее сохранение"))
            {
                myMaze.Load();
            }
            if (GUILayout.Button("Сбросить сохранения"))
            {
                myMaze.ResetSaves();
            }
        }
    }
}
