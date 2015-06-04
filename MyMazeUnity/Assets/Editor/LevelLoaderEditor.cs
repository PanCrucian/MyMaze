using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SceneLoader))]
[CanEditMultipleObjects]
public class LevelLoaderEditor : Editor
{  

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SceneLoader loader = (SceneLoader)target;
        if (GUILayout.Button("Загрузить выбранный уровень"))
        {
            loader.Load();
        }
    }
}
