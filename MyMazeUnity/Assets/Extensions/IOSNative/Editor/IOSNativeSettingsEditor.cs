﻿
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(IOSNativeSettings))]
public class IOSNativeSettingsEditor : Editor {




	GUIContent AppleIdLabel = new GUIContent("Apple Id [?]:", "Your Application Apple ID.");
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is the Plugin version.  If you have problems or compliments please include this so that we know exactly which version to look out for.");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical questions, feel free to drop us an e-mail");

	GUIContent UseOneSignalLabel = new GUIContent("Use OneSignal API [?]:", "Use OneSignal API for Push Notifications");


	GUIContent SKPVDLabel = new GUIContent("Store Products View [?]:", "The SKStoreProductViewController class makes it possible to integrate purchasing from Apple’s iTunes, App and iBooks stores directly into iOS 6 applications with minimal coding work.");
	GUIContent CheckInternetLabel = new GUIContent("Check Internet Connection[?]:", "If set to true, the Internet connection will be checked before sending load request. Requests will be sent automatically if network becomes available.");
	GUIContent SendBillingFakeActions = new GUIContent("Send Fake Action In Editor[?]:", "Fake connect and purchase events will be fired in the editor, can be useful for testing your implementation in Editor.");

	GUIContent UseGCCaching  = new GUIContent("Use Requests Caching[?]:", "Requests to Game Center will be cached if no Internet connection is available. Requests will be resent on the next Game Center connect event.");


	GUIContent EnablePushNotification  = new GUIContent("Enable Push Notifications API[?]:", "Enables Push Notifications Api");
	GUIContent DisablePluginLogsNote  = new GUIContent("Disable Plugin Logs[?]:", "All plugins 'Debug.Log' lines will be disabled if this option is enabled.");


	private IOSNativeSettings settings;

	void Awake() {
		#if !UNITY_WEBPLAYER
		UpdatePluginSettings();
		#endif
	}

	private void UpdatePluginSettings() {
		string IOSNotificationControllerContent = FileStaticAPI.Read("Extensions/IOSNative/Notifications/IOSNotificationController.cs");
		string DeviceTokenListenerContent = FileStaticAPI.Read("Extensions/IOSNative/Notifications/DeviceTokenListener.cs");


		int endlineIndex;
		endlineIndex = DeviceTokenListenerContent.IndexOf(System.Environment.NewLine);
		if(endlineIndex == -1) {
			endlineIndex = DeviceTokenListenerContent.IndexOf("\n");
		}
		string DTL_Line = DeviceTokenListenerContent.Substring(0, endlineIndex);



		endlineIndex = IOSNotificationControllerContent.IndexOf(System.Environment.NewLine);
		if(endlineIndex == -1) {
			endlineIndex = IOSNotificationControllerContent.IndexOf("\n");
		}
		string INC_Line = IOSNotificationControllerContent.Substring(0, endlineIndex);




		if(IOSNativeSettings.Instance.EnablePushNotificationsAPI) {
			IOSNotificationControllerContent 	= IOSNotificationControllerContent.Replace(INC_Line, "#define PUSH_ENABLED");
			DeviceTokenListenerContent 			= DeviceTokenListenerContent.Replace(DTL_Line, "#define PUSH_ENABLED");
		} else {
			IOSNotificationControllerContent 	= IOSNotificationControllerContent.Replace(INC_Line, "//#define PUSH_ENABLED");
			DeviceTokenListenerContent 			= DeviceTokenListenerContent.Replace(DTL_Line, "//#define PUSH_ENABLED");
		}

		FileStaticAPI.Write("Extensions/IOSNative/Notifications/IOSNotificationController.cs", IOSNotificationControllerContent);
		FileStaticAPI.Write("Extensions/IOSNative/Notifications/DeviceTokenListener.cs", DeviceTokenListenerContent);
	}

	public override void OnInspectorGUI()  {


		#if UNITY_WEBPLAYER
		EditorGUILayout.HelpBox("Editing IOS Native Settings not available with web player platfrom. Please switch to any other platform under Build Settings menu", MessageType.Warning);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if(GUILayout.Button("Switch To IOS Platfrom",  GUILayout.Width(150))) {
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
		}
		EditorGUILayout.EndHorizontal();

		if(Application.isEditor) {
			return;
		}

		#endif


		settings = target as IOSNativeSettings;

		GUI.changed = false;



		GeneralOptions();

		EditorGUILayout.HelpBox("(Optional) Services Settings", MessageType.None);
		BillingSettings();
		EditorGUILayout.Space();
		GameCenterSettings();
		EditorGUILayout.Space();
		OtherSettins();
		EditorGUILayout.Space();


		AboutGUI();
	

		if(GUI.changed) {
			DirtyEditor();
		}

	}




	private void GeneralOptions() {


		EditorGUILayout.HelpBox("(Required) Application Data", MessageType.None);

		if (settings.AppleId.Length == 0 || settings.AppleId.Equals("XXXXXXXXX")) {
			EditorGUILayout.HelpBox("Invalid Apple ID", MessageType.Error);
		}



		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(AppleIdLabel);
		settings.AppleId	 	= EditorGUILayout.TextField(settings.AppleId);
		if(settings.AppleId.Length > 0) {
			settings.AppleId		= settings.AppleId.Trim();
		}

		EditorGUILayout.EndHorizontal();




		EditorGUILayout.Space();

	}

	public static void CameraAndGallery() {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Max Loaded Image Size");
		IOSNativeSettings.Instance.MaxImageLoadSize	 	= EditorGUILayout.IntField(IOSNativeSettings.Instance.MaxImageLoadSize);
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Loaded Image Format");
		IOSNativeSettings.Instance.GalleryImageFormat	 	= (IOSGalleryLoadImageFormat) EditorGUILayout.EnumPopup(IOSNativeSettings.Instance.GalleryImageFormat);
		EditorGUILayout.EndHorizontal();


		if(IOSNativeSettings.Instance.GalleryImageFormat == IOSGalleryLoadImageFormat.JPEG) {
			GUI.enabled = true;
		} else {
			GUI.enabled = false;
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("JPEG Compression Rate");
		IOSNativeSettings.Instance.JPegCompressionRate	 	= EditorGUILayout.Slider(IOSNativeSettings.Instance.JPegCompressionRate, 0f, 1f);
		EditorGUILayout.EndHorizontal();
		GUI.enabled = true;

	}

	private void GameCenterSettings() {
		IOSNativeSettings.Instance.ShowGCParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowGCParams, "Game Center");
		if (IOSNativeSettings.Instance.ShowGCParams) {
		
			EditorGUI.indentLevel++;
			IOSNativeSettings.Instance.ShowAchievementsParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowAchievementsParams, "Achievements");
			if (IOSNativeSettings.Instance.ShowAchievementsParams) {
				if(IOSNativeSettings.Instance.RegisteredAchievementsIds.Count == 0) {
					EditorGUILayout.HelpBox("No Achievement IDs Registered", MessageType.Info);
				}
				
				
				int i = 0;
				foreach(string str in IOSNativeSettings.Instance.RegisteredAchievementsIds) {
					EditorGUILayout.BeginHorizontal();
					IOSNativeSettings.Instance.RegisteredAchievementsIds[i]	 	= EditorGUILayout.TextField(IOSNativeSettings.Instance.RegisteredAchievementsIds[i]);
					if(IOSNativeSettings.Instance.RegisteredAchievementsIds[i].Length > 0) {
						IOSNativeSettings.Instance.RegisteredAchievementsIds[i]		= IOSNativeSettings.Instance.RegisteredAchievementsIds[i].Trim();
					}

					if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
						IOSNativeSettings.Instance.RegisteredAchievementsIds.Remove(str);
						break;
					}
					EditorGUILayout.EndHorizontal();
					i++;
				}
				
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add",  GUILayout.Width(80))) {
					IOSNativeSettings.Instance.RegisteredAchievementsIds.Add("");
				}
				EditorGUILayout.EndHorizontal();


				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(UseGCCaching);
				IOSNativeSettings.Instance.UseGCRequestCaching = EditorGUILayout.Toggle(IOSNativeSettings.Instance.UseGCRequestCaching);
				EditorGUILayout.EndHorizontal();
				
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Save progress in PlayerPrefs[?]");
				IOSNativeSettings.Instance.UsePPForAchievements = EditorGUILayout.Toggle(IOSNativeSettings.Instance.UsePPForAchievements);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Read More",  GUILayout.Width(100))) {
					Application.OpenURL("http://goo.gl/3nq260");
				}
				EditorGUILayout.EndHorizontal();

			}

			EditorGUI.indentLevel--;




		}
	}

	private void OtherSettins() {

		IOSNativeSettings.Instance.ShowCameraAndGalleryParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowCameraAndGalleryParams, "Camera And Gallery");
		if (IOSNativeSettings.Instance.ShowCameraAndGalleryParams) {
		
			CameraAndGallery();
		}

		EditorGUILayout.Space();
		
		IOSNativeSettings.Instance.ShowOtherParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowOtherParams, "Other Settings");
		if (IOSNativeSettings.Instance.ShowOtherParams) {

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(EnablePushNotification);
			IOSNativeSettings.Instance.EnablePushNotificationsAPI = EditorGUILayout.Toggle(IOSNativeSettings.Instance.EnablePushNotificationsAPI);
			EditorGUILayout.EndHorizontal();

			if(IOSNativeSettings.Instance.EnablePushNotificationsAPI) {
				EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(UseOneSignalLabel);
				IOSNativeSettings.Instance.UseOneSignal = EditorGUILayout.Toggle(IOSNativeSettings.Instance.UseOneSignal);
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}


			if(EditorGUI.EndChangeCheck()) {
				UpdatePluginSettings();
			}


			EditorGUI.BeginChangeCheck();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(DisablePluginLogsNote);
			IOSNativeSettings.Instance.DisablePluginLogs = EditorGUILayout.Toggle(IOSNativeSettings.Instance.DisablePluginLogs);
			EditorGUILayout.EndHorizontal();


		}
	}


	private void BillingSettings() {

		IOSNativeSettings.Instance.ShowStoreKitParams = EditorGUILayout.Foldout(IOSNativeSettings.Instance.ShowStoreKitParams, "Billing Settings");
		if(IOSNativeSettings.Instance.ShowStoreKitParams) {

			if(settings.InAppProducts.Count == 0) {
				EditorGUILayout.HelpBox("No In-App Products Added", MessageType.Warning);
			}
		

			int i = 0;
			foreach(string str in settings.InAppProducts) {
				EditorGUILayout.BeginHorizontal();
				settings.InAppProducts[i]	 	= EditorGUILayout.TextField(settings.InAppProducts[i]);
				if(settings.InAppProducts[i].Length > 0) {
					settings.InAppProducts[i]		= settings.InAppProducts[i].Trim();
				}
			
				if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
					settings.InAppProducts.Remove(str);
					break;
				}
				EditorGUILayout.EndHorizontal();
				i++;
			}


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(80))) {
				settings.InAppProducts.Add("");
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.Space();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(SendBillingFakeActions);
			settings.SendFakeEventsInEditor = EditorGUILayout.Toggle(settings.SendFakeEventsInEditor);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(CheckInternetLabel);
			settings.checkInternetBeforeLoadRequest = EditorGUILayout.Toggle(settings.checkInternetBeforeLoadRequest);
			EditorGUILayout.EndHorizontal();






			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(SKPVDLabel);

			/*****************************************/

			if(settings.DefaultStoreProductsView.Count == 0) {
				EditorGUILayout.HelpBox("No Default Store Products View Added", MessageType.Info);
			}
			
			
			i = 0;
			foreach(string str in settings.DefaultStoreProductsView) {
				EditorGUILayout.BeginHorizontal();
				settings.DefaultStoreProductsView[i]	 	= EditorGUILayout.TextField(settings.DefaultStoreProductsView[i]);
				if(settings.DefaultStoreProductsView[i].Length > 0) {
					settings.DefaultStoreProductsView[i]		= settings.DefaultStoreProductsView[i].Trim();
				}

				if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
					settings.DefaultStoreProductsView.Remove(str);
					break;
				}
				EditorGUILayout.EndHorizontal();
				i++;
			}
			
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(80))) {
				settings.DefaultStoreProductsView.Add("");
			}
			EditorGUILayout.EndHorizontal();



			EditorGUILayout.Space();

		}
	}




	private void AboutGUI() {




		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		EditorGUILayout.Space();
		
		SelectableLabelField(SdkVersion, IOSNativeSettings.VERSION_NUMBER);
		SelectableLabelField(SupportEmail, "stans.assets@gmail.com");
		
		
	}
	
	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}



	private static void DirtyEditor() {
		#if UNITY_EDITOR
		EditorUtility.SetDirty(IOSNativeSettings.Instance);
		#endif
	}
	
	
}
