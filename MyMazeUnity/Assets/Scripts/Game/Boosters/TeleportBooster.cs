using UnityEngine;
using System.Collections;

public class TeleportBooster : Booster, ISavingElement {

    /// <summary>
    /// Интервал в секундах между использованиями
    /// </summary>
    public int usingInterval = 3600;

    /// <summary>
    /// Unix Timestamp последнего использования
    /// </summary>
    private int _lastUsingTime;
    public int LastUsingTime
    {
        get
        {
            return _lastUsingTime;
        }
    }

    public Level unlimitedUseLevel;

    /// <summary>
    /// Используем
    /// </summary>
    public void Use()
    {
        if (!IsAvaliable())
            Debug.LogWarning("Бустер телепорт недоступен для использования, но вы всеравно используете его");

        if (unlimitedUseLevel != MyMaze.Instance.LastSelectedLevel)
            _lastUsingTime = Timers.Instance.UnixTimestamp;
    }

    /// <summary>
    /// Доступен для использования?
    /// </summary>
    /// <returns></returns>
    public bool IsAvaliable()
    {
        if (unlimitedUseLevel == MyMaze.Instance.LastSelectedLevel)
            return true;
        if (Mathf.Abs(Timers.Instance.UnixTimestamp - _lastUsingTime) >= usingInterval)
            return true;

        return false;
    }

    public void Reset()
    {
        _lastUsingTime = 0;
    }
    
    public override void Open()
    {
        base.Open();
        MyMaze.Instance.Tutorial.GetStep(TutorialPhase.Teleport).Start();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("Teleport#LastUsing", _lastUsingTime);
        PlayerPrefs.SetInt("Teleport#IsClosed", System.Convert.ToInt32(IsClosed));
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("Teleport#LastUsing"))
            _lastUsingTime = PlayerPrefs.GetInt("Teleport#LastUsing");
        if (PlayerPrefs.HasKey("Teleport#IsClosed"))
            if (!System.Convert.ToBoolean(PlayerPrefs.GetInt("Teleport#IsClosed")))
                base.Open();
    }

    public void ResetSaves()
    {
        throw new System.NotImplementedException();
    }
}
