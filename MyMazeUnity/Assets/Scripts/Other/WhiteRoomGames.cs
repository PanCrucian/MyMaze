using UnityEngine;
using System.Collections;

public class WhiteRoomGames : MonoBehaviour {

    public float yieldLogo = 0.5f;

	IEnumerator Start () {
        yield return new WaitForSeconds(yieldLogo);
        ScreenOverlayUI.Instance.FadeIn();
        yield return new WaitForSeconds(ScreenOverlayUI.Instance.FadeDelay);
        Application.LoadLevel("Main");
	}
}
