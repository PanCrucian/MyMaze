﻿using UnityEngine;
using System.Collections;
using System;
using DeltaDNA;
using Heyzap;
using UnityEngine.iOS;

public class Prepare : MonoBehaviour {

    int counter;
    string ddnakey = "07355525153794960673570812114368";
    public string clientVersion = "1.0.9";

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
#if UNITY_IPHONE
        if (PlayerPrefs.HasKey("pushNotifKey"))
            DDNA.Instance.PushNotificationToken = PlayerPrefs.GetString("pushNotifKey");

        IOSNotificationController.OnDeviceTokenReceived += (IOSNotificationDeviceToken t) =>
        {
            Debug.Log("Got an iOS push token: " + t.tokenString);
            DDNA.Instance.PushNotificationToken = t.tokenString;
            PlayerPrefs.SetString("pushNotifKey", DDNA.Instance.PushNotificationToken);
        };
        IOSNotificationController.Instance.RegisterForRemoteNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);

        Debug.Log("iOS push token: " + DDNA.Instance.AndroidRegistrationID);
#endif
#if UNITY_ANDROID
        if (PlayerPrefs.HasKey("AndroidRegistrationID"))
            DDNA.Instance.AndroidRegistrationID = PlayerPrefs.GetString("AndroidRegistrationID");

        GoogleCloudMessageService.ActionCMDRegistrationResult += (GP_GCM_RegistrationResult res) =>
        {
            Debug.Log("Got an Android CMD Reg ID: " + GoogleCloudMessageService.Instance.registrationId);
            DDNA.Instance.AndroidRegistrationID = GoogleCloudMessageService.Instance.registrationId;
            PlayerPrefs.SetString("AndroidRegistrationID", DDNA.Instance.AndroidRegistrationID);
        };
        GoogleCloudMessageService.Instance.RgisterDevice();

        Debug.Log("AndroidRegistrationID: " + DDNA.Instance.AndroidRegistrationID);
#endif
        Debug.Log("Try for start: DeltaDNA SDK");
        DDNA.Instance.SetLoggingLevel(Logger.Level.INFO);
        DDNA.Instance.ClientVersion = clientVersion;

        DDNA.Instance.StartSDK(
            ddnakey,
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
		HeyzapAds.Start("dbd8d8664dc59f959a160d97849baf5c", HeyzapAds.FLAG_DISABLE_AUTOMATIC_FETCHING);
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
		// You can define AppsFlyer library here use this commented out code

        AppsFlyer.setAppID("com.thumbspire.mymaze"); // un-comment this in case you are not working with the manifest file
		//AppsFlyer.setIsSandbox(true);
		//AppsFlyer.setIsDebug (true);
		//AppsFlyer.createValidateInAppListener ("AppsFlyerTrackerCallbacks", "onInAppBillingSuccess", "onInAppBillingFailure");
        AppsFlyer.loadConversionData("AppsFlyerTrackerCallbacks", "didReceiveConversionData", "didReceiveConversionDataWithError");
		AppsFlyer.trackAppLaunch ();
#endif
    }
}
