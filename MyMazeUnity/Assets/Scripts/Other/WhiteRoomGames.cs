using UnityEngine;
using System.Collections;

public class WhiteRoomGames : MonoBehaviour {

    public GameObject[] destroyableObjects;

    void Awake()
    {
        if(Application.isPlaying)
            DontDestroyOnLoad(gameObject);
    }

	IEnumerator Start () {
        AsyncOperation async = Application.LoadLevelAsync("Main");
        yield return async;
        ScreenOverlayUI.Instance.FadeIn();
        yield return new WaitForSeconds(ScreenOverlayUI.Instance.FadeDelay);
        for (int i = 0; i < destroyableObjects.Length; i++)
            Destroy(destroyableObjects[i]);
	}
}
