﻿using UnityEngine;
using UnityEditor;
using System.Collections;

public class PluginsInstalationUtil  {


	public const string ANDROID_SOURCE_PATH       = "Plugins/StansAssets/Android/";
	public const string ANDROID_DESTANATION_PATH  = "Plugins/Android/";


	public const string IOS_SOURCE_PATH       = "Plugins/StansAssets/IOS/";
	public const string IOS_DESTANATION_PATH  = "Plugins/IOS/";





	public static void IOS_UpdatePlugin() {
		IOS_InstallPlugin(false);
	}

	public static void IOS_InstallPlugin(bool IsFirstInstall = true) {
		#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1	|| UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		FileStaticAPI.CopyFolder(IOS_SOURCE_PATH, IOS_DESTANATION_PATH);
		#else
		Unity5_IOS_CleanUp();
		#endif

		IOSCleanUP();
	}


	public static void IOSCleanUP() {
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.IOS_DESTANATION_PATH + "GADAdMobExtras.h");
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.IOS_DESTANATION_PATH + "GADAdNetworkExtras.h");
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.IOS_DESTANATION_PATH + "GADAdSize.h");
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.IOS_DESTANATION_PATH + "GADBannerViewDelegate.h");
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.IOS_DESTANATION_PATH + "GADInAppPurchase.h");
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.IOS_DESTANATION_PATH + "GADInAppPurchaseDelegate.h");
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.IOS_DESTANATION_PATH + "GADInterstitialDelegate.h");
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.IOS_DESTANATION_PATH + "GADModules.h");
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.IOS_DESTANATION_PATH + "GADRequest.h");
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.IOS_DESTANATION_PATH + "GADRequestError.h");
		FileStaticAPI.DeleteFile(PluginsInstalationUtil.IOS_DESTANATION_PATH + "libGoogleAdMobAds.a");

	}

	public static void Unity5_IOS_CleanUp() {
		RemoveIOSFile("AppEventListener");
		RemoveIOSFile("CloudManager");
		RemoveIOSFile("CustomBannerView");
		RemoveIOSFile("GameCenterManager");
		RemoveIOSFile("GCHelper");
		RemoveIOSFile("iAdBannerController");
		RemoveIOSFile("iAdBannerObject");
		RemoveIOSFile("InAppPurchaseManager");
		RemoveIOSFile("IOSGameCenterManager");
		RemoveIOSFile("IOSNativeNotificationCenter");
		RemoveIOSFile("IOSNativePopUpsManager");
		RemoveIOSFile("IOSNativeUtility");
		RemoveIOSFile("ISN_NSData+Base64");
		RemoveIOSFile("ISN_Reachability");
		RemoveIOSFile("ISNCamera");
		RemoveIOSFile("ISNDataConvertor");
		RemoveIOSFile("ISNSharedApplication");
		RemoveIOSFile("ISNVideo");
		RemoveIOSFile("PopUPDelegate");
		RemoveIOSFile("RatePopUPDelegate");
		RemoveIOSFile("SKProduct+LocalizedPrice");
		RemoveIOSFile("SocialGate");
		RemoveIOSFile("StoreProductView");
		RemoveIOSFile("TransactionServer");
		RemoveIOSFile("GoogleMobileAdBanner");
		RemoveIOSFile("GoogleMobileAdController");
		RemoveIOSFile("IOSInstaPlugin");
		RemoveIOSFile("IOSTwitterPlugin");
		RemoveIOSFile("MGInstagram");





	}


	private static void RemoveIOSFile(string filename) {
		FileStaticAPI.DeleteFile(IOS_DESTANATION_PATH + filename + ".h");
		FileStaticAPI.DeleteFile(IOS_DESTANATION_PATH + filename + ".m");
		FileStaticAPI.DeleteFile(IOS_DESTANATION_PATH + filename + ".mm");
	}


	public static void Android_UpdatePlugin() {
		Android_InstallPlugin(false);
	}



	public static void EnableGooglePlayAPI() {
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/an_googleplay.txt", 	ANDROID_DESTANATION_PATH + "libs/an_googleplay.jar");
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/google-play-services.txt", 	ANDROID_DESTANATION_PATH + "libs/google-play-services.jar");
	}

	public static void DisableGooglePlayAPI() {
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/google-play-services.jar");
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_googleplay.jar");
	}


	public static void EnableAppLicensingAPI() {
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "app_licensing/an_licensing_library.txt", 	ANDROID_DESTANATION_PATH + "libs/an_licensing_library.jar");
	}


	public static void DisableAppLicensingAPI() {
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_licensing_library.jar");
	}


	public static void EnableBillingAPI() {
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "billing/an_billing.txt", 	ANDROID_DESTANATION_PATH + "libs/an_billing.jar");
	}

	public static void DisableBillingAPI() {
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_billing.jar");
	}




	public static void EnableSocialAPI() {
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "social/an_social.txt", 	ANDROID_DESTANATION_PATH + "libs/an_social.jar");
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "social/twitter4j-core-3.0.5.txt", 	ANDROID_DESTANATION_PATH + "libs/twitter4j-core-3.0.5.jar");
	}
	
	public static void DisableSocialAPI() {
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_social.jar");
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/twitter4j-core-3.0.5.jar");
	}






	public static void EnableCameraAPI() {
		//Unity 5 upgdare:
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "libs/image-chooser-library-1.3.0.txt", 	ANDROID_DESTANATION_PATH + "libs/image-chooser-library-1.3.0.jar");
	}
	
	public static void DisableCameraAPI() {
		FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/image-chooser-library-1.3.0.jar");
	}





	public static void Android_InstallPlugin(bool IsFirstInstall = true) {


		//Unity 5 upgdare:
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/httpclient-4.3.1.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/signpost-commonshttp4-1.2.1.2.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/signpost-core-1.2.1.2.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/libGoogleAnalyticsServices.jar");

		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/android-support-v4.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/image-chooser-library-1.3.0.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/twitter4j-core-3.0.5.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/google-play-services.jar");


		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "social/an_social.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "social/twitter4j-core-3.0.5.jar");


		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "google_play/an_googleplay.jar");
		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "google_play/google-play-services.jar");

		FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "billing/an_billing.jar");



		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "libs/android-support-v4.txt", ANDROID_DESTANATION_PATH + "libs/android-support-v4.jar");
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "androidnative.txt", 	        ANDROID_DESTANATION_PATH + "androidnative.jar");
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "mobilenativepopups.txt", 	        ANDROID_DESTANATION_PATH + "mobilenativepopups.jar");





		FileStaticAPI.CopyFolder(ANDROID_SOURCE_PATH + "facebook", 			ANDROID_DESTANATION_PATH + "facebook");

		#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1	|| UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		
		#else
		FileStaticAPI.DeleteFolder(ANDROID_SOURCE_PATH + "facebook");
		#endif

		if(IsFirstInstall) {
			EnableBillingAPI();
			EnableGooglePlayAPI();
			EnableSocialAPI();
			EnableCameraAPI();
			EnableAppLicensingAPI();
		}


		
		
		string file;
		file = "res/values/" + "analytics.xml";
		if(!FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		}
		
		
		file = "res/values/" + "ids.xml";
		if(!FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		}

		file = "res/xml/" + "file_paths.xml";
		if(!FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		}

		
		file = "res/values/" + "version.xml";
		FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		

		
		//First install dependense


		file = "AndroidManifest.xml";
		if(!FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		} 

		AssetDatabase.Refresh();
		
	}
}
