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
        MyMaze.Instance.LevelLoader.LoadMenu();
    }
}
