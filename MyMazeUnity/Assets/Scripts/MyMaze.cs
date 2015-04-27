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
    /// Последний выбранный пак
    /// </summary>
    public Pack LastSelectedPack;

    /// <summary>
    /// Последний выбранный уровень
    /// </summary>
    public Level LastSelectedLevel;

    /// <summary>
    /// Последняя выбранная страница (её номер)
    /// </summary>
    public int LastSelectedPageNumber;

    /// <summary>
    /// Общий счет ходов игрока
    /// </summary>
    [HideInInspector]
    public int MovesCounter;

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

    /// <summary>
    /// Первый запуск приложения?
    /// </summary>
    public bool IsFirstLoad
    {
        get
        {
            return _isFirstLoad;
        }
    }

    private static MyMaze _instance;
    private bool _isFirstLoad = true;

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

        Application.targetFrameRate = 30;
    }

    void Start()
    {
        SetupData();
        CheckNames();
        CheckLastSelectedForNull();
        Load();
    }

    /// <summary>
    /// Проверяем чтобы не было нулевых ссылок на последние выбранные объекты игры
    /// </summary>
    void CheckLastSelectedForNull()
    {
        if (LastSelectedPack == null)
            LastSelectedPack = packs[0];
        if (LastSelectedLevel == null)
            LastSelectedLevel = levels[0];
        //LastSelectedPage не проверяем, он устанавливается в меню
    }

    void Update()
    {
        if (_isFirstLoad)
            _isFirstLoad = false;
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

    /// <summary>
    /// Возвращает следующий пак исходя из аргумента
    /// </summary>
    /// <param name="currentPack">Пак после которого нужно получить следующий пак</param>
    /// <returns></returns>
    public Pack GetNextPack(Pack currentPack)
    {
        bool returnFlag = false;
        foreach (Pack pack in packs)
        {
            if (returnFlag)
                return pack;
            if (pack.packName.Equals(currentPack.packName))
                returnFlag = true;
        }
        return null;
    }

    /// <summary>
    /// Возвращает пак к которому пренадлежит уровень
    /// </summary>
    /// <param name="level">Уровень</param>
    /// <returns></returns>
    public Pack GetPackViaLevel(Level level)
    {
        foreach (Pack pack in packs)
        {
            if (pack.IsYourLevel(level))
                return pack;
        }
        return null;
    }

    /// <summary>
    /// Возвращает пак через имя
    /// </summary>
    /// <param name="packName">имя пака</param>
    /// <returns></returns>
    public Pack GetPackViaName(string packName)
    {
        foreach (Pack pack in packs)
        {
            if (pack.packName.Equals(packName))
                return pack;
        }
        return null;
    }

    /// <summary>
    /// Возвращает уровень через имя
    /// </summary>
    /// <param name="levelName">имя уровня</param>
    /// <returns></returns>
    public Level GetLevelViaName(string levelName)
    {
        foreach (Level level in levels)
        {
            if (level.levelName.Equals(levelName))
                return level;
        }
        return null;
    }

    #region "Методы Сохранения"

    /// <summary>
    /// Сохраняет в PlayerPrefs информацию о текущем состоянии игры
    /// </summary>
    public void Save()
    {
        //уровни
        foreach (Level level in levels)
            level.Save();

        //ссылки на последние объекты
        PlayerPrefs.SetInt("LastSelectedPageNumber", LastSelectedPageNumber);
        PlayerPrefs.SetString("LastSelectedPack", LastSelectedPack.packName);
        PlayerPrefs.SetString("LastSelectedLevel", LastSelectedLevel.levelName);

        //звуки
        this.Sounds.Save();

        //туториал
        this.Tutorial.Save();

        //Сбросим на диск
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Загружает из PlayerPrefs информацию о прошлом состоянии игры
    /// </summary>
    public void Load()
    {
        //уровни
        foreach (Level level in levels)
            level.Load();

        //ссылки на последние объекты
        if (PlayerPrefs.HasKey("LastSelectedPageNumber"))
            LastSelectedPageNumber = PlayerPrefs.GetInt("LastSelectedPageNumber");
        if (PlayerPrefs.HasKey("LastSelectedPack"))
        {
            Pack pack = GetPackViaName(PlayerPrefs.GetString("LastSelectedPack"));
            if (pack != null)
                LastSelectedPack = pack;
        }
        if (PlayerPrefs.HasKey("LastSelectedLevel"))
        {
            Level level = GetLevelViaName(PlayerPrefs.GetString("LastSelectedLevel"));
            if (level != null)
                LastSelectedLevel = level;
        }

        //звуки
        this.Sounds.Load();

        //туториал
        this.Tutorial.Load();
    }

    /// <summary>
    /// Удалить все сохраниения
    /// </summary>
    public void ResetSaves()
    {
        PlayerPrefs.DeleteAll();
    }

    #endregion

    /// <summary>
    /// Когда выходим из приложения
    /// </summary>
    void OnApplicationQuit()
    {
        
    }
}
