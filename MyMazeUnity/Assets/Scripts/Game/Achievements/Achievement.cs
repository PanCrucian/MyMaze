using UnityEngine;

[System.Serializable]
public class Achievement : ISavingElement {
    public string name;
    public AchievementsTypes type;
    public bool IsAchieved
    {
        get
        {
            return _isAchieved;
        }
    }
    private bool _isAchieved;

    /// <summary>
    /// Получить
    /// </summary>
    public void Achieve()
    {
        if (_isAchieved)
        {
            Debug.Log("Достижение " + name + " уже было получено");
            return;
        }
        _isAchieved = true;
        Debug.Log("Получено достижение " + name);
#if UNITY_IPHONE
        GameCenterAchieve();
#endif
    }

    #region GameCenter methods and variables    
#if UNITY_IPHONE
    /// <summary>
    /// Ачивка в игровой центр
    /// </summary>
    public void GameCenterAchieve()
    {
        GameCenter gameCenter = MyMaze.Instance.GameCenter;
        if (gameCenter.IsAuth && gameCenter.IsAchievementsLoaded)
            GameCenterManager.SubmitAchievement(100f, MyMaze.Instance.Achievements.GetGameCenterId(type));
    }
#endif
    #endregion

    /// <summary>
    /// Убрать ачивку
    /// </summary>
    public void Lost()
    {
        _isAchieved = false;
    }

    public void Save()
    {
        PlayerPrefs.SetInt("Achievement" + "#" + type.ToString("g"), System.Convert.ToInt32(_isAchieved));
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("Achievement" + "#" + type.ToString("g")))
            _isAchieved = System.Convert.ToBoolean(PlayerPrefs.GetInt("Achievement" + "#" + type.ToString("g")));
    }

    public void ResetSaves()
    {
        PlayerPrefs.DeleteKey("Achievement" + "#" + type.ToString("g"));
    }
}
