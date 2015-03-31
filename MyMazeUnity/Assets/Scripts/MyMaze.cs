using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MyMaze : MonoBehaviour
{
    /// <summary>
    /// Сколько всего звезд получено
    /// </summary>
    public int StarsRecived;

    /// <summary>
    /// Сколько всего звезд в игре
    /// </summary>
    public int StarsCount;

    /// <summary>
    /// Последняя выбранная страница
    /// </summary>
    public PageUI LastSelectedPage;

    /// <summary>
    /// Последний выбранный пак
    /// </summary>
    public Pack LastSelectedPack;

    /// <summary>
    /// Ссылки на все паки
    /// </summary>
    public List<Pack> packs;

    /// <summary>
    /// ссылки на все уровни
    /// </summary>
    public List<Level> levels;

    /// <summary>
    /// Звуки и музыка
    /// </summary>
    public Sounds Sounds {
        get
        {
            return GetComponent<Sounds>();
        }   
    }

    public WebLinks WebLinks
    {
        get
        {
            return GetComponent<WebLinks>();
        }
    }

    /// <summary>
    /// Компонента обучения
    /// </summary>
    public Tutorial Tutorial
    {
        get
        {
           return GetComponent<Tutorial>();
        }
    }

    /// <summary>
    /// Компонента переводов текста
    /// </summary>
    public Localization Localization
    {
        get
        {
            return GetComponent<Localization>();
        }
    }

    /// <summary>
    /// Ссылка на загрузчик уровней
    /// </summary>
    public LevelLoader LevelLoader
    {
        get
        {
            return GetComponent<LevelLoader>();
        }
    }

    /// <summary>
    /// Ссылка на геймобъект
    /// </summary>
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
        if (!IsExist()) { 
            //Сохраняем инстанс в каждой сцене
            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);

            SetupApplicationPreferences();
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetupData();
        CheckNames();
        LastSelectedPack = packs[0];
    }

    void Update()
    {
        CalculateTotalStars();
        CalculateStarsRecived();
    }

    /// <summary>
    /// Считаем сколько всего звезд было получено
    /// </summary>
    void CalculateStarsRecived()
    {
        int summ = 0;
        foreach (Pack pack in MyMaze.Instance.packs)
            summ += pack.StarsRecived;
        StarsRecived = summ;
    }

    /// <summary>
    /// Считаем сколько всего звезд в игре
    /// </summary>
    void CalculateTotalStars()
    {
        int summ = 0;
        foreach (Pack pack in MyMaze.Instance.packs)
            summ += pack.StarsCount;
        StarsCount = summ;
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
        
    }

    /// <summary>
    /// Проверяет на копии себя же
    /// </summary>
    bool IsExist()
    {
        InputSimulator[] objects = GameObject.FindObjectsOfType<InputSimulator>();
        if (objects.Length > 1) { 
            return true;
        }
        return false;
    }
}
