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
        Pack pack = Game.Instance.LastSelectedPack;
        if (pack == null)
        {
            Debug.LogWarning("Потерялась переменная с последним выбранным паком");
            return;
        }
        PageUI pageui = transform.parent.GetComponentInParent<PageUI>();
        if (pageui != Game.Instance.LastSelectedPage)
            return;
        int i = 0;
        foreach (Level level in pack.levels)
        {
            LevelUI levelui = levelsui[i];
            if (levelui == null)
                continue;
            if (levelui.level == null)
                levelui.level = level;
            else if (!levelui.level.levelName.Equals(level.name))
                levelui.level = level;
            i++;
        }
    }
}
