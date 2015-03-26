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
    public List<Level> levels;

    /// <summary>
    /// Количество звезд в паке
    /// </summary>
    public int StarsCount
    {
        get
        {
            return _starsCount;
        }
    }
    private int _starsCount;

    public int StarsRecived
    {
        get
        {
            return _starsRecived;
        }
    }
    private int _starsRecived;

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
        _starsRecived = 0;
        foreach (Level level in levels)
        {
            foreach (Star star in level.stars)
            {
                if(star.IsCollected)
                    _starsRecived++;
            }
        }
    }

    /// <summary>
    /// Считаем сколько звезд в этом паке и записываем в @var starsCount
    /// </summary>
    void SetMaximumStars()
    {
        _starsCount = 0;
        foreach (Level level in levels)
        {
            _starsCount += level.stars.Count;
        }
    }

    void SetupLevels()
    {
        foreach (Transform t in transform)
        {
            Level level = t.GetComponent<Level>();
            if (!level)
                continue;
            levels.Add(level);
        }
    }
}
