using UnityEngine;

[System.Serializable]
public class Leaderboard : ISavingElement {
    public string name;
    public LeaderboardTypes type;
    public int score;

    public void Save()
    {
        PlayerPrefs.SetInt("Leaderboard" + "#" + type.ToString("g"), score);
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("Leaderboard" + "#" + type.ToString("g")))
            score = PlayerPrefs.GetInt("Leaderboard" + "#" + type.ToString("g"));
    }

    public void ResetSaves()
    {
        PlayerPrefs.DeleteKey("Leaderboard" + "#" + type.ToString("g"));
    }
}
