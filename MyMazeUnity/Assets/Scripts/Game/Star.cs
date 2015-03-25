using System;
using UnityEngine;

/// <summary>
/// Звезда
/// </summary>
public class Star : IStar, IComparable {

    /// <summary>
    /// Секретная звезда?
    /// </summary>
    public bool isHidden;

    /// <summary>
    /// Сколько ходов нужно сделать чтобы получить Star
    /// </summary>
    public int movesToGet;

    private bool isCollected;

    /// <summary>
    /// Подобрать звезду
    /// </summary>
    public void Collect()
    {
        if (this.isCollected)
            Debug.Log("Монета уже была собрана, но вы снова подбираете её");
        this.isCollected = true;
    }

    /// <summary>
    /// Потерять звезду
    /// </summary>
    public void Lose()
    {
        if (!this.isCollected)
            Debug.Log("Монеты и так нет, но вы пытаетесь потерять её");
        this.isCollected = false;
    }

    public int CompareTo(object obj)
    {
        Star star = (Star) obj;

        if (this.movesToGet > star.movesToGet) return 1;
        if (this.movesToGet < star.movesToGet) return -1;

        return 0;
    }
}
