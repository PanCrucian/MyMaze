using UnityEngine;
using System.Collections;

public class GameLevelDesign : MonoBehaviour {

    /// <summary>
    /// Диапазон значение из события инпута drag delta при котором не учитывается событие drag
    /// </summary>
    public float dragTreshold = 15f;

    /// <summary>
    /// Запрос на движение не чаще времени в этой переменной
    /// </summary>
    public float moveRequestDelayTime = 0.1f;

    /// <summary>
    /// Задержка перед завершением игры
    /// </summary>
    public float gameOverDelay = 0.5f;

    /// <summary>
    /// Задержка между анимациями получения звезд
    /// </summary>
    public float starsCollectingDelay = 0.75f;

    /// <summary>
    /// Задержка перед загрузкой следующего уровня
    /// </summary>
    public float nextLevelDelay = 1.5f;

    [HideInInspector]
    public float lastMoveRequestTime = 0f;

    public static GameLevelDesign Instance
    {
        get
        {
            return _instance;
        }
    }

    private static GameLevelDesign _instance;

    void Awake()
    {
        _instance = this;
    }
}
