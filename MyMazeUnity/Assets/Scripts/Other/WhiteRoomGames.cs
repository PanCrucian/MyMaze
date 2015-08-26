using UnityEngine;
using System.Collections;

public class WhiteRoomGames : MonoBehaviour {

    public float yieldLogo = 2f;

	IEnumerator Start () {
        yield return new WaitForSeconds(yieldLogo);
        Application.LoadLevel("Main");
	}
}
