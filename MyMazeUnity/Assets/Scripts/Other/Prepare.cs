using UnityEngine;
using System.Collections;
using System;

public class Prepare : MonoBehaviour {

    int counter;

    void Awake()
    {
        Application.targetFrameRate = 60;
        counter = 0;
        /*
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
        if (Screen.currentResolution.width > 1280 && Screen.currentResolution.height > 800 && Screen.dpi > 240f)
            Screen.SetResolution(Convert.ToInt32(Screen.currentResolution.width / 2), Convert.ToInt32(Screen.currentResolution.height / 2), true);
#endif*/
    }

    void Update()
    {
        if (counter > 1)
        {
            Application.LoadLevel("Main");
        }
        counter++;
    }
}
