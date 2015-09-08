using System;
using UnityEngine;
using System.Collections;

public class AndroidNativeUtility : SA_Singleton<AndroidNativeUtility> {
	

	//Actions
	public static event Action<AN_PackageCheckResult> OnPackageCheckResult = delegate{};
	public static event Action<string> OnAndroidIdLoaded = delegate{};

	public static event Action<string> InternalStoragePathLoaded = delegate{};
	public static event Action<string> ExternalStoragePathLoaded = delegate{};


	public static event Action<AN_Locale> LocaleInfoLoaded = delegate{};
	public static event Action<string[]> ActionDevicePackagesListLoaded = delegate{};
	public static event Action<AN_NetworkInfo> ActionNetworkInfoLoaded = delegate{};
	

	
	//--------------------------------------
	// Init
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	//--------------------------------------
	// Public Methods
	//--------------------------------------
	
	
	public void CheckIsPackageInstalled(string packageName) {
		AndroidNative.isPackageInstalled(packageName);
	}

	public void RunPackage(string packageName) {
		AndroidNative.runPackage(packageName);
	}

	public void LoadAndroidId() {
		AndroidNative.LoadAndroidId();
	}


	public void GetInternalStoragePath() {
		AndroidNative.GetInternalStoragePath();
	}
	
	public void GetExternalStoragePath() {
		AndroidNative.GetExternalStoragePath();
	}

	public void LoadLocaleInfo() {
		AndroidNative.LoadLocaleInfo();
	}

	public void LoadPackagesList() {
		AndroidNative.LoadPackagesList();
	}


	public void LoadNetworkInfo() {
		AndroidNative.LoadNetworkInfo();
	}


	
	//--------------------------------------
	// Static Methods
	//--------------------------------------

	public static void OpenSettingsPage(string action) {
		AndroidNative.OpenSettingsPage(action);
	}

	public static void ShowPreloader(string title, string message) {
		AN_PoupsProxy.ShowPreloader(title, message);
	}
	
	public static void HidePreloader() {
		AN_PoupsProxy.HidePreloader();
	}


	public static void OpenAppRatingPage(string url) {
		AN_PoupsProxy.OpenAppRatePage(url);
	}



	public static void HideCurrentPopup() {
		AN_PoupsProxy.HideCurrentPopup();
	}


	


	//--------------------------------------
	// Events
	//--------------------------------------

	private void OnAndroidIdLoadedEvent(string id) {
		OnAndroidIdLoaded(id);
	}

	private void OnPacakgeFound(string packageName) {
		AN_PackageCheckResult result = new AN_PackageCheckResult(packageName, true);
		OnPackageCheckResult(result);
	}

	private void OnPacakgeNotFound(string packageName) {
		AN_PackageCheckResult result = new AN_PackageCheckResult(packageName, false);
		OnPackageCheckResult(result);
	}


	private void OnExternalStoragePathLoaded(string path) {
		ExternalStoragePathLoaded(path);
	}

	private void OnInternalStoragePathLoaded(string path) {
		InternalStoragePathLoaded(path);
	}


	private void OnLocaleInfoLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		AN_Locale locale =  new AN_Locale();
		locale.CountryCode = storeData[0];
		locale.DisplayCountry = storeData[1];

		locale.LanguageCode = storeData[2];
		locale.DisplayLanguage = storeData[3];

		LocaleInfoLoaded(locale);

	}

	private void OnPackagesListLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		ActionDevicePackagesListLoaded(storeData);
	}

	private void OnNetworkInfoLoaded (string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		AN_NetworkInfo info =  new AN_NetworkInfo();
		info.SubnetMask = storeData[0];
		info.IpAddress = storeData[1];
		info.MacAddress =  storeData[2];
		info.SSID = storeData[3];
		info.BSSID = storeData[4];

		info.LinkSpeed = System.Convert.ToInt32(storeData[5]);
		info.NetworkId = System.Convert.ToInt32(storeData[6]);

		ActionNetworkInfoLoaded(info);
	
	}
	


}

