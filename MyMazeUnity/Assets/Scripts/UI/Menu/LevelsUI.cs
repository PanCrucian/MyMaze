using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelsUI : MonoBehaviour {

    public LevelUI[] levelsui;
    public Text loadingText;

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
        PageUI pageui = transform.parent.GetComponentInParent<PageUI>();
        if (pageui != MyMaze.Instance.LastSelectedPage)
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
