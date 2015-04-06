using UnityEngine;
using System.Collections;

public class LevelLoaderUI : MonoBehaviour {

    public void LoadMenu()
    {
        InputSimulator.Instance.OffAllInput();
        StartCoroutine(LoadMenuNumerator());
    }

    IEnumerator LoadMenuNumerator()
    {
        ScreenOverlayUI.Instance.FadeIn();
        yield return new WaitForSeconds(ScreenOverlayUI.Instance.FadeDelay);
        InputSimulator.Instance.OnAllInput();
        MyMaze.Instance.LevelLoader.LoadMenu();
    }
}
