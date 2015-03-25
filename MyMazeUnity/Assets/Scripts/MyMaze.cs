using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MyMaze : MonoBehaviour {

    public static MyMaze Instance 
    { 
        get {
            if (!_instance) {
                Debug.LogError("Не могу найти экземляр класса MyMaze");
                return null;
            }
            return _instance; 
        }
    }

    private static MyMaze _instance;

    public List<Level> levels;

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
        _instance = this;
    }

    void OnDestroy()
    {
        if (_instance)
            _instance = null;
    }
}
