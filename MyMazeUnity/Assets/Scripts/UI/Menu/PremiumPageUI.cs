using UnityEngine;
using System.Collections;

public class PremiumPageUI : MonoBehaviour {

    private InApps inApps;

    void Start()
    {
        inApps = MyMaze.Instance.InApps;
    }

	void Update () {
        if (inApps.IsPremium)
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
	}
}
