using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MyMaze : MonoBehaviour {
    public List<Pack> packs;
    public List<Level> levels;

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

    void Awake()
    {
        //Сохраняем инстанс в каждой сцене
        if (Application.isPlaying)
            DontDestroyOnLoad(gameObject);

        SetupApplicationPreferences();
    }

    void Start()
    {
        SetupData();
        CheckNames();
    }

    /// <summary>
    /// Устанавилваем ссылки на элементы игры
    /// </summary>
    void SetupData()
    {
        SetupPacks();
        SetupLevels();
    }

    /// <summary>
    /// Устанавливаем ссылки на паки
    /// </summary>
    void SetupPacks()
    {
        foreach (Transform t in transform)
        {
            Pack pack = t.GetComponent<Pack>();
            if (pack == null)
                continue;
            packs.Add(pack);
        }
    }

    /// <summary>
    /// Устанавливаем ссылки на уровни
    /// </summary>
    void SetupLevels()
    {
        if (packs == null)
        {
            Debug.LogError("Не могу установить уровни, паки не найдены");
            return;
        }
        foreach (Pack p in packs)
        {
            foreach (Level l in p.levels)
            {
                levels.Add(l);
            }
        }
    }

    /// <summary>
    /// Проверям дубликаты имен
    /// </summary>
    void CheckNames()
    {
        if (!CheckPackNames())
        {
            Debug.LogError("Дублирование имен для паков." +
                "\n" + "Проверьте названия паков и уберите дубликаты");
        }

        if (!CheckLevelNames())
        {
            Debug.LogError("Дублирование имен для уровней." +
                "\n" + "Проверьте названия паков и уберите дубликаты");
        }
    }

    /// <summary>
    /// Проверяем на уникальные имена уровней
    /// </summary>
    /// <returns></returns>
    bool CheckLevelNames()
    {
        string uniqname = "";
        foreach (Level level in levels)
        {
            uniqname = level.levelName;
            foreach (Level l in levels)
            {
                if (l == level)
                    continue;
                if (l.levelName.Equals(uniqname))
                {
                    Debug.LogWarning("Не уникальное имя уровня: " + uniqname);
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Проверяем на уникальные имена паков
    /// </summary>
    /// <returns></returns>
    bool CheckPackNames()
    {
        string uniqname = "";
        foreach (Pack pack in packs)
        {
            uniqname = pack.packName;
            foreach (Pack p in packs) {
                if (p == pack)
                    continue;
                if (p.packName.Equals(uniqname))
                {
                    Debug.LogWarning("Не уникальное имя пака: " + uniqname);
                    return false;
                }
            }        
        }
        return true;
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
