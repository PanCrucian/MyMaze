using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Игровой уровень
/// </summary>
[Serializable]
public class Level : MonoBehaviour, ILevel, IComparable, ISavingElement
{
    public Deligates.LevelEvent OnPassed;
    public Deligates.LevelEvent OnFirstTimePassed;

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
                minMovesRecord = 0;
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

    private int minMovesRecord = 999999;

    /// <summary>
    /// Этот уровень пройден в первый раз?
    /// </summary>
    [HideInInspector]
    public bool IsAllreadyPassed = false;

    /// <summary>
    /// Открыть уровень
    /// </summary>
    public void Open()
    {
        this.IsClosed = false;
    }

    /// <summary>
    /// Закрыть уровень
    /// </summary>
    public void Close()
    {
        this.IsClosed = true;
    }

    /// <summary>
    /// Пройти уровень
    /// </summary>
    public void Pass()
    {
        if (this.IsClosed)
        {
            Debug.LogWarning("Вы пытаетесь пройти уровень " + levelName + " который закрыт, прохождение не будет засчитано. Вначале откройте уровень!");
            return;
        }
        this.IsPassed = true;

        if (OnPassed != null)
            OnPassed(this);

        if (!IsAllreadyPassed)
        {
            IsAllreadyPassed = true;
            if (OnFirstTimePassed != null)
                OnFirstTimePassed(this);
        }
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


    /// <summary>
    /// Сохраняет в PlayerPrefs информацию о текущем состоянии уровня и звезд
    /// </summary>
    public void Save()
    {
        PlayerPrefs.SetInt(levelName + "#IsPassed", Convert.ToInt32(this.IsPassed));
        PlayerPrefs.SetInt(levelName + "#IsClosed", Convert.ToInt32(this.IsClosed));

        for (int i = 0; i < stars.Count; i++)
        {
            Star star = stars[i];
            PlayerPrefs.SetInt(levelName + "#Star" + i.ToString() + "#IsCollected", Convert.ToInt32(star.IsCollected));
        }

        PlayerPrefs.SetInt(levelName + "#minMovesRecord", MinMovesRecord);
        PlayerPrefs.SetInt(levelName + "#IsAllreadyPassed", Convert.ToInt32(this.IsAllreadyPassed));
    }

    /// <summary>
    /// Загружает из PlayerPrefs информацию о прошлом состоянии уровня и звезд
    /// </summary>
    public void Load()
    {
        if (PlayerPrefs.HasKey(levelName + "#IsClosed"))
        {

            if (Convert.ToBoolean(PlayerPrefs.GetInt(levelName + "#IsClosed")))
                Close();
            else
                Open();
        }

        if (PlayerPrefs.HasKey(levelName + "#IsPassed"))
            if (Convert.ToBoolean(PlayerPrefs.GetInt(levelName + "#IsPassed")))
                Pass();

        for (int i = 0; i < stars.Count; i++)
        {
            Star star = stars[i];
            if (PlayerPrefs.HasKey(levelName + "#Star" + i.ToString() + "#IsCollected"))
                if (Convert.ToBoolean(PlayerPrefs.GetInt(levelName + "#Star" + i.ToString() + "#IsCollected")))
                    star.Collect();
        }

        if (PlayerPrefs.HasKey(levelName + "#minMovesRecord"))
            MinMovesRecord = PlayerPrefs.GetInt(levelName + "#minMovesRecord");

        if (PlayerPrefs.HasKey(levelName + "#IsAllreadyPassed"))
            IsAllreadyPassed = Convert.ToBoolean(PlayerPrefs.GetInt(levelName + "#IsAllreadyPassed"));
    }
    
    public void ResetSaves()
    {
        throw new NotImplementedException();
    }
}
