using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Набор уровней
/// </summary>
[System.Serializable]
public class Pack : MonoBehaviour, IPack {
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
    }

    void Start()
    {
        SetMaximumStars();
    }

    void Update()
    {
        SetRecivedStars();
    }

    /// <summary>
    /// Считаем сколько звезд уже получено
    /// </summary>
    void SetRecivedStars()
    {
        StarsRecived = 0;
        foreach (Level level in levels)
        {
            foreach (Star star in level.stars)
            {
                if(star.IsCollected)
                    StarsRecived++;
            }
        }
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
}
