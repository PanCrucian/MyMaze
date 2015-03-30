using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;


[CustomEditor(typeof(Localization))]
[CanEditMultipleObjects]
public class LocalizationEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        
        DrawDefaultInspector();
        Localization localization = (Localization)target;
        
        if (GUILayout.Button("Обновить язык"))
        {
            localization.Setup();
            localization.RefreshTextMeshes();
        }
    }
}
