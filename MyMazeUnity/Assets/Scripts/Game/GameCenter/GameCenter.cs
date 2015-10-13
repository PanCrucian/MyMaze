using UnityEngine;
using System.Collections;

public class GameCenter : MonoBehaviour
{
#if UNITY_IPHONE
    /// <summary>
    /// Авторизовался?
    /// </summary>
    public bool IsAuth
    {
        get
        {
            return _isAuth;
        }
    }

    /// <summary>
    /// Ачивки загружены?
    /// </summary>
    public bool IsAchievementsLoaded
    {
        get
        {
            return _isAchievementsLoaded;
        }
    }

    private bool _isAuth = false;
    private bool _isAchievementsLoaded = false;

    public static string IDFA
    {
        get
        {
            return idfa;
        }
    }
    private static string idfa = "";

    void Start()
    {
        foreach (Achievements.IdToTypeMatching iOSMatch in MyMaze.Instance.Achievements.iOSMatching)
            GameCenterManager.RegisterAchievement(iOSMatch.achId);

        GameCenterManager.OnAuthFinished += OnAuthFinished;
        GameCenterManager.OnAchievementsLoaded += OnAchievementsLoaded;
        GameCenterManager.OnPlayerScoreLoaded += OnPlayerScoreLoaded;
        
        GameCenterManager.OnAchievementsProgress += OnAchievementProgress;

        GameCenterManager.Init();

        IOSSharedApplication.OnAdvertisingIdentifierLoadedAction += OnAdvertisingIdentifierLoadedAction;
        IOSSharedApplication.Instance.GetAdvertisingIdentifier();
    }

    void OnAdvertisingIdentifierLoadedAction (string Identifier) {
        Debug.Log("OnAdvertisingIdentifierLoadedAction Loaded: " + Identifier);
        idfa = Identifier;
    }

    /// <summary>
    /// Методы геймцентра теперь можно или нельзя использовать
    /// </summary>
    /// <param name="res">Зависит от результата</param>
    void OnAuthFinished(ISN_Result res)
    {
        _isAuth = res.IsSucceeded;
        if (!_isAuth)
        {
            if (res.Error != null)
                Debug.LogWarning("Game Center Auth Error: " + res.Error.Description);
            else
                Debug.LogWarning("Game Center Auth Error: " + "Unknow");
        }
        else
        {
            Debug.Log("Player Authed" + "\n" + "ID: " + GameCenterManager.Player.Id + "\n" + "Alias: " + GameCenterManager.Player.Alias);
            GameCenterManager.LoadCurrentPlayerScore(MyMaze.Instance.Leaderboards.GetMarketId(LeaderboardTypes.Stars));
        }
    }

    /// <summary>
    /// Загружена информация из геймцентра о ачивках
    /// </summary>
    /// <param name="result">Результаты</param>
    private void OnAchievementsLoaded(ISN_Result result)
    {
        _isAchievementsLoaded = result.IsSucceeded;

        if (_isAchievementsLoaded)
        {
            Debug.Log("OnAchievementsLoaded: " + _isAchievementsLoaded.ToString());
            CheckAchievementCache();
        }
        else
        {
            Debug.LogWarning("OnAchievementsLoaded: " + _isAchievementsLoaded.ToString());
        }
    }

    /// <summary>
    /// Проверим кэш и отправим в Game Center если не совпадает
    /// </summary>
    void CheckAchievementCache()
    {
        foreach (Achievement achievement in MyMaze.Instance.Achievements.elements)
            if (achievement.IsAchieved)
            {
                string achievementGcId = MyMaze.Instance.Achievements.GetMarketId(achievement.type);
                if (GameCenterManager.GetAchievementProgress(achievementGcId) < 100f)
                    achievement.GameCenterAchieve();
            }
    }

    /// <summary>
    /// Прогресс по достижению
    /// </summary>
    /// <param name="e"></param>
    private void OnAchievementProgress(GK_AchievementProgressResult result)
    {
        if (result.IsSucceeded)
            Debug.Log("OnAchievementProgress: true \n" + result.info.Id + ":  " + result.info.Progress.ToString());
        else
            Debug.LogWarning("OnAchievementProgress: false");
    }

    /*/// <summary>
    /// Загружена информация из геймцентра о рекордах
    /// </summary>
    /// <param name="e"></param>
    private void OnLeaderboardScoreLoaded(CEvent e)
    {
        ISN_PlayerScoreLoadedResult result = (ISN_PlayerScoreLoadedResult) e.data;
        if (result.IsSucceeded)
            Debug.Log("Leaderboard " + result.loadedScore.leaderboardId + "\n" + "Score: " + result.loadedScore.score + "\n" + "Rank:" + result.loadedScore.rank);
        else
            Debug.LogWarning("OnLeaderboardScoreLoaded false");
    }*/

    /// <summary>
    /// Загружена информация о рекорде игрока
    /// </summary>
    /// <param name="result"></param>
    private void OnPlayerScoreLoaded(GK_PlayerScoreLoadedResult result)
    {
        if (result.IsSucceeded)
        {
            Debug.Log("Leaderboard " + result.loadedScore.leaderboardId + "\n" + "Score: " + result.loadedScore.score + "\n" + "Rank:" + result.loadedScore.rank);
            //if (result.loadedScore.leaderboardId.Equals(MyMaze.Instance.Leaderboards.GetMarketId(LeaderboardTypes.Stars)))
                //MyMaze.Instance.Leaderboards.SetStarsLeaderboard();
        }
        else
            Debug.LogWarning("OnPlayerScoreLoaded false");
    }
#endif
}
