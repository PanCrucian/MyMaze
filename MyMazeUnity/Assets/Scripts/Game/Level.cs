using UnityEngine;
using System.Collections;

/// <summary>
/// Игровой уровень
/// </summary>
[System.Serializable]
public class Level : ILevel {
    /// <summary>
    /// Имя
    /// </summary>
    public string name;

    /// <summary>
    /// Список звезд
    /// </summary>
    public Star[] stars;

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
}
