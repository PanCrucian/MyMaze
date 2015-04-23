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
    public bool IsHidden;

    /// <summary>
    /// Сколько ходов нужно сделать чтобы получить Star
    /// </summary>
    public int movesToGet;

    /// <summary>
    /// Подобрана?
    /// </summary>
    public bool IsCollected;
    
    /// <summary>
    /// Подобрать звезду
    /// </summary>
    public void Collect()
    {
        if (this.IsCollected)
            Debug.Log("Монета уже была собрана, но вы снова подбираете её");
        this.IsCollected = true;
    }

    /// <summary>
    /// Потерять звезду
    /// </summary>
    public void Lose()
    {
        if (!this.IsCollected)
            Debug.Log("Монеты и так нет, но вы пытаетесь потерять её");
        this.IsCollected = false;
    }

    public int CompareTo(object obj)
    {
        Star star = (Star) obj;

        if (this.movesToGet > star.movesToGet) return 1;
        if (this.movesToGet < star.movesToGet) return -1;
        
        return 0;
    }

    public void SetHidden()
    {
        this.IsHidden = true;
    }    
}
