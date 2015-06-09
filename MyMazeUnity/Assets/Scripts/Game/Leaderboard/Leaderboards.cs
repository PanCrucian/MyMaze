using UnityEngine;
using System.Collections;

public class Leaderboards : MonoBehaviour, ISavingElement {
    [System.Serializable]
    public class GameCenterTypeMatching
    {
        public string boardId;
        public LeaderboardTypes type;
    }
    public GameCenterTypeMatching[] iOSMatching;
    public Leaderboard[] elements;
    
    /// <summary>
    /// Получить ID для геймцентра
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetGameCenterId(LeaderboardTypes type)
    {
        foreach (GameCenterTypeMatching iOSMatch in iOSMatching)
            if (iOSMatch.type == type)
                return iOSMatch.boardId;
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
    }

    #region GameCenter methods and variables
#if UNITY_IPHONE
    /// <summary>
    /// Отправляет рекорды в игровой центр
    /// </summary>
    /// <param name="leaderboard"></param>
    void GameCenterLeaderboard(Leaderboard leaderboard)
    {
        if(MyMaze.Instance.GameCenter.IsAuth)
            GameCenterManager.ReportScore((double)leaderboard.score, GetGameCenterId(leaderboard.type));
    }
#endif
    #endregion

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
