using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MyMaze : MonoBehaviour, ISavingElement
{
    public Deligates.LevelEvent OnLevelLoad;
    public Deligates.SimpleEvent OnMenuLoad;

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
            return _sounds;
        }   
    }
    private Sounds _sounds;

    public WebLinks WebLinks
    {
        get
        {
            return _webLinks;
        }
    }
    private WebLinks _webLinks;

    /// <summary>
    /// Компонента обучения
    /// </summary>
    public Tutorial Tutorial
    {
        get
        {
            return _tutorial;
        }
    }
    private Tutorial _tutorial;

    /// <summary>
    /// Компонента переводов текста
    /// </summary>
    public Localization Localization
    {
        get
        {
            return _localization;
        }
    }
    private Localization _localization;

    /// <summary>
    /// Ссылка на загрузчик сцен
    /// </summary>
    private SceneLoader SceneLoader
    {
        get
        {
            return _sceneLoader;
        }
    }
    private SceneLoader _sceneLoader;

    /// <summary>
    /// Ссылка на контроллер энергии
    /// </summary>
    public Energy Energy
    {
        get
        {
            return _energy;
        }
    }
    private Energy _energy;

    /// <summary>
    /// Ссылка на контроллер внутриигровых платежей
    /// </summary>
    public InApps InApps
    {
        get
        {
            return _inApps;
        }
    }
    private InApps _inApps;

    /// <summary>
    /// Ссылка на бустер "Машина времени"
    /// </summary>
    public TimeMachineBooster TimeMachineBooster
    {
        get
        {
            return _timeMachineBooster;
        }
    }
    private TimeMachineBooster _timeMachineBooster;

    /// <summary>
    /// Ссылка на бустер "Телепорт"
    /// </summary>
    public TeleportBooster TeleportBooster
    {
        get
        {
            return _teleportBooster;
        }
    }
    private TeleportBooster _teleportBooster;

    /// <summary>
    /// Ссылка на достижения
    /// </summary>
    public Achievements Achievements
    {
        get
        {
            return _achievements;
        }
    }
    private Achievements _achievements;

    public Leaderboards Leaderboards
    {
        get
        {
            return _leaderboards;
        }
    }
    private Leaderboards _leaderboards;

#if UNITY_IPHONE
    /// <summary>
    /// Ссылка на Геймцентр
    /// </summary>
    public GameCenter GameCenter
    {
        get
        {
            return _gameCenter;
        }
    }
    private GameCenter _gameCenter;
#endif

    /// <summary>
    /// Ссылка на геймобъект
    /// </summary>
    public static MyMaze Instance 
    { 
        get {
            return _instance; 
        }
    }

    /// <summary>
    /// Первый запуск сцены?
    /// </summary>
    public bool IsFirstSceneLoad
    {
        get
        {
            return _isFirstSceneLoad;
        }
    }

    private static MyMaze _instance;
    private bool _isFirstSceneLoad = true;

    /// <summary>
    /// Общий счет ходов игрока
    /// </summary>
    private int _movesCounter;

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

    IEnumerator Start()
    {
        SetupData();
        CheckNames();
        CheckLastSelectedForNull();
        Load();
        yield return new WaitForEndOfFrame();
        CalculateTotalStars();
        //CalculateStarsRecived();
    }

    /// <summary>
    /// Подобрали звезду
    /// </summary>
    /// <param name="star"></param>
    void OnStarCollected(Star star)
    {
        StarsRecived++;
        if (star.IsHidden)
            Achievements.HiddenStarsAchievement();
        Achievements.AllStarsAchievement();
        Leaderboards.SetStarsLeaderboard();
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
        foreach (Level level in levels)
            foreach (Star star in level.stars)
                star.OnCollected += OnStarCollected;
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
        _sounds = GetComponent<Sounds>();
        _webLinks = GetComponent<WebLinks>();
        _tutorial = GetComponent<Tutorial>();
        _localization = GetComponent<Localization>();
        _sceneLoader = GetComponent<SceneLoader>();
        _energy = GetComponent<Energy>();
        _inApps = GetComponent<InApps>();
        _timeMachineBooster = GetComponent<TimeMachineBooster>();
        _teleportBooster = GetComponent<TeleportBooster>();
        _achievements = GetComponent<Achievements>();
        _leaderboards = GetComponent<Leaderboards>();
#if UNITY_IPHONE
        _gameCenter = GetComponent<GameCenter>();
#endif

    }

    /// <summary>
    /// Проверяет на копии себя же
    /// </summary>
    bool IsExist()
    {
        MyMaze[] objects = GameObject.FindObjectsOfType<MyMaze>();
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

    /// <summary>
    /// Игрок загружает игровой уровень
    /// </summary>
    /// <param name="level">Какой уровень загружается</param>
    public void LevelLoadAction(Level level)
    {
        if (OnLevelLoad != null)
            OnLevelLoad(level);
        _isFirstSceneLoad = false;
        SceneLoader.sceneName = level.name;
        SceneLoader.Load();
    }

    public void MenuLoadAction()
    {
        if (OnMenuLoad != null)
            OnMenuLoad();
        SceneLoader.LoadMenu();
    }

    public void GoToMarket()
    {
        Debug.Log("Идем в магазин приложений");
    }

    /// <summary>
    /// Увеличим счетчик ходов игрока
    /// </summary>
    public void IncrementMovesCounter()
    {
        _movesCounter++;
        Achievements.MovesAchievement(_movesCounter);      
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

        //счетчик ходов игрока
        PlayerPrefs.SetInt("MovesCounter", _movesCounter);

        //звуки
        this.Sounds.Save();

        //туториал
        this.Tutorial.Save();

        //Энергия
        this.Energy.Save();

        //Покупки
        this.InApps.Save();

        //Бустер "Машина времени"
        this.TimeMachineBooster.Save();

        //Бустер "Телепорт"
        this.TeleportBooster.Save();

        //Локализация
        this.Localization.Save();

        //Достижения
        this.Achievements.Save();

        //Рекорды
        this.Leaderboards.Save();

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

        //счетчик ходов игрока
        if (PlayerPrefs.HasKey("MovesCounter"))
            _movesCounter = PlayerPrefs.GetInt("MovesCounter");

        //звуки
        this.Sounds.Load();

        //туториал
        this.Tutorial.Load();

        //Энергия
        this.Energy.Load();

        //Покупки
        this.InApps.Load();

        //Бустер "Машина времени"
        this.TimeMachineBooster.Load();

        //Бустер "Телепорт"
        this.TeleportBooster.Load();

        //Локализация
        this.Localization.Load();

        //Достижения
        this.Achievements.Load();

        //Рекорды
        this.Leaderboards.Load();
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
        //TODO Обязательно добавить сюда сохраниние всего прогресса
#if !UNITY_EDITOR
        Save();
#endif
    }
}
