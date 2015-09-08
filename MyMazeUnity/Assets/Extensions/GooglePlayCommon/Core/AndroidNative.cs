﻿////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class AndroidNative {

	//--------------------------------------
	// Constants
	//--------------------------------------

	public const string DATA_SPLITTER = "|";
	public const string DATA_EOF = "endofline";
	public const string DATA_SPLITTER2 = "|%|";
	


	// --------------------------------------
	// Twitter
	// --------------------------------------
	
	public static void TwitterInit(string consumer_key, string consumer_secret) {
		CallAndroidNativeBridge("TwitterInit", consumer_key, consumer_secret);
	}
	
	public static void AuthificateUser() {
		CallAndroidNativeBridge("AuthificateUser");
	}
	
	public static void LoadUserData() {
		CallAndroidNativeBridge("LoadUserData");
	}
	
	public static void TwitterPost(string status) {
		CallAndroidNativeBridge("TwitterPost", status);
	}
	
	public static void TwitterPostWithImage(string status, string data) {
		CallAndroidNativeBridge("TwitterPostWithImage", status, data);
	}
	
	public static void LogoutFromTwitter() {
		CallAndroidNativeBridge("LogoutFromTwitter");
	}




	// --------------------------------------
	// Camera And Gallery
	// --------------------------------------
	

	public static void InitCameraAPI(string folderName, int maxSize, int mode, int format) {
		CallAndroidNativeBridge("InitCameraAPI", folderName, maxSize.ToString(), mode.ToString(), format);
	}

	public static void SaveToGalalry(string ImageData, string name) {
		CallAndroidNativeBridge("SaveToGalalry", ImageData, name);
	}


	public static void GetImageFromGallery() {
		CallAndroidNativeBridge("GetImageFromGallery");
	}
	
	public static void GetImageFromCamera(bool bSaveToGallery = false) {
		CallAndroidNativeBridge("GetImageFromCamera", bSaveToGallery.ToString());
	}


	// --------------------------------------
	// Utils
	// --------------------------------------
	
	public static void isPackageInstalled(string packagename) {
		CallAndroidNativeBridge("isPackageInstalled", packagename);
	}
	
	public static void runPackage(string packagename) {
		CallAndroidNativeBridge("runPackage", packagename);
	}

	public static void LoadAndroidId() {
		CallAndroidNativeBridge("loadAndroidId");
	}

	public static void LoadPackagesList () {
		CallUtility("loadPackagesList");
	}

	public static void LoadNetworkInfo () {
		CallUtility("loadNetworkInfo");
	}

	public static void OpenSettingsPage (string action) {
		CallUtility("openSettingsPage", action);
	}
	
	//--------------------------------------
	// Other Features
	//--------------------------------------
	
	public static void LoadContacts() {
		CallAndroidNativeBridge("loadAddressBook");
	}
	
	
	public static void LoadPackageInfo() {
		CallAndroidNativeBridge("LoadPackageInfo");
	}

	public static void GetInternalStoragePath() {
		CallUtility("GetInternalStoragePath");
	}
	
	public static void GetExternalStoragePath() {
		CallUtility("GetExternalStoragePath");
	}

	public static void LoadLocaleInfo () {
		CallUtility("LoadLocaleInfo");
	}

	public static void StartLockTask() {
		CallAndroidNativeBridge ("StartLockTask");
	}
	
	
	public static void StopLockTask() {
		CallAndroidNativeBridge ("StopLockTask");
	}

	public static void OpenAppInStore(string appPackageName) {
		CallAndroidNativeBridge ("OpenAppInStore", appPackageName);
	}

	private const string UTILITY_CLASSS = "com.androidnative.features.common.AndroidNativeUtility";
	
	private static void CallUtility(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(UTILITY_CLASSS, methodName, args);
	}



	// --------------------------------------
	// Native Bridge
	// --------------------------------------


	private const string CLASS_NAME = "com.androidnative.AN_Bridge";
	
	private static void CallAndroidNativeBridge(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}

}
