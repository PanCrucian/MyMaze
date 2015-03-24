using UnityEngine;
using System.Collections;

public class MyMaze : MonoBehaviour {

    void Awake()
    {
        //Сохраняем инстанс в каждой сцене
        if (Application.isPlaying)
            DontDestroyOnLoad(gameObject);

        SetupApplicationPreferences();
    }

    /// <summary>
    /// Устанавливаем параметры приложения
    /// </summary>
    void SetupApplicationPreferences()
    {
        //Application.targetFrameRate = 60;
    }
}
