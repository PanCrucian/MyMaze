using System;
using UnityEngine;

/// <summary>
/// Звезда
/// </summary>
[Serializable]
public class Star : IStar, IComparable {

    public string starName = "Star";

    /// <summary>
    /// Секретная звезда?
    /// </summary>
    public bool isHidden;

    /// <summary>
    /// Сколько ходов нужно сделать чтобы получить Star
    /// </summary>
    public int movesToGet;

    public bool IsCollected
    {
        get
        {
            return _isCollected;
        }
    }

    private bool _isCollected;

    /// <summary>
    /// Подобрать звезду
    /// </summary>
    public void Collect()
    {
        if (this._isCollected)
            Debug.Log("Монета уже была собрана, но вы снова подбираете её");
        this._isCollected = true;
    }

    /// <summary>
    /// Потерять звезду
    /// </summary>
    public void Lose()
    {
        if (!this._isCollected)
            Debug.Log("Монеты и так нет, но вы пытаетесь потерять её");
        this._isCollected = false;
    }

    public int CompareTo(object obj)
    {
        Star star = (Star) obj;

        if (this.movesToGet > star.movesToGet) return 1;
        if (this.movesToGet < star.movesToGet) return -1;
        
        return 0;
    }
}
