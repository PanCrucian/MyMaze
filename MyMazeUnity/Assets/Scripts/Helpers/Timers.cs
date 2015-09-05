using UnityEngine;
using System.Collections;
using System;

public class Timers : MonoBehaviour {

    /// <summary>
    /// Ссылка на геймобъект
    /// </summary>
    public static Timers Instance
    {
        get
        {
            return _instance;
        }
    }

    private static Timers _instance;

    [HideInInspector]
    public int unixTimeOffset = 0;

    void Awake()
    {
        if (!IsExist())
        {
            //Сохраняем инстанс в каждой сцене
            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Проверяет на копии себя же
    /// </summary>
    bool IsExist()
    {
        Timers[] objects = GameObject.FindObjectsOfType<Timers>();
        if (objects.Length > 1)
        {
            return true;
        }
        return false;
    }

    public int UnixTimestamp
    {
        get
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds + unixTimeOffset;
        }
    }
}
