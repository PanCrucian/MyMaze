using UnityEngine;
using System.Collections;

public class GooglePlayServices : MonoBehaviour {
#if UNITY_ANDROID
    /// <summary>
    /// Подключен к сервисам Google Play?
    /// </summary>
    /// <returns></returns>
    public bool IsAuth
    {
        get
        {
            if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
                return true;
            else
                return false;
        }
    }

    /// <summary>
    /// Загружены ли лидерборды
    /// </summary>
    public bool IsLeaderboardsLoaded
    {
        get
        {
            return _leaderboardsLoaded;
        }
    }
    private bool _leaderboardsLoaded = false;

    /// <summary>
    /// Загружены ли ачивки
    /// </summary>
    public bool IsAchievementsLoaded
    {
        get
        {
            return _achievmentsLoaded;
        }
    }
    private bool _achievmentsLoaded = false;

    void Start()
    {
        GooglePlayConnection.ActionPlayerConnected += OnPlayerConnected;
        GooglePlayConnection.ActionPlayerDisconnected += OnPlayerDisconnected;

        GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;

        GooglePlayManager.ActionLeaderboardsLoaded += OnLeaderBoardsLoaded;
        GooglePlayManager.ActionScoreSubmited += OnScoreSubmited;
        GooglePlayManager.ActionAchievementsLoaded += OnAchievementsLoaded;
        GooglePlayManager.ActionAchievementUpdated += OnAchievementUpdated;

        if (GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
            OnPlayerConnected();
        else
            GooglePlayConnection.Instance.Connect();
    }

    /// <summary>
    /// Игрок отключился от GooglePlay
    /// </summary>
    void OnPlayerDisconnected()
    {
        Debug.Log("Player disconnected from Google Play Services");
    }

    /// <summary>
    /// Игрок подключился к GooglePlay
    /// </summary>
    void OnPlayerConnected()
    {
        Debug.Log("Google Play Player Connected " + GooglePlayManager.Instance.player.name + "(" + GooglePlayManager.Instance.currentAccount + ")");
        LoadLeaderboards();
    }

    /// <summary>
    /// Загружаем лидерборды
    /// </summary>
    void LoadLeaderboards()
    {
        Debug.Log("Loading Leader Boards Data...");
        GooglePlayManager.Instance.LoadLeaderBoards();
    }

    /// <summary>
    /// Загружена информация о лидербордах
    /// </summary>
    /// <param name="result"></param>
    void OnLeaderBoardsLoaded(GooglePlayResult result) 
    {
        if (result.isSuccess)
        {
            Debug.Log("Leaderboard loaded success");
            _leaderboardsLoaded = true;

            GPLeaderBoard gpLeaderBoard = GooglePlayManager.Instance.GetLeaderBoard(MyMaze.Instance.Leaderboards.GetMarketId(LeaderboardTypes.Stars));
            if (gpLeaderBoard != null)
                MyMaze.Instance.Leaderboards.SetStarsLeaderboard();
        }
        else
            Debug.LogWarning("Leader-Boards Loaded error: " + result.message);

        LoadAchievements();
    }

    /// <summary>
    /// Загружаем информацию о ачивках
    /// </summary>
    void LoadAchievements()
    {
        Debug.Log("Loading Achievements Data...");
        GooglePlayManager.Instance.LoadAchievements();
    }

    /// <summary>
    /// Загружена информация о ачивках
    /// </summary>
    /// <param name="result"></param>
    void OnAchievementsLoaded(GooglePlayResult result)
    {
        if (result.isSuccess)
        {
            Debug.Log("Total Achievement: " + GooglePlayManager.Instance.achievements.Count.ToString());
            _achievmentsLoaded = true;
            CheckAchievementCache();
            CheckAchIntegrity();
        }
        else
        {
            Debug.LogWarning("Achievments Loaded error: " + result.message);
        }
    }

    void CheckAchIntegrity()
    {
        foreach (Achievement achievement in MyMaze.Instance.Achievements.elements)
        {
            bool haveCopy = false;
            foreach (string achievementId in GooglePlayManager.Instance.achievements.Keys)
            {
                string achId = MyMaze.Instance.Achievements.GetMarketId(achievement.type);
                if (achId.Equals(achievementId))
                {
                    haveCopy = true;
                    break;
                }
            }
            if (!haveCopy)
            {
                Debug.LogWarning("No copy for achievement pair GooglePlay into MyMaze. Please check GameController->Achievements->gPSMathcing id's AND GooglePLay achievemnts id.xml");
                break;
            }
        }
    }

    /// <summary>
    /// Проверим кэш и отправим в GooglePlay если не совпадает
    /// </summary>
    void CheckAchievementCache()
    {
        foreach (Achievement achievement in MyMaze.Instance.Achievements.elements)
            if (achievement.IsAchieved)
                if (GooglePlayManager.Instance.GetAchievement(MyMaze.Instance.Achievements.GetMarketId(achievement.type)).state != GPAchievementState.STATE_UNLOCKED)
                    achievement.GooglePlayAchieve();
    }

    /// <summary>
    /// Получены сведения о состоянии соединения
    /// </summary>
    /// <param name="result"></param>
    void ActionConnectionResultReceived(GooglePlayConnectionResult result)
    {
        if (!result.IsSuccess)
        {
            Debug.LogWarning("Cnnection to GPS failed with code: " + result.code.ToString());
            return;
        }
        Debug.Log("Connected to GPS success!" + " " + "ConnectionResult:  " + result.code.ToString());
    }

    /// <summary>
    /// Изменилось состояние ачивки
    /// </summary>
    /// <param name="result"></param>
    void OnAchievementUpdated(GP_AchievementResult result)
    {
        Debug.Log("Achievment Updated " + "Id: " + result.achievementId + "\n status: " + result.message);
    }

    /// <summary>
    /// Отправилось информация о рекорде
    /// </summary>
    /// <param name="result"></param>
    void OnScoreSubmited(GooglePlayResult result)
    {
        Debug.Log("Score Submited:  " + result.message);
    }
#endif
}
