using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Набор уровней
/// </summary>
[System.Serializable]
public class Pack : MonoBehaviour, IPack {
    public Deligates.PackEvent OnPackOpened;
    /// <summary>
    /// Имя набора
    /// </summary>
    public string packName;

    /// <summary>
    /// Массив уровней в паке
    /// </summary>
    public Level[] levels;

    /// <summary>
    /// Количество звезд в паке
    /// </summary>
    public int StarsCount;

    /// <summary>
    /// Получено звезд в паке
    /// </summary>
    public int StarsRecived;

    /// <summary>
    /// Количество звезд которое необходимо набрать для открытия пака
    /// </summary>
    public int StarsRequired;

    public PackGroupTypes group = PackGroupTypes.Custom;

    private float oldUpdateTime = 0f;
    private bool packOpenEventFired = false;

    /// <summary>
    /// Закрыт ли пак? Считает все приобретенные звезды, сравнивает с необходимым количеством и говорит да или нет
    /// </summary>
    public bool IsClosed
    {
        get
        {
            if (MyMaze.Instance.StarsRecived >= StarsRequired)
                return false;

            return true;
        }
    }

    void Awake()
    {
        SetupLevels();
        foreach (Level level in levels)
            foreach (Star star in level.stars)
                star.OnCollected += OnStarCollected;
    }

    void Start()
    {
        SetMaximumStars();
        oldUpdateTime = Time.time;
    }

    /// <summary>
    /// Получена звезда
    /// </summary>
    /// <param name="star"></param>
    void OnStarCollected(Star star)
    {
        StarsRecived++;
        if (MyMaze.Instance.StarsRecived >= StarsRequired)
            if (!packOpenEventFired)
                PackOpened();
    }

    void Update()
    {
        if (Mathf.Abs(oldUpdateTime - Time.time) >= 0.5f)
        {            
            oldUpdateTime = Time.time;
        }
    }

    void PackOpened()
    {
        if(group != PackGroupTypes.Page00)
            MyMaze.Instance.Achievements.PageOpenedAchievement(this);

        if (OnPackOpened != null)
            OnPackOpened(this);
        packOpenEventFired = true;
    }

    /// <summary>
    /// Считаем сколько звезд в этом паке и записываем в @var starsCount
    /// </summary>
    void SetMaximumStars()
    {
        StarsCount = 0;
        foreach (Level level in levels)
        {
            StarsCount += level.stars.Count;
        }
    }

    void SetupLevels()
    {
        levels = GetComponentsInChildren<Level>();
    }

    /// <summary>
    /// проверяет принадлежит ли уровень к данному паку
    /// </summary>
    /// <param name="level">объект уровень</param>
    /// <returns></returns>
    public bool IsYourLevel(Level level)
    {
        foreach (Level lvl in levels)
            if (lvl.Equals(level))
                return true;

        return false;
    }

    /// <summary>
    /// Возвращает следующий уровень исходя из аргумента
    /// </summary>
    /// <param name="currentLevel">Уровень после которого нужно получить следующий уровень</param>
    /// <returns></returns>
    public Level GetNextLevel(Level currentLevel)
    {
        bool returnFlag = false;
        foreach (Level level in levels)
        {
            if(returnFlag)
                return level;
            if(level.levelName.Equals(currentLevel.levelName))
                returnFlag = true;
        }
        return null;
    }
}
