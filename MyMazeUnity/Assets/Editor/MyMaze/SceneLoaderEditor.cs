using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SceneLoader))]
[CanEditMultipleObjects]
public class SceneLoaderEditor : Editor
{  

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SceneLoader loader = (SceneLoader)target;

        if (!Application.isPlaying)
            return;
        if (GUILayout.Button("Загрузить выбранный уровень"))
        {
            loader.Load();
        }
    }
}
