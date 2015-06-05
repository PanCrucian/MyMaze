using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class MainMenuUI : MonoBehaviour {

    public LevelsMenuUI levelsMenuUi;

	void Start () {
        if (!MyMaze.Instance.IsFirstLoad)
        {
            CGSwitcher.Instance.SetHideObject(GetComponent<Animator>());
            CGSwitcher.Instance.SetShowObject(levelsMenuUi.GetComponent<Animator>());
            CGSwitcher.Instance.Switch();
        }
        GetComponent<SoundsPlayer>().PlayLooped();
        MyMaze.Instance.OnLevelLoad += OnLevelLoad;
	}

    /// <summary>
    /// Временная фича, сбрасывает сохранения
    /// </summary>
    public void ResetSaves()
    {
        MyMaze.Instance.ResetSaves();
    }

    void OnLevelLoad(Level level)
    {
        GetComponent<SoundsPlayer>().StopSound();
    }

    void OnDestroy()
    {
        if (MyMaze.Instance != null)
            MyMaze.Instance.OnLevelLoad -= OnLevelLoad;
    }
}
