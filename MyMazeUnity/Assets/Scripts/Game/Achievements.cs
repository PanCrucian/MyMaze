using UnityEngine;
using System.Collections;

public class Achievements : MonoBehaviour, ISavingElement {
    [System.Serializable]
    public class GameCenterTypeMatching
    {
        public string achId;
        public AchievementsTypes type;
    }
    public GameCenterTypeMatching[] iOSMatching;
    public Achievement[] elements;

    /// <summary>
    /// Получить ID ачивки для геймцентра
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetGameCenterId(AchievementsTypes type)
    {
        foreach (GameCenterTypeMatching iOSMatch in iOSMatching)
            if (iOSMatch.type == type)
                return iOSMatch.achId;
        return "";
    }

    /// <summary>
    /// Получить достижение
    /// </summary>
    /// <param name="type"></param>
    public void Achieve(AchievementsTypes type)
    {
        foreach (Achievement achievement in elements)
            if (achievement.type == type)
            {
                achievement.Achieve();
                return;
            }
    }

    /// <summary>
    /// Получить ссылку на достижение
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Achievement GetElement(AchievementsTypes type)
    {
        foreach (Achievement achievement in elements)
            if (achievement.type == type)
                return achievement;
        return null;
    }
        
    /// <summary>
    /// Проверяет и получает достижения свзяанные с ходами
    /// </summary>
    public void MovesAchievement(int movesCounter)
    {
        AchievementsTypes god = AchievementsTypes.God;
        if (movesCounter >= 1000 && !GetElement(god).IsAchieved)
            Achieve(god);

        AchievementsTypes master = AchievementsTypes.Master;
        if (movesCounter >= 500 && !GetElement(master).IsAchieved)
            Achieve(master);

        AchievementsTypes experienced = AchievementsTypes.Experienced;
        if (movesCounter >= 100 && !GetElement(experienced).IsAchieved)
            Achieve(experienced);

        AchievementsTypes firstSteps = AchievementsTypes.FirstSteps;
        if (movesCounter >= 20 && !GetElement(firstSteps).IsAchieved)
            Achieve(firstSteps);
    }

    /// <summary>
    /// Проверяет и получает достижение на все собранные секретные звезды
    /// </summary>
    public void HiddenStarsAchievement()
    {
        bool flag = true;
        foreach (Level level in MyMaze.Instance.levels)
        {
            Star star = level.GetHiddenStar();
            if (star != null)
                if (!star.IsCollected)
                {
                    flag = false;
                    break;
                }
        }
        if (flag)
            Achieve(AchievementsTypes.TreasureHunter);
    }

    /// <summary>
    /// Проверяет и получает достижение на все собранные звезды
    /// </summary>
    public void AllStarsAchievement()
    {
        bool flag = true;
        foreach (Level level in MyMaze.Instance.levels)
        {
            System.Collections.Generic.List<Star> stars = level.GetSimpleStars();
            foreach (Star star in stars)
                if (!star.IsCollected)
                {
                    flag = false;
                    break;
                }
            if (!flag)
                break;
        }
        if (flag)
            Achieve(AchievementsTypes.Congratulations);
    }

    /// <summary>
    /// Проверяет и получает достижение за окончание страницы с паками
    /// </summary>
    /// <param name="pack"></param>
    public void PageOpenedAchievement(Pack pack)
    {
        switch (pack.group)
        {
            case PackGroupTypes.Page00:
                if (GetElement(AchievementsTypes.Page00Opened).IsAchieved)
                    return;
                break;
            case PackGroupTypes.Page01:
                if (GetElement(AchievementsTypes.Page01Opened).IsAchieved)
                    return;
                break;
            case PackGroupTypes.Page02:
                if (GetElement(AchievementsTypes.Page02Opened).IsAchieved)
                    return;
                break;
            case PackGroupTypes.Page03:
                if (GetElement(AchievementsTypes.Page03Opened).IsAchieved)
                    return;
                break;
            case PackGroupTypes.Page04:
                if (GetElement(AchievementsTypes.Page04Opened).IsAchieved)
                    return;
                break;
            default:
                return;
        }

        bool flag = true;
        foreach (Pack p in MyMaze.Instance.packs)
            if (p.group == pack.group)
                if (MyMaze.Instance.StarsRecived < p.StarsRequired)
                {
                    flag = false;
                    break;
                }
        if (!flag)
            return;

        switch (pack.group)
        {
            case PackGroupTypes.Page00:
                Achieve(AchievementsTypes.Page00Opened);
                break;
            case PackGroupTypes.Page01:
                Achieve(AchievementsTypes.Page01Opened);
                break;
            case PackGroupTypes.Page02:
                Achieve(AchievementsTypes.Page02Opened);
                break;
            case PackGroupTypes.Page03:
                Achieve(AchievementsTypes.Page03Opened);
                break;
            case PackGroupTypes.Page04:
                Achieve(AchievementsTypes.Page04Opened);
                break;
        }
    }

    public void Save()
    {
        foreach (Achievement achievement in elements)
            achievement.Save();
    }

    public void Load()
    {
        foreach (Achievement achievement in elements)
            achievement.Load();
    }

    public void ResetSaves()
    {
        foreach (Achievement achievement in elements)
            achievement.ResetSaves();
    }
}
