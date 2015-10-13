using UnityEngine;
using System.Collections;

public class RestoreIAPBtn : MonoBehaviour {

	void Start () {
#if UNITY_ANDROID
        gameObject.SetActive(false);
#endif
	}
}
