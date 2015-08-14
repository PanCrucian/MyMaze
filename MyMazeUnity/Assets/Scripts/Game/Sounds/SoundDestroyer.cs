using UnityEngine;
using System.Collections;

public class SoundDestroyer : MonoBehaviour {

	IEnumerator Start () {
        AudioSource audio = GetComponent<AudioSource>();
        yield return new WaitForSeconds(audio.clip.length);
        Destroy(gameObject);
	}
}
