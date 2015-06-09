using UnityEngine;
using System.Collections;
#if UNITY_IPHONE
using UnionAssets.FLE;
#endif

public class GameCenter : MonoBehaviour {
    #region GameCenter methods and variables
#if UNITY_IPHONE
    public bool GcAchievementsLoaded
    {
        get
        {
            return _gcAchievementsLoaded;
        }
    }
    private bool _gcAchievementsLoaded = false;
    public bool GcIsAuth
    {
        get
        {
            return _gcIsAuth;
        }
    }
    private bool _gcIsAuth = false;

    void Start()
    {
        foreach (Achievements.GameCenterTypeMatching iOSMatch in MyMaze.Instance.Achievements.iOSMatching)
            GameCenterManager.RegisterAchievement(iOSMatch.achId);

        GameCenterManager.OnAuthFinished += OnAuthFinished;
        GameCenterManager.OnAchievementsLoaded += OnAchievementsLoaded;

        GameCenterManager.Dispatcher.addEventListener(GameCenterManager.GAME_CENTER_ACHIEVEMENT_PROGRESS, OnAchievementProgress);

        GameCenterManager.init();
    }

    /// <summary>
    /// Методы геймцентра теперь можно или нельзя использовать
    /// </summary>
    /// <param name="res">Зависит от результата</param>
    void OnAuthFinished(ISN_Result res)
    {
        _gcIsAuth = res.IsSucceeded;
        if (!_gcIsAuth)
        {
            if (res.error != null)
                Debug.LogWarning("Game Center Auth Error: " + res.error.description);
            else
                Debug.LogWarning("Game Center Auth Error: " + "Unknow");
        }
    }

    /// <summary>
    /// Пришел ответ от геймцентра с ачивками
    /// </summary>
    /// <param name="result">Результаты</param>
    private void OnAchievementsLoaded(ISN_Result result)
    {
        _gcAchievementsLoaded = result.IsSucceeded;
        Debug.Log("OnAchievementsLoaded: " + _gcAchievementsLoaded.ToString());

        if (_gcAchievementsLoaded)
        {   //Проверим кэш и отправим в Game Center если не совпадает
            foreach (Achievement achievement in MyMaze.Instance.Achievements.elements)
                if (achievement.IsAchieved)
                {
                    string achievementGcId = MyMaze.Instance.Achievements.GetGameCenterId(achievement.type);
                    if (GameCenterManager.GetAchievementProgress(achievementGcId) < 100f)
                        achievement.GameCenterAchieve();
                }
        }
    }

    /// <summary>
    /// Прогресс по достижению
    /// </summary>
    /// <param name="e"></param>
    private void OnAchievementProgress(CEvent e)
    {
        ISN_AchievementProgressResult result = (ISN_AchievementProgressResult)e.data;
        if (result.IsSucceeded)
            Debug.Log("OnAchievementProgress: true \n" + result.info.Id + ":  " + result.info.Progress.ToString());
        else
            Debug.Log("OnAchievementProgress: false");
    }
#endif
    #endregion
}
