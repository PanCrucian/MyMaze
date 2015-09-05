using UnityEngine;
using System.Collections;
using System;
using DeltaDNA;
using ChartboostSDK;

public class Prepare : MonoBehaviour {

    int counter;

    void Awake()
    {
        Application.targetFrameRate = 60;
        counter = 0;
        
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8) && !UNITY_EDITOR
        if (Screen.currentResolution.width > 1280 && Screen.currentResolution.height > 800 && Screen.dpi > 240f)
            Screen.SetResolution(Convert.ToInt32(Screen.currentResolution.width / 2), Convert.ToInt32(Screen.currentResolution.height / 2), true);
#endif
    }

    void Update()
    {
        if (counter == 1)
        {
            InitDeltaDNA();
            InitAppsFlayer();
            InitHeyzap();
            Application.LoadLevel("WhiteRoomGames");
        }
        counter++;
    }

    /// <summary>
    /// Инициализируем компоненту по сбору данных "DeltaDNA"
    /// </summary>
    void InitDeltaDNA()
    {
        Debug.Log("Try for start: DeltaDNA SDK");
        DDNA.Instance.SetLoggingLevel(Logger.Level.DEBUG);
        DDNA.Instance.ClientVersion = "1.0.0";

        DDNA.Instance.StartSDK(
            "07340666555270785346798059914368",
            "http://collect5099mymzs.deltadna.net/collect/api",
            "http://engage5099mymzs.deltadna.net",
            DDNA.AUTO_GENERATED_USER_ID
            );
    }

    /// <summary>
    /// Инициализируем компоненту рекламы HeyzapADS, которая юзает в свою очередь Chartboost
    /// </summary>
    void InitHeyzap()
    {
        Debug.Log("Try for start: Heyzap SDK");
        HeyzapAds.start("dbd8d8664dc59f959a160d97849baf5c", HeyzapAds.FLAG_NO_OPTIONS);
    }

    /// <summary>
    /// Инициализируем AppsFlayer
    /// </summary>
    void InitAppsFlayer()
    {
        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
            return;

        Debug.Log("Try for start: AppsFlayer");
        AppsFlyer.setAppsFlyerKey("wr4oyoitcFGVerfzDk5Qf9");
#if UNITY_IPHONE
        AppsFlyer.setAppID("1018888691");
        //AppsFlyer.setIsDebug(false);
        AppsFlyer.getConversionData();
        AppsFlyer.trackAppLaunch();
#elif UNITY_ANDROID
		// All Initialization occur in the override activity defined in the mainfest.xml, including track app launch
		// You can define AppsFlyer library here use this commented out code.

		//AppsFlyer.setAppID ("YOUR_ANDROID_PACKAGE_NAME_HERE"); // un-comment this in case you are not working with the manifest file
		//AppsFlyer.setIsSandbox(true);
		//AppsFlyer.setIsDebug (true);
		//AppsFlyer.createValidateInAppListener ("AppsFlyerTrackerCallbacks", "onInAppBillingSuccess", "onInAppBillingFailure");
		//AppsFlyer.loadConversionData("AppsFlyerTrackerCallbacks","didReceiveConversionData", "didReceiveConversionDataWithError");
		//AppsFlyer.trackAppLaunch ();
#endif
    }
}
