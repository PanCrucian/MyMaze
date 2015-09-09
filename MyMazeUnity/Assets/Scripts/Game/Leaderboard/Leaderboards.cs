using UnityEngine;
using System.Collections;

public class Leaderboards : MonoBehaviour, ISavingElement {
    [System.Serializable]
    public class IdToTypeMatching
    {
        public string boardId;
        public LeaderboardTypes type;
    }
    public IdToTypeMatching[] iOSMatching;
    public IdToTypeMatching[] gPSMatching;
    public Leaderboard[] elements;
    
    /// <summary>
    /// Получить ID для магазина GameCenter или Google Play Service
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetMarketId(LeaderboardTypes type)
    {
#if UNITY_IPHONE
        foreach (IdToTypeMatching iOSMatch in iOSMatching)
            if (iOSMatch.type == type)
                return iOSMatch.boardId;
#endif
#if UNITY_ANDROID
        foreach (IdToTypeMatching gPSMatch in gPSMatching)
            if (gPSMatch.type == type)
                return gPSMatch.boardId;
#endif
        return "";
    }

    /// <summary>
    /// Получить ссылку на рекорд
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Leaderboard GetElement(LeaderboardTypes type)
    {
        foreach (Leaderboard leaderboard in elements)
            if (leaderboard.type == type)
                return leaderboard;
        return null;
    }

    /// <summary>
    /// Запишем рекорд о звездах для игрока
    /// </summary>
    public void SetStarsLeaderboard()
    {
        if(MyMaze.Instance == null)
            return;
        Leaderboard leaderboard = GetElement(LeaderboardTypes.Stars);
        leaderboard.score = MyMaze.Instance.StarsRecived;
#if UNITY_IPHONE
        GameCenterLeaderboard(leaderboard);
#endif
#if UNITY_ANDROID
        GooglePlayLeaderboard(leaderboard);
#endif
    }

#if UNITY_IPHONE
    /// <summary>
    /// Отправляет рекорды в GameCenter
    /// </summary>
    /// <param name="leaderboard"></param>
    void GameCenterLeaderboard(Leaderboard leaderboard)
    {
        if(MyMaze.Instance.GameCenter.IsAuth)
            GameCenterManager.ReportScore((double)leaderboard.score / 100.00d, GetMarketId(leaderboard.type));
    }
#endif
#if UNITY_ANDROID
    /// <summary>
    /// Отправляет рекорды в Google Play Services
    /// </summary>
    /// <param name="leaderboard"></param>
    void GooglePlayLeaderboard(Leaderboard leaderboard)
    {
        if(MyMaze.Instance.GooglePlayServices.IsAuth && MyMaze.Instance.GooglePlayServices.IsLeaderboardsLoaded)
            GooglePlayManager.Instance.SubmitScoreById(GetMarketId(leaderboard.type), (long)leaderboard.score);
    }
#endif

    public void Save()
    {
        foreach (Leaderboard leaderboard in elements)
            leaderboard.Save();
    }

    public void Load()
    {
        foreach (Leaderboard leaderboard in elements)
            leaderboard.Load();
    }

    public void ResetSaves()
    {
        foreach (Leaderboard leaderboard in elements)
            leaderboard.ResetSaves();
    }
}
