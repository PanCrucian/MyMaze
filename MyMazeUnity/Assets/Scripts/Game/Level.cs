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
    public List<Star> stars;

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
    public bool IsPassed;

    /// <summary>
    /// Уровень закрыт? true = да false = нет
    /// </summary>
    public bool IsClosed;

    private int minMovesRecord;
    
    /// <summary>
    /// Открыть уровень
    /// </summary>
    public void Open()
    {
        if (!this.IsClosed)
            Debug.Log("Уровень уже был открыт");
        this.IsClosed = false;
    }

    /// <summary>
    /// Закрыть уровень
    /// </summary>
    public void Close()
    {
        if (this.IsClosed)
            Debug.Log("Уровень уже был закрыт");
        this.IsClosed = true;
    }

    /// <summary>
    /// Пройти уровень
    /// </summary>
    public void Pass()
    {
        if (this.IsClosed)
        {
            Debug.LogWarning("Вы пытаетесь пройти уровень который закрыт, прохождение не будет засчитано. Вначале откройте уровень!");
            return;
        }
        this.IsPassed = true;
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
            if (star.IsHidden)
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
            if (star.IsHidden)
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
            if (star.IsHidden)
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
        List<Star> ss = new List<Star>();
        foreach (Star star in this.stars)
        {
            if (!star.IsHidden)
                ss.Add(star);
        }

        return ss;
    }
}
