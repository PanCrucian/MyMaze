using UnityEngine;
using System.Collections;

public class Ads : MonoBehaviour {

    public Deligates.SimpleEvent OnLifeWindowAds;

    public void CallLifeWindowAds()
    {
        if (OnLifeWindowAds != null)
            OnLifeWindowAds();
    }
}
