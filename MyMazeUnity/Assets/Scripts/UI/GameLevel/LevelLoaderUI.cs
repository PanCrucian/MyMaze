using UnityEngine;
using System.Collections;

public class LevelLoaderUI : MonoBehaviour {

    public void LoadMenu()
    {
        StartCoroutine(LoadMenuNumerator());
    }

    IEnumerator LoadMenuNumerator()
    {
        ScreenOverlayUI.Instance.FadeIn();
        yield return new WaitForSeconds(ScreenOverlayUI.Instance.FadeDelay);
        if (GameLevel.Instance.state != GameLevelStates.Game && GameLevel.Instance.state != GameLevelStates.Pause)
            MyMaze.Instance.Save();
        MyMaze.Instance.MenuLoadAction();
    }
}
