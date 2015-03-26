using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Игровой уровень
/// </summary>
[Serializable]
public class Level : MonoBehaviour, ILevel, IComparable
{
    /// <summary>
    /// Имя
    /// </summary>
    public string levelName;

    /// <summary>
    /// Отображаемое имя
    /// </summary>
    public string displayText;

    /// <summary>
    /// Список звезд
    /// </summary>
    public List<Star> stars = new List<Star>() { new Star(), new Star(), new Star()};

    /// <summary>
    /// Минимальное количество ходов сделанные на этом уровне
    /// </summary>
    public int MinMovesRecord
    {
        get {
            return this.minMovesRecord;
        }
        set{
            if (value >= 0)
                minMovesRecord = value;
            else
            {
                minMovesRecord = 1;
                Debug.LogWarning("Значение рекорда не может быть отрицательным, установил свое значение");
            }
        }
    }

    /// <summary>
    /// Уровень пройден? true = да false = нет
    /// </summary>
    public bool IsPassed { get { return this.isPassed; } }

    /// <summary>
    /// Уровень закрыт? true = да false = нет
    /// </summary>
    public bool IsClosed { get { return this.isClosed; } }

    private bool isPassed;
    private bool isClosed;
    private int minMovesRecord;
    
    /// <summary>
    /// Открыть уровень
    /// </summary>
    public void Open()
    {
        if (!this.isClosed)
            Debug.Log("Уровень уже был открыт");
        this.isClosed = false;
    }

    /// <summary>
    /// Закрыть уровень
    /// </summary>
    public void Close()
    {
        if (this.isClosed)
            Debug.Log("Уровень уже был закрыт");
        this.isClosed = true;
    }

    /// <summary>
    /// Пройти уровень
    /// </summary>
    public void Pass()
    {
        if (this.isClosed)
        {
            Debug.LogWarning("Вы пытаетесь пройти уровень который закрыт, прохождение не будет засчитано. Вначале откройте уровень!");
            return;
        }
        this.isPassed = true;
    }

    public int CompareTo(object obj)
    {
        Level level = (Level)obj;

        if ((int)this.levelName[0] > (int)level.levelName[0]) return 1;
        if ((int)this.levelName[0] < (int)level.levelName[0]) return -1;

        return 0;
    }

    /// <summary>
    /// Есть ли скрытые звезды в этом уровне
    /// </summary>
    /// <returns></returns>
    public bool HasHiddenStar()
    {
        foreach (Star star in this.stars)
        {
            if (star.isHidden)
                return true;
        }

        return false;
    }


    /// <summary>
    /// Возвращает скрытую звезду
    /// </summary>
    /// <returns></returns>
    public Star GetHiddenStar()
    {
        foreach (Star star in this.stars)
        {
            if (star.isHidden)
                return star;
        }

        return null;
    }

    /// <summary>
    /// Возвращает скрытые звезды
    /// </summary>
    /// <returns></returns>
    public List<Star> GetHiddenStars()
    {
        List<Star> stars = new List<Star>();
        foreach (Star star in this.stars)
        {
            if (star.isHidden)
                stars.Add(star);
        }

        return stars;
    }

    /// <summary>
    /// Возвращает обычные звезды
    /// </summary>
    /// <returns></returns>
    public List<Star> GetSimpleStars()
    {
        List<Star> stars = new List<Star>();
        foreach (Star star in this.stars)
        {
            if (!star.isHidden)
                stars.Add(star);
        }

        return stars;
    }
}
