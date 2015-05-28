using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MainMenuTester))]
[CanEditMultipleObjects]
public class MainMenuEditor : Editor
{
    public CanvasGroup workMenu;
    public int pageNumber = 0;
    public bool isPackWork = true;

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
            {
                pageNumber = EditorGUILayout.IntField("Номер страницы", pageNumber);
                isPackWork = EditorGUILayout.Toggle("Работаем с паками?", isPackWork);
            }
            else
                pageNumber = 0;
        if (GUILayout.Button("Подготовить к работе"))
        {
            mmTester.PrepareForWork(workMenu, pageNumber, isPackWork);
        }
        if (Application.isPlaying)
        {
            EditorGUILayout.Separator();
            if (GUILayout.Button("Сохранить состояние игры"))
            {
                if (MyMaze.Instance == null)
                    Debug.LogWarning("Запустите игру чтобы работать с сохранениями");
                else
                    MyMaze.Instance.Save();
            }
            if (GUILayout.Button("Загрузить последнее сохранение"))
            {
                if (MyMaze.Instance == null)
                    Debug.LogWarning("Запустите игру чтобы работать с сохранениями");
                else
                    MyMaze.Instance.Load();
            }
            if (GUILayout.Button("Сбросить сохранения"))
            {
                if (MyMaze.Instance == null)
                    Debug.LogWarning("Запустите игру чтобы работать с сохранениями");
                else
                    MyMaze.Instance.ResetSaves();
            }
        }
    }
}
