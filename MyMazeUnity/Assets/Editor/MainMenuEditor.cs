using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MainMenuTester))]
public class MainMenuEditor : Editor
{
    public CanvasGroup workMenu;
    public int pageNumber = 0; 

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MainMenuTester mmTester = (MainMenuTester)target;

        EditorGUILayout.LabelField("Параметры для плейтеста:");
        if (GUILayout.Button("Подготовить к игре"))
        {
            mmTester.PrepareForPlay();
        }

        EditorGUILayout.LabelField("Параметры для работы:");
        if (!workMenu)
            workMenu = (CanvasGroup) EditorGUILayout.ObjectField("Меню", workMenu, typeof(CanvasGroup), true);
        if (workMenu)
            if (workMenu.name.Equals("cg_Levels"))
                pageNumber = EditorGUILayout.IntField("Номер страницы", pageNumber);
            else
                pageNumber = 0;
        if (GUILayout.Button("Подготовить к работе"))
        {
            mmTester.PrepareForWork(workMenu, pageNumber);
        }
    }
}
