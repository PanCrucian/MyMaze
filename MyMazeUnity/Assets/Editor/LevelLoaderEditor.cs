using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LevelLoader))]
[CanEditMultipleObjects]
public class LevelLoaderEditor : Editor
{  

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LevelLoader loader = (LevelLoader)target;
        if (GUILayout.Button("Загрузить выбранный уровень"))
        {
            loader.Load();
        }
    }
}
