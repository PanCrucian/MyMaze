using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class MainMenuUI : MonoBehaviour {

	void Start () {
        if (!MyMaze.Instance.IsFirstLoad)
        {
            CGSwitcher.Instance.SetHideObject(GetComponent<Animator>());
            LevelsMenuUI levelsMenuUi = GameObject.FindObjectOfType<LevelsMenuUI>();
            CGSwitcher.Instance.SetShowObject(levelsMenuUi.GetComponent<Animator>());
            CGSwitcher.Instance.Switch();
        }
	}

    /// <summary>
    /// Временная фича, сбрасывает сохранения
    /// </summary>
    public void ResetSaves()
    {
        MyMaze.Instance.ResetSaves();
    }
}
