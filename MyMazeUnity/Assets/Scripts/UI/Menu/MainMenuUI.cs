using UnityEngine;
using System.Collections;
using Heyzap;

[RequireComponent(typeof(Animator))]
public class MainMenuUI : MonoBehaviour {

    public LevelsMenuUI levelsMenuUi;

	void Start () {
        StartCoroutine(FadeOutNumerator());
        if (!MyMaze.Instance.IsFirstSceneLoad)
        {
            CGSwitcher.Instance.SetHideObject(GetComponent<Animator>());
            CGSwitcher.Instance.SetShowObject(levelsMenuUi.GetComponent<Animator>());
            CGSwitcher.Instance.Switch();
        }
        GetComponent<SoundsPlayer>().PlayLooped();
        MyMaze.Instance.OnLevelLoad += OnLevelLoad;
        HeyzapAds.ShowMediationTestSuite();
	}

    IEnumerator FadeOutNumerator()
    {
        yield return new WaitForSeconds(0.1f);
        if (ScreenOverlayUI.Instance != null)
            ScreenOverlayUI.Instance.FadeOut();
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
