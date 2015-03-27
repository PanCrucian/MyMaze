using UnityEngine;
using System.Collections;

public class LevelsUI : MonoBehaviour {

    public LevelUI[] levelsui;

    void Start()
    {
        levelsui = GetComponentsInChildren<LevelUI>();        
    }

    void Update()
    {
        Pack pack = MyMaze.Instance.LastSelectedPack;
        if (pack == null)
        {
            Debug.LogWarning("Потерялась переменная с последним выбранным паком");
            return;
        }
        int i = 0;
        foreach (Level level in pack.levels)
        {
            LevelUI levelui = levelsui[i];
            if (levelui == null)
                continue;
            if (!levelui.level.levelName.Equals(level.levelName))
                levelui.level = level;
            i++;
        }
    }
}
