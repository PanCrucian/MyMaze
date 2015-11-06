﻿
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

[CustomEditor(typeof(AndroidNativeSettings))]
public class AndroidNativeSettingsEditor : Editor {


	GUIContent PlusApiLabel   = new GUIContent("Enable Plus API [?]:", "API used for account managment");
	GUIContent GamesApiLabel   = new GUIContent("Enable Games API [?]:", "API used for achivements and leaderboards");
	GUIContent AppSateApiLabel = new GUIContent("Enable App State API [?]:", "API used for cloud data save");
	GUIContent DriveApiLabel = new GUIContent("Enable Drive API [?]:", "API used for saved games");


	GUIContent Base64KeyLabel = new GUIContent("Base64 Key[?]:", "Base64 Key app key.");
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	GUIContent GPSdkVersion   = new GUIContent("Google Play SDK Version [?]", "Version of Google Play SDK used by the plugin");
	GUIContent FBdkVersion   = new GUIContent("Facebook SDK Version [?]", "Version of Unity Facebook SDK Plugin");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical quastion, feel free to drop an e-mail");


	private AndroidNativeSettings settings;


	void Awake() {
		ApplaySettings();

		if(IsInstalled && IsUpToDate) {
			UpdateManifest();
		}

		#if !UNITY_WEBPLAYER
		UpdatePluginSettings();
		#endif

	}

	public static void UpdatePluginSettings() {
		string AndroidNativeSettingsContent = FileStaticAPI.Read("Extensions/GooglePlayCommon/Core/AndroidNativeSettings.cs");
	
		
		
		int endlineIndex;
		endlineIndex = AndroidNativeSettingsContent.IndexOf(System.Environment.NewLine);
		if(endlineIndex == -1) {
			endlineIndex = AndroidNativeSettingsContent.IndexOf("\n");
		}

		string ANS_Line = AndroidNativeSettingsContent.Substring(0, endlineIndex);
		


		


		if(AndroidNativeSettings.Instance.EnableATCSupport) {
			AndroidNativeSettingsContent 	= AndroidNativeSettingsContent.Replace(ANS_Line, "#define ATC_SUPPORT_ENABLED");
		} else {
			AndroidNativeSettingsContent 	= AndroidNativeSettingsContent.Replace(ANS_Line, "//#define ATC_SUPPORT_ENABLED");
		}
		
		FileStaticAPI.Write("Extensions/GooglePlayCommon/Core/AndroidNativeSettings.cs", AndroidNativeSettingsContent);
	}


	private Texture[] _ToolbarImages = null;

	public Texture[] ToolbarImages {
		get {
			if(_ToolbarImages == null) {
				Texture2D market =  Resources.Load("market") as Texture2D;
				Texture2D googleplay =  Resources.Load("googleplay") as Texture2D;
				Texture2D notifications =  Resources.Load("notifications") as Texture2D;
				Texture2D sharing =  Resources.Load("sharing") as Texture2D;
				Texture2D other =  Resources.Load("other") as Texture2D;

				Texture2D android =  Resources.Load("android") as Texture2D;
				Texture2D camera =  Resources.Load("gallery") as Texture2D;
				
				List<Texture2D> textures =  new List<Texture2D>();
				textures.Add(android);
				textures.Add(googleplay);
				textures.Add(market);
				textures.Add(notifications);
				textures.Add(sharing);
				textures.Add(camera);
				textures.Add(other);


				_ToolbarImages = textures.ToArray();

			}
			return _ToolbarImages;
		}
	}


	private int _Width = 500;
	public int Width {
		get {
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			Rect scale = GUILayoutUtility.GetLastRect();

			if(scale.width != 1) {
				_Width = System.Convert.ToInt32(scale.width);
			}

			return _Width;
		}
	}

	public override void OnInspectorGUI() {
		#if UNITY_WEBPLAYER
		EditorGUILayout.HelpBox("Editing Android Native Settings not avaliable with web player platfrom. Please swith to any other platfrom under Build Seting menu", MessageType.Warning);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if(GUILayout.Button("Switch To Android Platfrom",  GUILayout.Width(180))) {
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		}
		EditorGUILayout.EndHorizontal();

		if(Application.isEditor) {
			return;
		}



		#endif


		settings = target as AndroidNativeSettings;

		GUI.changed = false;

		InstallOptions();
	

		GUILayoutOption[] toolbarSize = new GUILayoutOption[]{GUILayout.Width(Width-5), GUILayout.Height(30)};

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		AndroidNativeSettings.Instance.ToolbarSelectedIndex =  GUILayout.Toolbar(AndroidNativeSettings.Instance.ToolbarSelectedIndex, ToolbarImages, toolbarSize);
		GUILayout.FlexibleSpace();

		EditorGUILayout.EndHorizontal();

		switch(AndroidNativeSettings.Instance.ToolbarSelectedIndex) {
		case 0:
			PluginSetting();
			EditorGUILayout.Space();
			AboutGUI();
			break;

		case 1:
			PlayServiceSettings();
			break;

		case 2:
			BillingSettings();
			break;

		case 3:
			NotificationsSettings();
			break;

		case 4:
			SocialPlatfromSettingsEditor.FacebookSettings();
			EditorGUILayout.Space();
			SocialPlatfromSettingsEditor.TwitterSettings();
			break;
		case 5:
			CameraAndGalleryParams();
			break;

		case 6:
			ThirdPartyParams ();
			break;
		}


		if(GUI.changed) {
			DirtyEditor();
		}

	}
	

	public static bool IsInstalled {
		get {
			return SA_VersionsManager.Is_AN_Installed;
		}
	}

	public static bool IsUpToDate {
		get {
			if(CurrentVersion == SA_VersionsManager.AN_Version) {
				return true;
			} else {
				return false;
			}
		}
	}


	public static int CurrentVersion {
		get {
			return SA_VersionsManager.ParceVersion(AndroidNativeSettings.VERSION_NUMBER);
		}
	}

	public static int CurrentMagorVersion {
		get {
			return SA_VersionsManager.ParceMagorVersion(AndroidNativeSettings.VERSION_NUMBER);
		}
	}



	public static bool IsFacebookInstalled {
		get {
			if(!FileStaticAPI.IsFolderExists("Facebook")) {
				return false;
			} else {
				return true;
			}
		}
	}


	public static void UpdateVersionInfo() {
		FileStaticAPI.Write(SA_VersionsManager.AN_VERSION_INFO_PATH, AndroidNativeSettings.VERSION_NUMBER);
		SocialPlatfromSettingsEditor.UpdateVersionInfo();
		UpdateManifest();
	}





	private void DrawOpenManifestButton() {


		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		
		if(GUILayout.Button("Open Manifest ",  GUILayout.Width(120))) {
			UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal("Assets" + AN_ManifestManager.MANIFEST_FILE_PATH, 1);
		}
		EditorGUILayout.EndHorizontal();
	}
	

	private void InstallOptions() {



		if(!IsInstalled) {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.HelpBox("Install Required ", MessageType.Error);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			Color c = GUI.color;
			GUI.color = Color.cyan;
			if(GUILayout.Button("Install Plugin",  GUILayout.Width(350))) {
				PluginsInstalationUtil.Android_InstallPlugin();
				UpdateVersionInfo();
			}

			EditorGUILayout.Space();
			GUI.color = c;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();

		}

		if(IsInstalled) {
			if(!IsUpToDate) {
				EditorGUILayout.BeginVertical (GUI.skin.box);
				EditorGUILayout.HelpBox("Update Required \nResources version: " + SA_VersionsManager.AN_StringVersionId + " Plugin version: " + AndroidNativeSettings.VERSION_NUMBER, MessageType.Warning);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				Color c = GUI.color;
				GUI.color = Color.cyan;



				if(CurrentMagorVersion != SA_VersionsManager.AN_MagorVersion) {
					if(GUILayout.Button("How to update",  GUILayout.Width(350))) {
						Application.OpenURL("https://goo.gl/Z9wgEI");
					}
				} else {
					if(GUILayout.Button("Upgrade Resources",  GUILayout.Width(350))) {
						AN_Plugin_Update();
						UpdateVersionInfo();
					}
				}


				GUI.color = c;
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();

				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();


			} else {
				EditorGUILayout.HelpBox("Android Native Plugin v" + AndroidNativeSettings.VERSION_NUMBER + " is installed", MessageType.Info);
			}
		}


		EditorGUILayout.Space();

	}


	private void Actions() {

		EditorGUILayout.Space();
 		EditorGUILayout.LabelField("More Actions", EditorStyles.boldLabel);


		if(!IsFacebookInstalled) {
			GUI.enabled = false;
		}	

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();

		if(GUILayout.Button("Remove Facebook SDK",  GUILayout.Width(160))) {
			bool result = EditorUtility.DisplayDialog(
				"Removing Facebook SDK",
				"Warning action can not be undone without reimporting the plugin",
				"Remove",
				"Cansel");

			if(result) {
				PluginsInstalationUtil.Remove_FB_SDK();
			}

		}

		GUI.enabled = true;

		if(GUILayout.Button("Open Manifest ",  GUILayout.Width(160))) {
			UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal("Assets" + AN_ManifestManager.MANIFEST_FILE_PATH, 1);
		}

		if(GUILayout.Button("Reset Settings",  GUILayout.Width(160))) {
			
			SocialPlatfromSettingsEditor.ResetSettings();
			
			FileStaticAPI.DeleteFile("Extensions/AndroidNative/Resources/AndroidNativeSettings.asset");
			AndroidNativeSettings.Instance.ShowActions = true;
			Selection.activeObject = AndroidNativeSettings.Instance;
			
			return;
		}
		EditorGUILayout.Space();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();


		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();

		if(GUILayout.Button("Load Example Settings",  GUILayout.Width(160))) {
			LoadExampleSettings();
		}

		if(GUILayout.Button("Reinstall",  GUILayout.Width(160))) {
			AN_Plugin_Update();
			UpdateVersionInfo();
			
		}
		if(GUILayout.Button("Remove",  GUILayout.Width(160))) {
			SA_RemoveTool.RemovePlugins();
		}

		EditorGUILayout.Space();

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		
	}

	public static void LoadExampleSettings()  {
		AndroidNativeSettings.Instance.base64EncodedPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsV676BTvO5djSDdUwotbLCIPtGZ5OVCbIn402RXuEpDwuHZMIOy5E6DQjUlQPKCiB7A1Vx+ePQI50Gk8NO1zuPRBgCgvW/oTTf863KkF34QLZD+Ii8fc6VE0UKp3GfApnLmq2qtr1fwDmRCteBUET1h0EcRn3/6R/BA5DMmF1aTv8yUY5LQETWqEPIjGdyNaAhmnWf2sTliYLANiR51WXsfbDdCNT4Ux3gQo/XJynGadfwRS7A9N9e5SgvMEFUR6EwnANOF9QXgE2d0HEitpS56D3uHH/2LwICrTWAmbLX3qPYlQ3Ncf1SRyjqiKae2wW8QUnDFU5BSozwGW6tcQvQIDAQAB";
		AndroidNativeSettings.Instance.InAppProducts =  new List<GoogleProductTemplate>();

		AndroidNativeSettings.Instance.InAppProducts.Add(new GoogleProductTemplate(){ SKU = "coins_bonus", 		Title = "Bonus Coins", IsOpen = false});
		AndroidNativeSettings.Instance.InAppProducts.Add(new GoogleProductTemplate(){ SKU = "small_coins_bag", 	Title = "Small Coins Bag", IsOpen = false});
		AndroidNativeSettings.Instance.InAppProducts.Add(new GoogleProductTemplate(){ SKU = "pm_coins", 		Title = "Coins Pack", IsOpen = false});
		AndroidNativeSettings.Instance.InAppProducts.Add(new GoogleProductTemplate(){ SKU = "pm_green_sphere", 	Title = "Green Sphere", IsOpen = false});
		AndroidNativeSettings.Instance.InAppProducts.Add(new GoogleProductTemplate(){ SKU = "pm_red_sphere", 	Title = "Red Sphere", IsOpen = false});


		AndroidNativeSettings.Instance.SoomlaEnvKey = "3c3df370-ad80-4577-8fe5-ca2c49b2c1b4";
		AndroidNativeSettings.Instance.SoomlaGameKey = "db24ba61-3aa7-4653-a3f7-9c613cb2c0f3";
		
		AndroidNativeSettings.Instance.GCM_SenderId = "216817929098";
		AndroidNativeSettings.Instance.GooglePlayServiceAppID = "216817929098";

		PlayerSettings.bundleIdentifier = "com.unionassets.android.plugin.preview";

		SocialPlatfromSettingsEditor.LoadExampleSettings();
	}



	private void PluginSetting() {

		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Plugin Settings", MessageType.None);


		EditorGUILayout.LabelField("Android Native Libs", EditorStyles.boldLabel);
	
		
		EditorGUI.indentLevel++;
		EditorGUI.BeginChangeCheck();
		
		
		
		//Native Lib API
		EditorGUILayout.BeginHorizontal();
		settings.ExpandNativeAPI = EditorGUILayout.Foldout(settings.ExpandNativeAPI, "Enable Native Lib");
		SuperSpace();
		GUI.enabled = false;
		EditorGUILayout.Toggle(true);
		GUI.enabled = true;
		EditorGUILayout.EndHorizontal();
		if(settings.ExpandNativeAPI) {
			EditorGUI.indentLevel++;
			
			
			EditorGUILayout.BeginHorizontal();
			settings.LocalNotificationsAPI = EditorGUILayout.Toggle(AN_API_NAME.LocalNotifications,  settings.LocalNotificationsAPI);
			settings.ImmersiveModeAPI = EditorGUILayout.Toggle(AN_API_NAME.ImmersiveMode,  settings.ImmersiveModeAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			settings.ApplicationInformationAPI = EditorGUILayout.Toggle(AN_API_NAME.ApplicationInformation,  settings.ApplicationInformationAPI);
			settings.ExternalAppsAPI = EditorGUILayout.Toggle(AN_API_NAME.RunExternalApp,  settings.ExternalAppsAPI);
			EditorGUILayout.EndHorizontal();
			
			
			EditorGUILayout.BeginHorizontal();
			settings.PoupsandPreloadersAPI = EditorGUILayout.Toggle(AN_API_NAME.PoupsandPreloaders,  settings.PoupsandPreloadersAPI);
			settings.CheckAppLicenseAPI = EditorGUILayout.Toggle(AN_API_NAME.CheckAppLicense,  settings.CheckAppLicenseAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			settings.NetworkStateAPI = EditorGUILayout.Toggle(AN_API_NAME.NetworkInfo,  settings.NetworkStateAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
		
		
		
		
		EditorGUILayout.BeginHorizontal();
		settings.ExpandBillingAPI = EditorGUILayout.Foldout(settings.ExpandBillingAPI, "Enable Billing Lib");
		SuperSpace();
		settings.EnableBillingAPI	 	= EditorGUILayout.Toggle(settings.EnableBillingAPI);
		
		EditorGUILayout.EndHorizontal();
		if(settings.ExpandBillingAPI) {
			EditorGUI.indentLevel++;
			
			
			EditorGUILayout.BeginHorizontal();
			settings.InAppPurchasesAPI = EditorGUILayout.Toggle(AN_API_NAME.InAppPurchases,  settings.InAppPurchasesAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
		
		
		//GOOGLE PLAY API
		EditorGUILayout.BeginHorizontal();
		settings.ExpandPSAPI = EditorGUILayout.Foldout(settings.ExpandPSAPI, "Enable Google Play Lib");
		SuperSpace();
		
		settings.EnablePSAPI = EditorGUILayout.Toggle(settings.EnablePSAPI);
		
		EditorGUILayout.EndHorizontal();
		
		if(settings.ExpandPSAPI) {
			EditorGUI.indentLevel++;
			
			EditorGUILayout.BeginHorizontal();
			settings.GooglePlayServicesAPI = EditorGUILayout.Toggle(AN_API_NAME.GooglePlayServices,  settings.GooglePlayServicesAPI);
			settings.PlayServicesAdvancedSignInAPI = EditorGUILayout.Toggle(AN_API_NAME.GooglePlayServicesAdvancedSignIn,  settings.PlayServicesAdvancedSignInAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			settings.PushNotificationsAPI = EditorGUILayout.Toggle(AN_API_NAME.PushNotifications,  settings.PushNotificationsAPI);
			settings.GoogleCloudSaveAPI = EditorGUILayout.Toggle(AN_API_NAME.GoogleCloudSave,  settings.GoogleCloudSaveAPI);
			EditorGUILayout.EndHorizontal();
			
			
			EditorGUILayout.BeginHorizontal();
			settings.AnalyticsAPI = EditorGUILayout.Toggle(AN_API_NAME.Analytics,  settings.AnalyticsAPI);
			settings.GoogleMobileAdAPI = EditorGUILayout.Toggle(AN_API_NAME.GoogleMobileAd,  settings.GoogleMobileAdAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			settings.GoogleButtonAPI = EditorGUILayout.Toggle(AN_API_NAME.GoogleButton,  settings.GoogleButtonAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
		
		
		
		
		
		EditorGUILayout.BeginHorizontal();
		settings.ExpandSocialAPI = EditorGUILayout.Foldout(settings.ExpandSocialAPI, "Enable Social Lib");
		SuperSpace();
		
		
		settings.EnableSocialAPI	 	= EditorGUILayout.Toggle(settings.EnableSocialAPI);
		EditorGUILayout.EndHorizontal();
		if(settings.ExpandSocialAPI) {
			EditorGUI.indentLevel++;
			
			SocialPlatfromSettingsEditor.DrawAPIsList();
			
			
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
		
		
		
		EditorGUILayout.BeginHorizontal();
		settings.ExpandCameraAPI = EditorGUILayout.Foldout(settings.ExpandCameraAPI, "Enable Camera Lib");
		SuperSpace();
		
		settings.EnableCameraAPI	 	= EditorGUILayout.Toggle(settings.EnableCameraAPI);
		
		
		EditorGUILayout.EndHorizontal();
		if(settings.ExpandCameraAPI) {
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();
			settings.CameraAPI = EditorGUILayout.Toggle(AN_API_NAME.CameraAPI,  settings.CameraAPI);
			settings.GalleryAPI = EditorGUILayout.Toggle(AN_API_NAME.Gallery,  settings.GalleryAPI);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
		
		
		EditorGUI.indentLevel--;
		
		if(EditorGUI.EndChangeCheck()) {
			UpdateAPIsInstalation();
		}


		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Android Manifest", EditorStyles.boldLabel);


		EditorGUI.indentLevel++;


		AN_ManifestManager.Refresh();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Keep Android Mnifest Clean");

		EditorGUI.BeginChangeCheck();
		AndroidNativeSettings.Instance.KeepManifestClean = EditorGUILayout.Toggle(AndroidNativeSettings.Instance.KeepManifestClean);
		SocialPlatfromSettings.Instance.KeepManifestClean = AndroidNativeSettings.Instance.KeepManifestClean;
		if(EditorGUI.EndChangeCheck()) {
			UpdateManifest();
		}

		if(GUILayout.Button("[?]",  GUILayout.Width(27))) {
			Application.OpenURL("http://goo.gl/syIebl");
		}
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.EndHorizontal();



		AndroidNativeSettings.Instance.ShowAppPermissions = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowAppPermissions, "Application Permissions");
		if(AndroidNativeSettings.Instance.ShowAppPermissions) {
			AN_ManifestManager.Refresh();


			EditorGUILayout.LabelField("Required By Android Native:", EditorStyles.boldLabel);
			List<string> permissions = GetRequiredPermissions();

			foreach(string p in permissions) {
				EditorGUILayout.BeginVertical (GUI.skin.box);
				EditorGUILayout.BeginHorizontal();
				
				EditorGUILayout.SelectableLabel(p, GUILayout.Height(16));

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Other Permissions in Manifest:", EditorStyles.boldLabel);
			foreach(AN_PropertyTemplate tpl in AN_ManifestManager.GetManifest().Permissions) {
				if(!permissions.Contains(tpl.Name)) {

					EditorGUILayout.BeginVertical (GUI.skin.box);
					EditorGUILayout.BeginHorizontal();
					
					EditorGUILayout.SelectableLabel(tpl.Name, GUILayout.Height(16));
					if(GUILayout.Button("x",  GUILayout.Width(20))) {
						AN_ManifestManager.GetManifest().RemovePermission(tpl);
						AN_ManifestManager.SaveManifest();
						return;
					}

					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}
			} 


			//EditorGUI.indentLevel--;
		}


		EditorGUI.indentLevel--;


		Actions();

		EditorGUILayout.Space();
	}

	private static void SuperSpace() {
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
	}

	

	public static void UpdateAPIsInstalation() {


		if(AndroidNativeSettings.Instance.EnableBillingAPI) {
			PluginsInstalationUtil.EnableBillingAPI();
		} else {
			PluginsInstalationUtil.DisableBillingAPI();
			AndroidNativeSettings.Instance.InAppPurchasesAPI = false;
		}



		
		if(AndroidNativeSettings.Instance.EnablePSAPI) {
			PluginsInstalationUtil.EnableGooglePlayAPI();
		} else {
			PluginsInstalationUtil.DisableGooglePlayAPI();

			AndroidNativeSettings.Instance.GooglePlayServicesAPI = false;
			AndroidNativeSettings.Instance.PushNotificationsAPI = false;

			AndroidNativeSettings.Instance.GoogleCloudSaveAPI = false;
			AndroidNativeSettings.Instance.GoogleMobileAdAPI = false;
		
			AndroidNativeSettings.Instance.AnalyticsAPI = false;
			AndroidNativeSettings.Instance.GoogleButtonAPI = false;
		}


		if(AndroidNativeSettings.Instance.EnableSocialAPI) {
			PluginsInstalationUtil.EnableSocialAPI();
		} else {
			PluginsInstalationUtil.DisableSocialAPI();
			SocialPlatfromSettings.Instance.TwitterAPI = false;
			SocialPlatfromSettings.Instance.NativeSharingAPI = false;
			SocialPlatfromSettings.Instance.InstagramAPI = false;
		}


		if(AndroidNativeSettings.Instance.EnableCameraAPI) {
			PluginsInstalationUtil.EnableCameraAPI();
		} else {
			PluginsInstalationUtil.DisableCameraAPI();
			AndroidNativeSettings.Instance.CameraAPI = false;
			AndroidNativeSettings.Instance.GalleryAPI = false;
		}


		if(AndroidNativeSettings.Instance.GooglePlayServicesAPI == false) {
			AndroidNativeSettings.Instance.PlayServicesAdvancedSignInAPI = false;
		}


		if(AndroidNativeSettings.Instance.CheckAppLicenseAPI) {
			PluginsInstalationUtil.EnableAppLicensingAPI();
		} else {
			PluginsInstalationUtil.DisableAppLicensingAPI();
		}


		UpdateManifest();
		
	
	}



	public static void UpdateManifest() {

		if(!AndroidNativeSettings.Instance.KeepManifestClean) {
			return;
		}

		UpdateAppID ();

		AN_ManifestManager.Refresh();

		int UpdateId = 0;
		AN_ManifestTemplate Manifest =  AN_ManifestManager.GetManifest();
		AN_ApplicationTemplate application =  Manifest.ApplicationTemplate;
		AN_ActivityTemplate launcherActivity = application.GetLauncherActivity();


		if(launcherActivity.Name == "com.androidnative.AndroidNativeBridge") {
			launcherActivity.SetName("com.unity3d.player.UnityPlayerNativeActivity");
		}

		foreach (KeyValuePair<int, AN_ActivityTemplate> a in application.Activities) {
			if (a.Value.Name.Equals("com.unity3d.player.UnityPlayerNativeActivity") && !a.Value.IsLauncher) {
				application.RemoveActivity(a.Value);
				break;
			}
		}

		////////////////////////
		//REQUIRED
		////////////////////////
		AN_ActivityTemplate AndroidNativeProxy = application.GetOrCreateActivityWithName("com.androidnative.AndroidNativeProxy");
		AndroidNativeProxy.SetValue("android:launchMode", "singleTask");
		AndroidNativeProxy.SetValue("android:label", "@string/app_name");
		AndroidNativeProxy.SetValue("android:configChanges", "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
		AndroidNativeProxy.SetValue("android:theme", "@android:style/Theme.Translucent.NoTitleBar");





		////////////////////////
		//Google Play Service API
		////////////////////////
		AN_PropertyTemplate games_version = application.GetOrCreatePropertyWithName("meta-data",  "com.google.android.gms.version");
		if(AndroidNativeSettings.Instance.EnablePSAPI) {
			games_version.SetValue("android:value", "@integer/google_play_services_version");

			AN_PropertyTemplate property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.version");
			property.SetValue("android:value", AndroidNativeSettings.GOOGLE_PLAY_SDK_VERSION_NUMBER);
		} else {
			application.RemoveProperty(games_version);
		}

		////////////////////////
		//GooglePlayServicesAPI
		////////////////////////

		UpdateId++;
		AN_PropertyTemplate games_APP_ID  = application.GetOrCreatePropertyWithName("meta-data",  "com.google.android.gms.games.APP_ID");
		if(!AndroidNativeSettings.Instance.GooglePlayServicesAPI) {
			application.RemoveProperty(games_APP_ID);
		} else {
			games_APP_ID.SetValue("android:value", "@string/app_id");
						
			AN_PropertyTemplate property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.games.APP_ID");
			property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);
		}
		
		////////////////////////
		//GoogleCloudSaveAPI
		////////////////////////
		UpdateId++;
		AN_PropertyTemplate appstate_APP_ID = application.GetOrCreatePropertyWithName("meta-data",  "com.google.android.gms.appstate.APP_ID");
		if(AndroidNativeSettings.Instance.GoogleCloudSaveAPI) {
			appstate_APP_ID.SetValue("android:value", "@string/app_id");

			AN_PropertyTemplate property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.appstate.APP_ID");
			property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);

			AndroidNativeSettings.Instance.EnableAppStateAPI = true;
		} else {
			AndroidNativeSettings.Instance.EnableAppStateAPI = false;
			application.RemoveProperty(appstate_APP_ID);
		}


		////////////////////////
		//AnalyticsAPI
		////////////////////////
		UpdateId++;
		if(AndroidNativeSettings.Instance.AnalyticsAPI) {
			//Nothing to do
		}


		////////////////////////
		//PushNotificationsAPI
		////////////////////////
		UpdateId++;


		AN_PropertyTemplate permission_C2D_MESSAGE_Old = Manifest.GetPropertyWithName ("permission", "com.example.gcm.permission.C2D_MESSAGE");
		if (permission_C2D_MESSAGE_Old != null) {
			Manifest.RemoveProperty(permission_C2D_MESSAGE_Old);
		}



		AN_PropertyTemplate GcmBroadcastReceiver = application.GetOrCreatePropertyWithName("receiver",  "com.androidnative.gcm.GcmBroadcastReceiver");
		AN_PropertyTemplate GcmIntentService = application.GetOrCreatePropertyWithName("service",  "com.androidnative.gcm.GcmIntentService");
		AN_PropertyTemplate permission_C2D_MESSAGE = Manifest.GetOrCreatePropertyWithName("permission", PlayerSettings.bundleIdentifier + ".permission.C2D_MESSAGE");

		AN_PropertyTemplate ParseBroadcastReceiver = application.GetOrCreatePropertyWithName ("receiver",  "com.parse.ParsePushBroadcastReceiver");
		
		if(AndroidNativeSettings.Instance.PushNotificationsAPI) {
			GcmBroadcastReceiver.SetValue("android:permission", "com.google.android.c2dm.permission.SEND");
			
			AN_PropertyTemplate intent_filter = GcmBroadcastReceiver.GetOrCreateIntentFilterWithName("com.google.android.c2dm.intent.RECEIVE");
			AN_PropertyTemplate category = intent_filter.GetOrCreatePropertyWithTag("category");
			category.SetValue("android:name", PlayerSettings.bundleIdentifier);

			permission_C2D_MESSAGE.SetValue("android:protectionLevel", "signature");
		} else {
			application.RemoveProperty(GcmBroadcastReceiver);
			application.RemoveProperty(GcmIntentService);
			Manifest.RemoveProperty(permission_C2D_MESSAGE);
		}

		AN_ActivityTemplate gameThriveActivity = application.GetOrCreateActivityWithName ("com.onesignal.NotificationOpenedActivity");
		AN_PropertyTemplate gameThriveService = application.GetOrCreatePropertyWithName("service", "com.onesignal.GcmIntentService");
		AN_PropertyTemplate gameThriveReceiver = application.GetOrCreatePropertyWithName ("receiver", "com.onesignal.GcmBroadcastReceiver");
		if (AndroidNativeSettings.Instance.UseGameThrivePushNotifications) {
			FileStaticAPI.CopyFile(PluginsInstalationUtil.ANDROID_SOURCE_PATH + "OneSignalSDK.txt",
			                       PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "OneSignalSDK.jar");

			gameThriveReceiver.SetValue("android:permission", "com.google.android.c2dm.permission.SEND");
			AN_PropertyTemplate gameThriveIntentFilter = gameThriveReceiver.GetOrCreateIntentFilterWithName("com.google.android.c2dm.intent.RECEIVE");
			gameThriveIntentFilter.GetOrCreatePropertyWithName("category", PlayerSettings.bundleIdentifier);

			//Remove GcmBroadcastReceiver from AndroidManifest if it exists
			AN_PropertyTemplate property = application.GetOrCreatePropertyWithName("receiver",  "com.androidnative.gcm.GcmBroadcastReceiver");
			application.RemoveProperty(property);
			//Remove GcmIntentService from AndroidManifest if it exists
			property = application.GetOrCreatePropertyWithName("service", "com.androidnative.gcm.GcmIntentService");
			application.RemoveProperty(property);
		} else {
			FileStaticAPI.DeleteFile(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "OneSignalSDK.jar");

			application.RemoveActivity(gameThriveActivity);
			application.RemoveProperty(gameThriveService);
			//application.RemoveProperty(gameThriveReceiver);
		}

		if (AndroidNativeSettings.Instance.UseParsePushNotifications) {
			ParseBroadcastReceiver.SetValue("android:exported", "false");
			
			AN_PropertyTemplate parseIntentFilter = ParseBroadcastReceiver.GetOrCreateIntentFilterWithName("com.parse.push.intent.RECEIVE");
			parseIntentFilter.GetOrCreatePropertyWithName("action", "com.parse.push.intent.DELETE");
			parseIntentFilter.GetOrCreatePropertyWithName("action", "com.parse.push.intent.OPEN");
		} else {
			application.RemoveProperty(ParseBroadcastReceiver);
		}



		////////////////////////
		//In App Purchases API
		////////////////////////

		AN_ActivityTemplate BillingProxyActivity = application.GetOrCreateActivityWithName("com.androidnative.billing.core.AN_BillingProxyActivity");
		if(AndroidNativeSettings.Instance.InAppPurchasesAPI) {

			BillingProxyActivity.SetValue("android:launchMode", "singleTask");
			BillingProxyActivity.SetValue("android:label", "@string/app_name");
			BillingProxyActivity.SetValue("android:configChanges", "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
			BillingProxyActivity.SetValue("android:theme", "@android:style/Theme.Translucent.NoTitleBar");
		} else {
			application.RemoveActivity(BillingProxyActivity);
		}



		AN_ActivityTemplate GP_ProxyActivity = application.GetOrCreateActivityWithName("com.androidnative.gms.core.GooglePlaySupportActivity");
		if(AndroidNativeSettings.Instance.EnablePSAPI) {
			GP_ProxyActivity.SetValue("android:launchMode", "singleTask");
			GP_ProxyActivity.SetValue("android:label", "@string/app_name");
			GP_ProxyActivity.SetValue("android:configChanges", "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
			GP_ProxyActivity.SetValue("android:theme", "@android:style/Theme.Translucent.NoTitleBar");
		} else {
			application.RemoveActivity(GP_ProxyActivity);
		}



		////////////////////////
		//GoogleMobileAdAPI
		////////////////////////
		UpdateId++;
		AN_ActivityTemplate AdActivity = application.GetOrCreateActivityWithName("com.google.android.gms.ads.AdActivity");



		if(AndroidNativeSettings.Instance.GoogleMobileAdAPI) {
			if(launcherActivity != null) {
				AN_PropertyTemplate ForwardNativeEventsToDalvik = launcherActivity.GetOrCreatePropertyWithName("meta-data",  "unityplayer.ForwardNativeEventsToDalvik");

#if !(UNITY_4_0	|| UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
				ForwardNativeEventsToDalvik.SetValue("android:value", "false");
#else
				ForwardNativeEventsToDalvik.SetValue("android:value", "true");
#endif
			}

			AdActivity.SetValue("android:configChanges", "keyboard|keyboardHidden|orientation|screenLayout|uiMode|screenSize|smallestScreenSize");
		} else {
			application.RemoveActivity(AdActivity);
		}



		////////////////////////
		//GoogleButtonAPI
		////////////////////////
		UpdateId++;
		if(AndroidNativeSettings.Instance.GoogleButtonAPI) {
			//Nothing to do
		} 



		////////////////////////
		//LocalNotificationReceiver
		////////////////////////
		AN_PropertyTemplate LocalNotificationReceiver = application.GetOrCreatePropertyWithName("receiver",  "com.androidnative.features.notifications.LocalNotificationReceiver");
		if(AndroidNativeSettings.Instance.LocalNotificationsAPI) {
		
		} else {
			application.RemoveProperty(LocalNotificationReceiver);
		}

		////////////////////////
		//ImmersiveModeAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.ImmersiveModeAPI) {
			//Nothing to do
		}


		////////////////////////
		//ApplicationInformationAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.ApplicationInformationAPI) {
			//Nothing to do
		}

		////////////////////////
		//ExternalAppsAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.ExternalAppsAPI) {
			//Nothing to do
		}


		////////////////////////
		//PoupsandPreloadersAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.PoupsandPreloadersAPI) {
			//Nothing to do
		}


		////////////////////////
		//CameraAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.CameraAPI) {
			//Nothing to do
		}


		////////////////////////
		//GalleryAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.GalleryAPI) {
			//Nothing to do
		}

		List<string> permissions = GetRequiredPermissions();
		foreach(string p in permissions) {
			Manifest.AddPermission(p);
		}

		////////////////////////
		//Check for C2D_MESSAGE <permission> duplicates
		////////////////////////
		bool duplicated = true;
		while (duplicated) {
			duplicated = false;
			List<AN_PropertyTemplate> properties = Manifest.Properties["permission"];
			foreach (AN_PropertyTemplate permission in properties) {
				if (permission.Name.EndsWith(".permission.C2D_MESSAGE")
				    && !permission.Name.Equals(PlayerSettings.bundleIdentifier + ".permission.C2D_MESSAGE")) {
					properties.Remove(permission);
					duplicated = true;
					break;
				}
			}
		}

		////////////////////////
		//Check for C2D_MESSAGE <permission> <uses-permission> duplicates
		////////////////////////
		duplicated = true;
		while (duplicated) {
			duplicated = false;
			List<AN_PropertyTemplate> properties = Manifest.Permissions;
			foreach (AN_PropertyTemplate permission in properties) {
				if (permission.Name.EndsWith(".permission.C2D_MESSAGE")
				    && !permission.Name.Equals(PlayerSettings.bundleIdentifier + ".permission.C2D_MESSAGE")) {
					properties.Remove(permission);
					duplicated = true;
					break;
				}
			}
		}

		AN_ManifestManager.SaveManifest();

		SocialPlatfromSettingsEditor.UpdateManifest();
	}


	private static List<string> GetRequiredPermissions() {
		List<string> permissions =  new List<string>();
		permissions.Add("android.permission.INTERNET");


		if(AndroidNativeSettings.Instance.AnalyticsAPI) {
			permissions.Add("android.permission.ACCESS_NETWORK_STATE");
		}

		if(AndroidNativeSettings.Instance.InAppPurchasesAPI) {
			permissions.Add("com.android.vending.BILLING");
		}

		if(AndroidNativeSettings.Instance.PushNotificationsAPI) {
			permissions.Add("com.google.android.c2dm.permission.RECEIVE");
			permissions.Add(PlayerSettings.bundleIdentifier + ".permission.C2D_MESSAGE");
			permissions.Add("android.permission.WAKE_LOCK");
		}

		if(AndroidNativeSettings.Instance.LocalNotificationsAPI || AndroidNativeSettings.Instance.PushNotificationsAPI) {
			permissions.Add("android.permission.VIBRATE");
			permissions.Add("android.permission.GET_TASKS");
		}



		if(SocialPlatfromSettings.Instance.EnableImageSharing) {
			permissions.Add("android.permission.WRITE_EXTERNAL_STORAGE");
		}

		if(AndroidNativeSettings.Instance.PlayServicesAdvancedSignInAPI) {
			permissions.Add("android.permission.GET_ACCOUNTS");
		}

		if (AndroidNativeSettings.Instance.CheckAppLicenseAPI) {
			permissions.Add("com.android.vending.CHECK_LICENSE");
		}

		if (AndroidNativeSettings.Instance.NetworkStateAPI) {
			permissions.Add("android.permission.ACCESS_WIFI_STATE");
		}

		return permissions;
	}

	private bool IsDigitsOnly(string str) {
		foreach (char c in str) {
			if (!char.IsDigit(c)) {
				return false;
			}
		}

		return true;
	}

	private static void UpdateAppID() {
		if (!FileStaticAPI.IsFolderExists("Plugins/Android/res/values")) {
			EditorGUILayout.HelpBox("Android resource folder DOESN'T exist", MessageType.Warning);
		} else {
			if (!FileStaticAPI.IsFileExists ("Plugins/Android/res/values/ids.xml")) {
				EditorGUILayout.HelpBox("XML file with PlayService ID's DOESN'T exist", MessageType.Warning);
			} else {
				//Parse XML file with PlayService Settings ID's
				XmlDocument doc = new XmlDocument();
				doc.Load(Application.dataPath + "/Plugins/Android/res/values/ids.xml");
				
				bool bAppIdNodeExists = false;
				string appId = string.Empty;
				XmlNode rootResourcesNode = doc.DocumentElement;
				
				List<XmlNode> resources = new List<XmlNode>();
				foreach(XmlNode chn in rootResourcesNode.ChildNodes) {
					if (chn.Name.Equals("string")) {
						if (chn.Attributes["name"] != null) {
							if (chn.Attributes["name"].Value.Equals("app_id")) {
								bAppIdNodeExists = true;
								appId = chn.InnerText;
							} else {
								resources.Add(chn);
							}
						}
					}
				}
				
				if (bAppIdNodeExists) {
					//Save AppID to manifest file, if it has been changed
					if (!appId.Equals(AndroidNativeSettings.Instance.GooglePlayServiceAppID)) {
						AndroidNativeSettings.Instance.GooglePlayServiceAppID = appId;
						
						AN_ManifestManager.Refresh();
						AN_ManifestTemplate manifest = AN_ManifestManager.GetManifest();
						AN_ApplicationTemplate application = manifest.ApplicationTemplate;
						
						AN_PropertyTemplate property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.games.APP_ID");
						property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);
						property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.version");
						property.SetValue("android:value", AndroidNativeSettings.GOOGLE_PLAY_SDK_VERSION_NUMBER);
						property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.appstate.APP_ID");
						property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);
						AN_ManifestManager.SaveManifest();
					}
				}
			}
		}
	}

	private void PlayServiceDrawXmlIDs() {
		if (!FileStaticAPI.IsFolderExists("Plugins/Android/res/values")) {
			EditorGUILayout.HelpBox("Android resource folder DOESN'T exist", MessageType.Warning);
		} else {
			if (!FileStaticAPI.IsFileExists ("Plugins/Android/res/values/ids.xml")) {
				EditorGUILayout.HelpBox("XML file with PlayService ID's DOESN'T exist", MessageType.Warning);
			} else {
				//Parse XML file with PlayService Settings ID's
				XmlDocument doc = new XmlDocument();
				doc.Load(Application.dataPath + "/Plugins/Android/res/values/ids.xml");

				bool bAppIdNodeExists = false;
				string appId = string.Empty;
				XmlNode rootResourcesNode = doc.DocumentElement;

				List<XmlNode> resources = new List<XmlNode>();
				foreach(XmlNode chn in rootResourcesNode.ChildNodes) {
					if (chn.Name.Equals("string")) {
						if (chn.Attributes["name"] != null) {
							if (chn.Attributes["name"].Value.Equals("app_id")) {
								bAppIdNodeExists = true;
								appId = chn.InnerText;

								EditorGUILayout.BeginHorizontal();
								GUI.enabled = true;
								EditorGUILayout.LabelField("App ID:");
								GUI.enabled = false;
								EditorGUILayout.TextField(chn.InnerText);
								EditorGUILayout.EndHorizontal();
							} else {
								resources.Add(chn);
							}
						}
					}
				}

				if (!bAppIdNodeExists) {
					//Warning in Inspector window if there is NO AppID info in XML file
					EditorGUILayout.HelpBox("XML file with DOESN'T contain information for App ID", MessageType.Warning);
				} else {
					//Save AppID to manifest file, if it has been changed
					if (!appId.Equals(AndroidNativeSettings.Instance.GooglePlayServiceAppID)) {
						AndroidNativeSettings.Instance.GooglePlayServiceAppID = appId;
						
						AN_ManifestManager.Refresh();
						AN_ManifestTemplate manifest = AN_ManifestManager.GetManifest();
						AN_ApplicationTemplate application = manifest.ApplicationTemplate;
						
						AN_PropertyTemplate property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.games.APP_ID");
						property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);
						property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.version");
						property.SetValue("android:value", AndroidNativeSettings.GOOGLE_PLAY_SDK_VERSION_NUMBER);
						property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.appstate.APP_ID");
						property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);
						AN_ManifestManager.SaveManifest();
					}
				}

				EditorGUI.indentLevel++;
				if (resources.Count > 0) {
					GUI.enabled = true;
					AndroidNativeSettings.Instance.ShowPSSettingsResources = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowPSSettingsResources, "Resources IDs");

					if (AndroidNativeSettings.Instance.ShowPSSettingsResources) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Name", EditorStyles.boldLabel);
						EditorGUILayout.Space();
						EditorGUILayout.LabelField("ID", EditorStyles.boldLabel, GUILayout.Width(170.0f));
						EditorGUILayout.EndHorizontal();
					}
					GUI.enabled = false;
				}

				if (AndroidNativeSettings.Instance.ShowPSSettingsResources) {
					foreach (XmlNode r in resources) {
						EditorGUILayout.BeginHorizontal();
						GUI.enabled = true;
						EditorGUILayout.LabelField(r.Attributes["name"].Value);
						GUI.enabled = false;
						EditorGUILayout.TextField(r.InnerText, GUILayout.Width(170.0f));
						EditorGUILayout.EndHorizontal();
					}

					GUI.enabled = true;
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.Space ();
					if (GUILayout.Button("[?] How to GET Resources?", GUILayout.Width(200.0f))) {
						Application.OpenURL("https://unionassets.com/android-native-plugin/get-playservice-settings-resources-284");
					}
					EditorGUILayout.Space ();
					EditorGUILayout.EndHorizontal ();
				}
				EditorGUI.indentLevel--;
			}
		}

		GUI.enabled = true;
	}

	GUIContent LeaderboardIdDLabel 		= new GUIContent("LeaderboardId[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
	GUIContent LeaderboardNameLabel  	= new GUIContent("Display Name[?]:", "This is the name of the Leaderboard that will be seen by customers (if this is their primary language). For automatically renewable subscriptions, don’t include a duration in the display name. The display name can’t be longer than 75 characters.");
	GUIContent LeaderboardDescriptionLabel 	= new GUIContent("Description[?]:", "This is the description of the Leaderboard. The description cannot be longer than 255 bytes.");

	GUIContent AchievementIdDLabel 		= new GUIContent("AchievementId[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
	GUIContent AchievementNameLabel  	= new GUIContent("Display Name[?]:", "This is the name of the Achievement that will be seen by customers (if this is their primary language). For automatically renewable subscriptions, don’t include a duration in the display name. The display name can’t be longer than 75 characters.");
	GUIContent AchievementDescriptionLabel 	= new GUIContent("Description[?]:", "This is the description of the Achievement. The description cannot be longer than 255 bytes.");

	private void PlayServiceSettings() {

		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Play Service API Settings", MessageType.None);


		PlayServiceDrawXmlIDs();
		EditorGUILayout.Space();

		EditorGUI.indentLevel++;
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			
			EditorGUILayout.BeginHorizontal();
			AndroidNativeSettings.Instance.ShowLeaderboards = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowLeaderboards, "Leaderboards");
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
			if(AndroidNativeSettings.Instance.ShowLeaderboards) {
				
				foreach(GPLeaderBoard leaderboard in AndroidNativeSettings.Instance.Leaderboards) {
					
					EditorGUILayout.BeginVertical (GUI.skin.box);
					
					EditorGUILayout.BeginHorizontal();
					
					GUIStyle s =  new GUIStyle();
					s.padding =  new RectOffset();
					s.margin =  new RectOffset();
					s.border =  new RectOffset();
					
					if(leaderboard.Texture != null) {
						GUILayout.Box(leaderboard.Texture, s, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)});
					}
					
					leaderboard.IsOpen 	= EditorGUILayout.Foldout(leaderboard.IsOpen, leaderboard.Name);
					
					bool ItemWasRemoved = DrawSortingButtons((object) leaderboard, AndroidNativeSettings.Instance.Leaderboards);
					if(ItemWasRemoved) {
						return;
					}
					
					EditorGUILayout.EndHorizontal();
					
					if(leaderboard.IsOpen) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(LeaderboardIdDLabel);
						leaderboard.Id	 	= EditorGUILayout.TextField(leaderboard.Id);
						if(leaderboard.Id.Length > 0) {
							leaderboard.Id 		= leaderboard.Id.Trim();
						}
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(LeaderboardNameLabel);
						leaderboard.Name	 	= EditorGUILayout.TextField(leaderboard.Name);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.Space();
						EditorGUILayout.Space();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(LeaderboardDescriptionLabel);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						leaderboard.Description	 = EditorGUILayout.TextArea(leaderboard.Description,  new GUILayoutOption[]{GUILayout.Height(60), GUILayout.Width(200)} );
						leaderboard.Texture = (Texture2D) EditorGUILayout.ObjectField("", leaderboard.Texture, typeof (Texture2D), false);
						EditorGUILayout.EndHorizontal();
					}
					
					EditorGUILayout.EndVertical();
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
					GPLeaderBoard leaderboard =  new GPLeaderBoard(string.Empty, "New Leaderboard");
					AndroidNativeSettings.Instance.Leaderboards.Add(leaderboard);
				}
				
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			
			EditorGUILayout.EndVertical();
		}
		EditorGUI.indentLevel--;
		
		EditorGUI.indentLevel++;
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			
			EditorGUILayout.BeginHorizontal();
			AndroidNativeSettings.Instance.ShowAchievements = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowAchievements, "Achievements");
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
			if(AndroidNativeSettings.Instance.ShowAchievements) {
				
				foreach(GPAchievement achievement in AndroidNativeSettings.Instance.Achievements) {
					
					EditorGUILayout.BeginVertical (GUI.skin.box);
					
					EditorGUILayout.BeginHorizontal();
					
					GUIStyle s =  new GUIStyle();
					s.padding =  new RectOffset();
					s.margin =  new RectOffset();
					s.border =  new RectOffset();
					
					if(achievement.Texture != null) {
						GUILayout.Box(achievement.Texture, s, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)});
					}
					
					achievement.IsOpen 	= EditorGUILayout.Foldout(achievement.IsOpen, achievement.Name);
					
					bool ItemWasRemoved = DrawSortingButtons((object) achievement, AndroidNativeSettings.Instance.Achievements);
					if(ItemWasRemoved) {
						return;
					}
					
					EditorGUILayout.EndHorizontal();
					
					if(achievement.IsOpen) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(AchievementIdDLabel);
						achievement.Id	 	= EditorGUILayout.TextField(achievement.Id);
						if(achievement.Id.Length > 0) {
							achievement.Id 		= achievement.Id.Trim();
						}
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(AchievementNameLabel);
						achievement.Name	 	= EditorGUILayout.TextField(achievement.Name);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.Space();
						EditorGUILayout.Space();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(AchievementDescriptionLabel);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						achievement.Description	 = EditorGUILayout.TextArea(achievement.Description,  new GUILayoutOption[]{GUILayout.Height(60), GUILayout.Width(200)} );
						achievement.Texture = (Texture2D) EditorGUILayout.ObjectField("", achievement.Texture, typeof (Texture2D), false);
						EditorGUILayout.EndHorizontal();
					}
					
					EditorGUILayout.EndVertical();
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
					GPAchievement achievement =  new GPAchievement(string.Empty, "New Achievement");
					AndroidNativeSettings.Instance.Achievements.Add(achievement);
				}
				
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			
			EditorGUILayout.EndVertical();
		}
		EditorGUI.indentLevel--;






			

			EditorGUILayout.LabelField("API:", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(PlusApiLabel);
			settings.EnablePlusAPI	 	= EditorGUILayout.Toggle(settings.EnablePlusAPI);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(GamesApiLabel);
			settings.EnableGamesAPI	 	= EditorGUILayout.Toggle(settings.EnableGamesAPI);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(DriveApiLabel);
			settings.EnableDriveAPI	 	= EditorGUILayout.Toggle(settings.EnableDriveAPI);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(AppSateApiLabel);
			settings.EnableAppStateAPI	 	= EditorGUILayout.Toggle(settings.EnableAppStateAPI);
			EditorGUILayout.EndHorizontal();




			EditorGUI.indentLevel--;


			EditorGUILayout.LabelField("Auto Image Loading:", EditorStyles.boldLabel);

			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Profile Icons");
			settings.LoadProfileIcons	 	= EditorGUILayout.Toggle(settings.LoadProfileIcons);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Profile Hi-res Images");
			settings.LoadProfileImages	 	= EditorGUILayout.Toggle(settings.LoadProfileImages);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Event Icons");
			settings.LoadEventsIcons	 	= EditorGUILayout.Toggle(settings.LoadEventsIcons);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Quest Icons");
			settings.LoadQuestsIcons	 	= EditorGUILayout.Toggle(settings.LoadQuestsIcons);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Quest Banners");
			settings.LoadQuestsImages	 	= EditorGUILayout.Toggle(settings.LoadQuestsImages);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;

			EditorGUILayout.LabelField("Extras:", EditorStyles.boldLabel);

			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Show Connecting Popup");
			settings.ShowConnectingPopup	= EditorGUILayout.Toggle(settings.ShowConnectingPopup);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;

			

		

	}

	GUIContent ProductIdDLabel 		= new GUIContent("ProductId[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
	GUIContent IsConsLabel 			= new GUIContent("Is Consumable[?]:", "Is prodcut allowed to be purchased more than once?");
	GUIContent DisplayNameLabel  	= new GUIContent("Display Name[?]:", "This is the name of the In-App Purchase that will be seen by customers (if this is their primary language). For automatically renewable subscriptions, don’t include a duration in the display name. The display name can’t be longer than 75 characters.");
	GUIContent DescriptionLabel 	= new GUIContent("Description[?]:", "This is the description of the In-App Purchase that will be used by App Review during the review process. If indicated in your code, this description may also be seen by customers. For automatically renewable subscriptions, do not include a duration in the description. The description cannot be longer than 255 bytes.");

	private void BillingSettings() {
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Billing Settings", MessageType.None);


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(Base64KeyLabel);
			settings.base64EncodedPublicKey	 	= EditorGUILayout.TextField(settings.base64EncodedPublicKey);

			if(settings.base64EncodedPublicKey.ToString().Length > 0) {
				settings.base64EncodedPublicKey 	= settings.base64EncodedPublicKey.ToString().Trim();
			}

			EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel++;
			{
				EditorGUILayout.BeginVertical (GUI.skin.box);
				
				
				EditorGUILayout.BeginHorizontal();
				AndroidNativeSettings.Instance.ShowStoreProducts = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowStoreProducts, "Products");
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if(AndroidNativeSettings.Instance.ShowStoreProducts) {
					
					foreach(GoogleProductTemplate product in AndroidNativeSettings.Instance.InAppProducts) {
						
						EditorGUILayout.BeginVertical (GUI.skin.box);
						
						EditorGUILayout.BeginHorizontal();
						
						GUIStyle s =  new GUIStyle();
						s.padding =  new RectOffset();
						s.margin =  new RectOffset();
						s.border =  new RectOffset();
						
						if(product.Texture != null) {
							GUILayout.Box(product.Texture, s, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)});
						}
						
						product.IsOpen 	= EditorGUILayout.Foldout(product.IsOpen, product.Title);

						
						EditorGUILayout.LabelField(product.Price + "$");
						bool ItemWasRemoved = DrawSortingButtons((object) product, AndroidNativeSettings.Instance.InAppProducts);
						if(ItemWasRemoved) {
							return;
						}
						
						EditorGUILayout.EndHorizontal();
						
						if(product.IsOpen) {
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField(ProductIdDLabel);
							product.SKU	 	= EditorGUILayout.TextField(product.SKU);
							if(product.SKU.Length > 0) {
								product.SKU 		= product.SKU.Trim();
							}
							EditorGUILayout.EndHorizontal();
							
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField(DisplayNameLabel);
							product.Title	 	= EditorGUILayout.TextField(product.Title);
							EditorGUILayout.EndHorizontal();
							
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField(IsConsLabel);
							product.ProductType	 	= (AN_InAppType) EditorGUILayout.EnumPopup(product.ProductType);
							EditorGUILayout.EndHorizontal();
							
							EditorGUILayout.Space();
							EditorGUILayout.Space();

							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField(DescriptionLabel);
							EditorGUILayout.EndHorizontal();
							
							EditorGUILayout.BeginHorizontal();
							product.Description	 = EditorGUILayout.TextArea(product.Description,  new GUILayoutOption[]{GUILayout.Height(60), GUILayout.Width(200)} );
							product.Texture = (Texture2D) EditorGUILayout.ObjectField("", product.Texture, typeof (Texture2D), false);
							EditorGUILayout.EndHorizontal();
						}

						
						EditorGUILayout.EndVertical();
						
					}
					
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space();
					if(GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
						GoogleProductTemplate product =  new GoogleProductTemplate();
						AndroidNativeSettings.Instance.InAppProducts.Add(product);
					}
					
					EditorGUILayout.Space();
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space();
				}
				
				EditorGUILayout.EndVertical();
			}

			EditorGUI.indentLevel--;

	}

	private bool DrawSortingButtons(object currentObject, IList ObjectsList) {
		
		int ObjectIndex = ObjectsList.IndexOf(currentObject);
		if(ObjectIndex == 0) {
			GUI.enabled = false;
		} 
		
		bool up 		= GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20));
		if(up) {
			object c = currentObject;
			ObjectsList[ObjectIndex]  		= ObjectsList[ObjectIndex - 1];
			ObjectsList[ObjectIndex - 1] 	=  c;
		}
		
		
		if(ObjectIndex >= ObjectsList.Count -1) {
			GUI.enabled = false;
		} else {
			GUI.enabled = true;
		}
		
		bool down 		= GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20));
		if(down) {
			object c = currentObject;
			ObjectsList[ObjectIndex] =  ObjectsList[ObjectIndex + 1];
			ObjectsList[ObjectIndex + 1] = c;
		}
		
		
		GUI.enabled = true;
		bool r 			= GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20));
		if(r) {
			ObjectsList.Remove(currentObject);
		}
		
		return r;
	}
	
	private void NotificationsSettings() {
		EditorGUILayout.Space ();

		EditorGUILayout.HelpBox("Local Notifications", MessageType.None);
		LocalNotificationParams();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.HelpBox("Push Notifications", MessageType.None);
		PushNotificationParams();

	}



	public void ThirdPartyParams() {

		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Third-Party Plug-Ins Support Seettings", MessageType.None);

		EditorGUI.BeginChangeCheck ();



		EditorGUILayout.LabelField ("Anti-Cheat Toolkit", EditorStyles.boldLabel);

		EditorGUI.indentLevel++; {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Anti-Cheat Toolkit Support");
			AndroidNativeSettings.Instance.EnableATCSupport = EditorGUILayout.Toggle ("", AndroidNativeSettings.Instance.EnableATCSupport);
			
			
			EditorGUILayout.Space ();
			EditorGUILayout.EndHorizontal ();
			
			if(EditorGUI.EndChangeCheck()) {
				UpdatePluginSettings();
			}
			
			
			
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space ();
			if (GUILayout.Button("[?] Read More", GUILayout.Width(100.0f))) {
				Application.OpenURL("http://goo.gl/dokdpv");
			}
			
			EditorGUILayout.EndHorizontal ();
		} EditorGUI.indentLevel--;


		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Soomla Configuration", EditorStyles.boldLabel);
		
		EditorGUI.indentLevel++; {
			
			
			EditorGUI.BeginChangeCheck(); 
			bool prevSoomlaState = AndroidNativeSettings.Instance.EnableSoomla;
			AndroidNativeSettings.Instance.EnableSoomla = ToggleFiled("Enable GROW", AndroidNativeSettings.Instance.EnableSoomla);
			if(EditorGUI.EndChangeCheck())  {
				
				if(!AndroidNativeSettings.Instance.EnableSoomla) {
					PluginsInstalationUtil.DisableSoomlaAPI();
				} else {
					
					if(FileStaticAPI.IsFileExists("Plugins/IOS/libSoomlaGrowLite.a")) {
						PluginsInstalationUtil.EnableSoomlaAPI();
					} else {
						
						
						bool res = EditorUtility.DisplayDialog("Soomla Grow not found", "Android Native wasn't able to find Soomla Grow libraryes in your project. Would you like to donwload and install it?", "Download", "No Thanks");
						if(res) {
							Application.OpenURL(AndroidNativeSettings.Instance.SoomlaDownloadLink);
						}
						
						AndroidNativeSettings.Instance.EnableSoomla = false;
					}
				}
			}

			if(!prevSoomlaState && AndroidNativeSettings.Instance.EnableSoomla) {
				bool res = EditorUtility.DisplayDialog("Soomla Grow", "Make sure you initialize SoomlaGrow when your games starts: \nAN_SoomlaGrow.Init();", "Documentation", "Got it");
				if(res) {
					Application.OpenURL(AndroidNativeSettings.Instance.SoomlaDocsLink);
				}
			}


			
			
			GUI.enabled = AndroidNativeSettings.Instance.EnableSoomla;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Game Key");
			AndroidNativeSettings.Instance.SoomlaGameKey =  EditorGUILayout.TextField(AndroidNativeSettings.Instance.SoomlaGameKey);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Env Key");
			AndroidNativeSettings.Instance.SoomlaEnvKey =  EditorGUILayout.TextField(AndroidNativeSettings.Instance.SoomlaEnvKey);
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;
			
		}EditorGUI.indentLevel--;

	}

	public static void LocalNotificationParams() {
		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Show when App is foreground");
		AndroidNativeSettings.Instance.ShowWhenAppIsForeground = EditorGUILayout.Toggle ("", AndroidNativeSettings.Instance.ShowWhenAppIsForeground);
		EditorGUILayout.EndHorizontal ();

		AndroidNativeSettings.Instance.EnableVibrationLocal = EditorGUILayout.Toggle ("Enable Vibration", AndroidNativeSettings.Instance.EnableVibrationLocal);

		Texture2D icon = (Texture2D)EditorGUILayout.ObjectField ("Local Notification Icon", AndroidNativeSettings.Instance.LocalNotificationIcon, typeof(Texture2D), false);
		if (EditorGUI.EndChangeCheck ()) {
			if (AndroidNativeSettings.Instance.LocalNotificationIcon != null) {
				string path = AssetDatabase.GetAssetPath(AndroidNativeSettings.Instance.LocalNotificationIcon);
				if (AndroidNativeSettings.Instance.PushNotificationIcon != null) {
					if (!AndroidNativeSettings.Instance.PushNotificationIcon.name.Equals(AndroidNativeSettings.Instance.LocalNotificationIcon.name)) {
						FileStaticAPI.DeleteFile("Plugins/Android/res/drawable/" + AndroidNativeSettings.Instance.LocalNotificationIcon.name.ToLower() + Path.GetExtension(path));
					}
				} else {
					FileStaticAPI.DeleteFile("Plugins/Android/res/drawable/" + AndroidNativeSettings.Instance.LocalNotificationIcon.name.ToLower() + Path.GetExtension(path));
				}
			}
			
			if (icon != null) {
				string path = AssetDatabase.GetAssetPath(icon);
				FileStaticAPI.CopyFile(path.Substring(path.IndexOf("/"), path.Length - path.IndexOf("/")),
				                       "Plugins/Android/res/drawable/" + icon.name.ToLower() + Path.GetExtension(path));
			}
			AndroidNativeSettings.Instance.LocalNotificationIcon = icon;
		}
		
		EditorGUILayout.Space ();
		EditorGUI.BeginChangeCheck ();
		AudioClip sound = (AudioClip)EditorGUILayout.ObjectField ("Local Notification Sound", AndroidNativeSettings.Instance.LocalNotificationSound, typeof(AudioClip), false);
		if (EditorGUI.EndChangeCheck ()) {
			if (AndroidNativeSettings.Instance.LocalNotificationSound != null) {
				string path = AssetDatabase.GetAssetPath(AndroidNativeSettings.Instance.LocalNotificationSound);
				if (AndroidNativeSettings.Instance.PushNotificationSound != null) {
					if (!AndroidNativeSettings.Instance.PushNotificationSound.name.Equals(AndroidNativeSettings.Instance.LocalNotificationSound.name)) {
						FileStaticAPI.DeleteFile("Plugins/Android/res/raw/" + AndroidNativeSettings.Instance.LocalNotificationSound.name.ToLower() + Path.GetExtension(path));
					}
				} else {
					FileStaticAPI.DeleteFile("Plugins/Android/res/raw/" + AndroidNativeSettings.Instance.LocalNotificationSound.name.ToLower() + Path.GetExtension(path));
				}
			}
			
			if (sound != null) {
				string path = AssetDatabase.GetAssetPath(sound);
				FileStaticAPI.CopyFile(path.Substring(path.IndexOf("/"), path.Length - path.IndexOf("/")),
				                       "Plugins/Android/res/raw/" + sound.name.ToLower() + Path.GetExtension(path));
			}
			AndroidNativeSettings.Instance.LocalNotificationSound = sound;
		}
	}

	public static void PushNotificationParams() {
		EditorGUI.BeginChangeCheck ();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Sender Id");
		AndroidNativeSettings.Instance.GCM_SenderId	 	= EditorGUILayout.TextField(AndroidNativeSettings.Instance.GCM_SenderId);
		if(AndroidNativeSettings.Instance.GCM_SenderId.Length > 0) {
			AndroidNativeSettings.Instance.GCM_SenderId		= AndroidNativeSettings.Instance.GCM_SenderId.Trim();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Show when App is foreground");
		AndroidNativeSettings.Instance.ShowPushWhenAppIsForeground = EditorGUILayout.Toggle ("", AndroidNativeSettings.Instance.ShowPushWhenAppIsForeground);
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Replace old notification with new one");
		AndroidNativeSettings.Instance.ReplaceOldNotificationWithNew = EditorGUILayout.Toggle ("", AndroidNativeSettings.Instance.ReplaceOldNotificationWithNew);
		EditorGUILayout.EndHorizontal ();

		AndroidNativeSettings.Instance.EnableVibrationPush = EditorGUILayout.Toggle ("Enable Vibration", AndroidNativeSettings.Instance.EnableVibrationPush);

		Texture2D icon = (Texture2D)EditorGUILayout.ObjectField ("Push Notification Icon", AndroidNativeSettings.Instance.PushNotificationIcon, typeof(Texture2D), false);
		if (EditorGUI.EndChangeCheck ()) {
			if (AndroidNativeSettings.Instance.PushNotificationIcon != null) {
				string path = AssetDatabase.GetAssetPath(AndroidNativeSettings.Instance.PushNotificationIcon);
				if (AndroidNativeSettings.Instance.LocalNotificationIcon != null) {
					if (!AndroidNativeSettings.Instance.PushNotificationIcon.name.Equals(AndroidNativeSettings.Instance.LocalNotificationIcon.name)) {
						FileStaticAPI.DeleteFile("Plugins/Android/res/drawable/" + AndroidNativeSettings.Instance.PushNotificationIcon.name.ToLower() + Path.GetExtension(path));
					}
				} else {
					FileStaticAPI.DeleteFile("Plugins/Android/res/drawable/" + AndroidNativeSettings.Instance.PushNotificationIcon.name.ToLower() + Path.GetExtension(path));
				}
			}

			if (icon != null) {
				string path = AssetDatabase.GetAssetPath(icon);
				FileStaticAPI.CopyFile(path.Substring(path.IndexOf("/"), path.Length - path.IndexOf("/")),
				                       "Plugins/Android/res/drawable/" + icon.name.ToLower() + Path.GetExtension(path));
			}
			AndroidNativeSettings.Instance.PushNotificationIcon = icon;
		}

		EditorGUILayout.Space ();
		EditorGUI.BeginChangeCheck ();
		AudioClip sound = (AudioClip)EditorGUILayout.ObjectField ("Push Notification Sound", AndroidNativeSettings.Instance.PushNotificationSound, typeof(AudioClip), false);
		if (EditorGUI.EndChangeCheck ()) {
			if (AndroidNativeSettings.Instance.PushNotificationSound != null) {
				string path = AssetDatabase.GetAssetPath(AndroidNativeSettings.Instance.PushNotificationSound);
				if (AndroidNativeSettings.Instance.LocalNotificationSound != null) {
					if (!AndroidNativeSettings.Instance.PushNotificationSound.name.Equals(AndroidNativeSettings.Instance.LocalNotificationSound.name)) {
						FileStaticAPI.DeleteFile("Plugins/Android/res/raw/" + AndroidNativeSettings.Instance.PushNotificationSound.name.ToLower() + Path.GetExtension(path));
					}
				} else {
					FileStaticAPI.DeleteFile("Plugins/Android/res/raw/" + AndroidNativeSettings.Instance.PushNotificationSound.name.ToLower() + Path.GetExtension(path));
				}
			}

			if (sound != null) {
				string path = AssetDatabase.GetAssetPath(sound);
				FileStaticAPI.CopyFile(path.Substring(path.IndexOf("/"), path.Length - path.IndexOf("/")),
				                       "Plugins/Android/res/raw/" + sound.name.ToLower() + Path.GetExtension(path));
			}
			AndroidNativeSettings.Instance.PushNotificationSound = sound;
		}

		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("OneSignal Push Notifications", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Use OneSignal Push Notifications");
		
		EditorGUI.BeginChangeCheck ();
		AndroidNativeSettings.Instance.UseGameThrivePushNotifications = EditorGUILayout.Toggle (AndroidNativeSettings.Instance.UseGameThrivePushNotifications);
		if (EditorGUI.EndChangeCheck ()) {
			UpdateManifest();
		}
		
		EditorGUILayout.EndHorizontal ();
		
		if (AndroidNativeSettings.Instance.UseGameThrivePushNotifications) {
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("OneSignal App ID");
			AndroidNativeSettings.Instance.GameThriveAppID = EditorGUILayout.TextField(AndroidNativeSettings.Instance.GameThriveAppID);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space ();
			if (GUILayout.Button("[?] How To SetUp OneSignal Push Notifications?", GUILayout.Width(300.0f))) {
				Application.OpenURL("http://goo.gl/tfmbMF");
			}
			EditorGUILayout.Space ();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space ();
		}

		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Parse Push Notifications", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Use Parse Push Notifications");

		EditorGUI.BeginChangeCheck ();
		AndroidNativeSettings.Instance.UseParsePushNotifications = EditorGUILayout.Toggle (AndroidNativeSettings.Instance.UseParsePushNotifications);
		if (EditorGUI.EndChangeCheck ()) {
			UpdateManifest();
		}

		EditorGUILayout.EndHorizontal ();

		if (AndroidNativeSettings.Instance.UseParsePushNotifications) {
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Parse Application ID");
			AndroidNativeSettings.Instance.ParseAppId = EditorGUILayout.TextField(AndroidNativeSettings.Instance.ParseAppId);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Parse .NET Key");
			AndroidNativeSettings.Instance.DotNetKey = EditorGUILayout.TextField(AndroidNativeSettings.Instance.DotNetKey);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space ();
			if (GUILayout.Button("[?] How To SetUp Parse Push Notifications?", GUILayout.Width(300.0f))) {
				Application.OpenURL("http://goo.gl/9BgQ8r");
			}
			EditorGUILayout.Space ();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space ();
		}
	}

	public static void CameraAndGalleryParams() {

		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Camera and Gallery", MessageType.None);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Camera Capture Mode");
		AndroidNativeSettings.Instance.CameraCaptureMode	 	= (AN_CameraCaptureType) EditorGUILayout.EnumPopup(AndroidNativeSettings.Instance.CameraCaptureMode);
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Max Loaded Image Size");
		AndroidNativeSettings.Instance.MaxImageLoadSize	 	= EditorGUILayout.IntField(AndroidNativeSettings.Instance.MaxImageLoadSize);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Image Format");
		AndroidNativeSettings.Instance.ImageFormat = (AndroidCameraImageFormat) EditorGUILayout.EnumPopup(AndroidNativeSettings.Instance.ImageFormat);
		EditorGUILayout.EndHorizontal();
		
		GUI.enabled = !AndroidNativeSettings.Instance.UseProductNameAsFolderName;
		if(AndroidNativeSettings.Instance.UseProductNameAsFolderName) {
			if(PlayerSettings.productName.Length > 0) {
				AndroidNativeSettings.Instance.GalleryFolderName = PlayerSettings.productName.Trim();
			}


		}
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("App Gallery Folder");
		AndroidNativeSettings.Instance.GalleryFolderName	 	= EditorGUILayout.TextField(AndroidNativeSettings.Instance.GalleryFolderName);
		if(AndroidNativeSettings.Instance.GalleryFolderName.Length > 0) {
			AndroidNativeSettings.Instance.GalleryFolderName		= AndroidNativeSettings.Instance.GalleryFolderName.Trim();
			AndroidNativeSettings.Instance.GalleryFolderName		= AndroidNativeSettings.Instance.GalleryFolderName.Trim('/');
		}

		EditorGUILayout.EndHorizontal();
		
		GUI.enabled = true;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Use Product Name As Folder Name");
		AndroidNativeSettings.Instance.UseProductNameAsFolderName	 	= EditorGUILayout.Toggle(AndroidNativeSettings.Instance.UseProductNameAsFolderName);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Save Camera Image To Gallery");
		AndroidNativeSettings.Instance.SaveCameraImageToGallery	 	= EditorGUILayout.Toggle(AndroidNativeSettings.Instance.SaveCameraImageToGallery);
		EditorGUILayout.EndHorizontal();
	}




	
	
	private void AboutGUI() {

		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		
		SelectableLabelField(SdkVersion,   AndroidNativeSettings.VERSION_NUMBER);
		if(IsFacebookInstalled) {
			SelectableLabelField(FBdkVersion, SocialPlatfromSettings.FB_SDK_VERSION_NUMBER);
		}	
		SelectableLabelField(GPSdkVersion, AndroidNativeSettings.GOOGLE_PLAY_SDK_VERSION_NUMBER);



		SelectableLabelField(SupportEmail, "support@stansassets.com");
		
		
	}
	
	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}

	private void ApplaySettings() {
		if(AndroidNativeSettings.Instance.UseProductNameAsFolderName) {
			AndroidNativeSettings.Instance.GalleryFolderName = PlayerSettings.productName;
		}
	}

	public static void AN_Plugin_Update() {
		PluginsInstalationUtil.Android_UpdatePlugin();
		AndroidNativeSettingsEditor.UpdateAPIsInstalation();
	}

	private bool ToggleFiled(string titile, bool value) {
		
		AN_Bool initialValue = AN_Bool.Yes;
		if(!value) {
			initialValue = AN_Bool.No;
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(titile);
		
		initialValue = (AN_Bool) EditorGUILayout.EnumPopup(initialValue);
		if(initialValue == AN_Bool.Yes) {
			value = true;
		} else {
			value = false;
		}
		EditorGUILayout.EndHorizontal();
		
		return value;
	}

	private static void DirtyEditor() {
		#if UNITY_EDITOR
		EditorUtility.SetDirty(SocialPlatfromSettings.Instance);
		EditorUtility.SetDirty(AndroidNativeSettings.Instance);
		#endif
	}
	
	
}
