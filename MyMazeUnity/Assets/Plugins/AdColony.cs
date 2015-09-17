//=============================================================================
//  AdColony.cs
//
//  AdColony Plugin for Unity.
//
//  Copyright 2011 Jirbo, Inc.  All rights reserved.
//
//  ---------------------------------------------------------------------------
//
//  * INSTRUCTIONS *
//
//  For more information on AdColony concepts, refer to the Quick Start Guide.
//
//  GENERAL SETUP
//  1.  Copy this file into your Unity project's "Assets" folder.
//
//  2.  In the Start() method of one of your existing game scripts, write, for
//      example:
//
//        AdColony.Configure( "1.0", "app4dc1bc42a5529", "z4dc1bc79c5fc9" );
//
//      - "1.0" is your application's version (arbitrary).
//      - "app..." is your AdColony application id.
//      - "z..." is an AdColony zone id.  You need at least one and you can
//        have multiple zone id's separated by commas.
//
//  3.  You can then call any of the methods listed below, often done when you
//      start a new game or start a new level.  A zone id (such as
//      "z4dc1bc79c5fc9") is optional for every call - you can include omit it
//      entirely if you only have one zone id.
//
//        SetCustomID( custom_id:string ) -- called BEFORE calling Configure.
//        GetCustomID():string
//        IsV4VCAvailable( [zone_id] ):bool
//        IsVideoAvailable( [zone_id] ):bool
//        IsV4VCAvailable( [zone_id] ):bool
//        GetDeviceID():string
//        GetOpenUDID():string
//      GetODIN1():string (only supported by iOS)
//        GetV4VCAmount( [zone_id] ):int
//        GetV4VCName( [zone_id] ):string
//        ShowVideoAd( [zone_id] ):bool
//        ShowV4VC( popup_result:bool, [zone_id] ):bool
//        OfferV4VC( popup_result:bool, [zone_id] )
//        StatusForZone( zone_id:string ):string
//
//      NOTES
//      - The only method many apps will need is "ShowVideoAd()".  For example,
//        just write "AdColony.showVideoAd();" when starting a new game or
//        a new level.  AdColony may or may not pause your game and show a
//        video - it depends on whether an ad is available.
//
//      - Send "true" to the methods ShowV4VC() and OfferV4VC() to enable an
//        automatic pop-up window that displays the amount of virtual currency
//        earned after watching a video.
//
//      - Call OfferV4VC to enable an automatic pop-up *before* a Video
//        4 Virtual Currency plays that gives the player the option to either
//        watch the video or decline the offer.
//
//      - AdColony only works on iOS and Android devices.  Calls to AdColony
//        made from a program running in the Unity editor will have no effect.
//
//  4.  You can set delegates that are notified when a video is started,
//      when it's finished, and when a V4VC award has been recieved.  By
//      example:
//
//        public class MyGame : MonoBehavior
//        ...
//          void ShowAd()
//          {
//             AdColony.OnVideoStarted = OnVideoStarted;
//             AdColony.OnVideoFinished = OnVideoFinished;
//             AdColony.OnV4VCResult = OnV4VCResult;
//             AdColony.OnAdAvailabilityChange = OnAdAvailabilityChange;
//             AdColony.ShowVideoAd();
//          }
//
//          void OnVideoStarted()
//          {
//            Debug.Log( "Ad playing." );
//          }
//
//          void OnVideoFinished( boolean ad_shown )
//          {
//            Debug.Log( "Ad finished." );
//          }
//
//          void OnV4VCResult( bool success, string name, int amount )
//          {
//            if (success)
//            {
//              Debug.Log( "Awarded " + amount + " " + name );
//              // e.g. "Awarded 100 Gold"
//            }
//          }
//
//      Note that the OnVideoStarted delegate is only called for iOS, since
//      iOS needs a chance to pause music etc. (the game is paused for you;
//      Time.timeScale is set to 0.0 and then restored after).  Conversely
//      Unity apps are fully suspended when video playback begins on Android,
//      so there is neither the opportunity nor the need to pause the
//      game or game music.
//
//  IOS SETUP
//
//  1.  To support iOS, copy "UnityADC.mm", "AdColonyPublic.h", and
//      "libAdColony.a" into your Unity project's Assets/Plugins/iOS
//      folder.
//
//
//  ANDROID SETUP
//
//  1.  To support Android, copy "adcolony.jar", "unityadc.jar", and
//      "AndroidManifest.xml" into your Unity project's
//      Assets/Plugins/Android folder.
//
//  2.  Edit "Assets/Plugins/Android/AndroidManifest.xml" and change the
//      package name to that of your app.
//
//  3.  (Optional) If you are an experienced Android Unity user and wish to use
//      your own main Activity similar to class UnityPlayerActivity, just be
//      sure there are no calls to mUnityPlayer.quit().
//
//=============================================================================
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class AdColony : MonoBehaviour
{
  // The single instance of the AdColony component
  private static AdColony instance;

  private static void ensureInstance()
  {
    if(instance == null)
    {
      instance = FindObjectOfType( typeof(AdColony) ) as AdColony;
      if(instance == null)
      {
        instance = new GameObject("AdColony").AddComponent<AdColony>();
      }
    }
  }

  // DELEGATE TYPE SPECIFICATIONS
  // Your class can define methods matching these signatures and assign them
  // to the 'OnVideoStarted', 'OnVideoFinished', and and 'OnV4VCResult'
  // delegates as described in Step 4 of the "GENERAL SETUP" instructions
  // above.
  public delegate void VideoStartedDelegate();
  public delegate void VideoFinishedDelegate( bool ad_shown );
  public delegate void VideoFinishedWithInfoDelegate( AdColonyAd ad_shown );
  public delegate void V4VCResultDelegate( bool success, string name, int amount );
  public delegate void AdAvailabilityChangeDelegate( bool available, string zone_id );

  // DELEGATE PROPERTIES
  public static VideoStartedDelegate          OnVideoStarted;
  public static VideoFinishedDelegate         OnVideoFinished;
  public static VideoFinishedWithInfoDelegate OnVideoFinishedWithInfo;
  public static V4VCResultDelegate            OnV4VCResult;
  public static AdAvailabilityChangeDelegate  OnAdAvailabilityChange;

  //---------------------------------------------------------------------------
  //  PUBLIC INTERFACE - NON-IOS/NON-ANDROID (stub functionality)
  //---------------------------------------------------------------------------
#if (!UNITY_ANDROID && !UNITY_IPHONE) || UNITY_EDITOR
  static public void Configure( string app_version, string app_id, params string[] zone_ids )
  {
    if (configured) return;
    ensureInstance();

    Debug.LogWarning( "Note: AdColony doesn't play videos in the editor." );
    configured = true;
  }

  static public void   SetCustomID( string custom_id ) { }
  static public string GetCustomID() { return "undefined"; }
  static public bool   IsVideoAvailable() { return false; }
  static public bool   IsVideoAvailable( string zone_id ) { return false; }
  static public bool   IsV4VCAvailable() { return false; }
  static public bool   IsV4VCAvailable( string zone_id ) { return false; }
  static public string GetDeviceID() { return "undefined"; }
  static public string GetOpenUDID() { return "undefined"; }
  static public string GetODIN1() { return "undefined"; }
  static public int    GetV4VCAmount() { return 0; }
  static public int    GetV4VCAmount( string zone_id ) { return 0; }
  static public string GetV4VCName() { return "undefined"; }
  static public string GetV4VCName( string zone_id ) { return "undefined"; }
  static public bool   ShowVideoAd() { return false; }
  static public bool   ShowVideoAd( string zone_id ) { return false; }
  static public bool   ShowV4VC( bool popup_result ) { return false; }
  static public bool   ShowV4VC( bool popup_result, string zone_id ) { return false; }
  static public void   OfferV4VC( bool popup_result ) { }
  static public void   OfferV4VC( bool popup_result, string zone_id ) { }
  static public string StatusForZone( string zone_id ) { return "undefined"; }
  static public void notifyIAPComplete( string product_id, string trans_id, string currency_code = null, double price = 0.0 ) { return; }
  // static public int    GetAvailableViews( string zone_id ) { return 0; }
#endif


  //---------------------------------------------------------------------------
  //  PUBLIC INTERFACE - IOS
  //---------------------------------------------------------------------------
#if UNITY_IPHONE && !UNITY_EDITOR
  static public void SetCustomID( string custom_id ) {
    IOSSetCustomID( custom_id );
  }

  static public string GetCustomID( ) {
    return IOSGetCustomID();
  }

  static public void Configure( string app_version, string app_id, params string[] zone_ids ) {
    if (configured) {
      return;
    }

    if (app_version.Contains("version:")) {
      string[] delims = new string[] {"version:", ","};
      string[] app_version_split = app_version.Split(delims, StringSplitOptions.RemoveEmptyEntries);
      app_version = app_version_split[0];
    }

    ensureInstance();
    IOSConfigure( app_version, app_id, zone_ids.Length, zone_ids );
    configured = true;
  }

  static public bool IsVideoAvailable() {
    if ( !configured ) {
      return false;
    }
    return IOSIsVideoAvailable( null );
  }

  static public bool IsVideoAvailable( string zone_id ) {
    if ( !configured ) {
      return false;
    }
    return IOSIsVideoAvailable( zone_id );
  }

  static public bool IsV4VCAvailable() {
    if ( !configured ) {
      return false;
    }
    return IOSIsV4VCAvailable( null );
  }

  static public bool IsV4VCAvailable( string zone_id ) {
    if ( !configured ) {
      return false;
    }
    return IOSIsV4VCAvailable( zone_id );
  }

  static public string GetDeviceID() {
    if ( !configured ) {
      return "undefined";
    }
    return IOSGetDeviceID();
  }

  static public string GetOpenUDID() {
    if ( !configured ) {
      return "undefined";
    }
    return IOSGetOpenUDID();
  }

  static public string GetODIN1() {
    if ( !configured ) {
      return "undefined";
    }
    return IOSGetODIN1();
  }

  static public int GetV4VCAmount() {
    if ( !configured ) {
      return 0;
    }
    return IOSGetV4VCAmount( null );
  }

  static public int GetV4VCAmount( string zone_id ) {
    if ( !configured ) {
      return 0;
    }
    return IOSGetV4VCAmount( zone_id );
  }

  static public string GetV4VCName() {
    if ( !configured ) {
      return "undefined";
    }
    return IOSGetV4VCName( null );
  }

  static public string GetV4VCName( string zone_id ) {
    if ( !configured ) {
      return "undefined";
    }
    return IOSGetV4VCName( zone_id );
  }

  static public bool ShowVideoAd() {
    if ( !configured ) {
      return false;
    }
    return IOSShowVideoAd( null);
  }

  static public bool ShowVideoAd( string zone_id ) {
    if ( !configured ) {
      return false;
    }
    return IOSShowVideoAd( zone_id );
  }

  static public bool ShowV4VC( bool popup_result ) {
    if ( !configured ) {
      return false;
    }
    return IOSShowV4VC( popup_result, null );
  }

  static public bool ShowV4VC( bool popup_result, string zone_id ) {
    if ( !configured ) {
      return false;
    }
    return IOSShowV4VC( popup_result, zone_id );
  }

  static public void OfferV4VC( bool popup_result ) {
    if ( !configured ) {
      return;
    }
    IOSOfferV4VC( popup_result, null );
  }

  static public void OfferV4VC( bool popup_result, string zone_id ) {
    if ( !configured ) {
      return;
    }
    IOSOfferV4VC( popup_result, zone_id );
  }

  static public string StatusForZone( string zone_id ) {
    if ( !configured ) {
      return "";
    }
    return IOSStatusForZone( zone_id );
  }

  static public void notifyIAPComplete( string product_id, string trans_id, string currency_code = null, double price = 0.0, int quantity = 1 ) {
    if ( !configured ) {
      return;
    }
    IOSNotifyIAPComplete(product_id, trans_id, currency_code, price, quantity);
  }

  // static public int GetAvailableViews( string zone_id )
  // {
  //   if ( !configured ) return -1;
  //   return 0; //TODO: Add IOS version of this method
  // }
#endif

  //---------------------------------------------------------------------------
  //  PUBLIC INTERFACE - ANDROID
  //---------------------------------------------------------------------------
#if UNITY_ANDROID && !UNITY_EDITOR
  static public void Configure( string app_version, string app_id, params string[] zone_ids ) {
    if (configured) {
      return;
    }

    ensureInstance();
    AndroidConfigure( app_version, app_id, zone_ids );
  }

  static public void SetCustomID( string custom_id ) {
    AndroidSetCustomID( custom_id );
  }

  static public string GetCustomID() {
    return AndroidGetCustomID();
  }

  static public bool IsVideoAvailable() {
    if ( !configured ) {
      return false;
    }
    return AndroidIsVideoAvailable( null );
  }

  static public bool IsVideoAvailable( string zone_id ) {
    if ( !configured ) {
      return false;
    }
    return AndroidIsVideoAvailable( zone_id );
  }

  static public bool IsV4VCAvailable() {
    if ( !configured ) {
      return false;
    }
    return AndroidIsV4VCAvailable( null );
  }

  static public bool IsV4VCAvailable( string zone_id ) {
    if ( !configured ) {
      return false;
    }
    return AndroidIsV4VCAvailable( zone_id );
  }

  static public string GetDeviceID() {
    if ( !configured ) {
      return "undefined";
    }
    return AndroidGetDeviceID();
  }

  static public string GetOpenUDID() {
    if ( !configured ) {
      return "undefined";
    }
    return AndroidGetOpenUDID();
  }

  static public string GetODIN1() {
  // Not supported by AndroidSDK
  return "undefined";
  }

  static public int GetV4VCAmount() {
    if ( !configured ) {
      return 0;
    }
    return AndroidGetV4VCAmount( null );
  }

  static public int GetV4VCAmount( string zone_id ) {
    if ( !configured ) {
      return 0;
    }
    return AndroidGetV4VCAmount( zone_id );
  }

  static public string GetV4VCName() {
    if ( !configured ) {
      return "undefined";
    }
    return AndroidGetV4VCName( null );
  }

  static public string GetV4VCName( string zone_id ) {
    if ( !configured ) {
      return "undefined";
    }
    return AndroidGetV4VCName( zone_id );
  }

  static public bool ShowVideoAd() {
    if ( !configured ) {
      return false;
    }
    return AndroidShowVideoAd( null);
  }

  static public bool ShowVideoAd( string zone_id ) {
    if ( !configured ) {
      return false;
    }
    return AndroidShowVideoAd( zone_id );
  }

  static public bool ShowV4VC( bool popup_result ) {
    if ( !configured ) {
      return false;
    }
    return AndroidShowV4VC( popup_result, null );
  }

  static public bool ShowV4VC( bool popup_result, string zone_id ) {
    if ( !configured ) {
      return false;
    }
    return AndroidShowV4VC( popup_result, zone_id );
  }

  static public void OfferV4VC( bool popup_result ) {
    if ( !configured ) {
      return;
    }
    AndroidOfferV4VC( popup_result, null );
  }

  static public void OfferV4VC( bool popup_result, string zone_id ) {
    if ( !configured ) {
      return;
    }
    AndroidOfferV4VC( popup_result, zone_id );
  }

  static public string StatusForZone( string zone_id ) {
    if ( !configured ) {
      return "";
    }
    return AndroidStatusForZone( zone_id );
  }

  static public int GetAvailableViews( string zone_id ) {
    if ( !configured ) {
      return -1;
    }
    return AndroidGetAvailableViews( zone_id );
  }

  static public void notifyIAPComplete(string product_id, string trans_id, string currency_code = null, double price = 0.0 ) {
    if (!configured) {
      return;
    }
    AndroidNotifyIAPComplete(product_id, trans_id, currency_code, price );
  }
#endif

  //---------------------------------------------------------------------------
  // INTERNAL USE
  //---------------------------------------------------------------------------
  static bool configured;
  bool  was_paused;

  void Awake() {
    // Set the name to allow UnitySendMessage to find this object.
    name = "AdColony";
    // Make sure this GameObject persists across scenes
    DontDestroyOnLoad(transform.gameObject);
  }

  void OnApplicationPause() {
    was_paused = true;
  #if UNITY_ANDROID && !UNITY_EDITOR
      AndroidPause();
    #endif
  }

  void Update() {
    if (was_paused) {
      was_paused = false;
      #if UNITY_ANDROID && !UNITY_EDITOR
        AndroidResume();
      #endif
    }
  }

  public void OnAdColonyVideoStarted( string args ) {
    if (OnVideoStarted != null) {
      OnVideoStarted();
    }
  }

  public void OnAdColonyVideoFinished( string args ) {

    //ad_shown | iapenabled | engagementType | iapproductid
    string[] split_args = args.Split('|');

    Debug.Log("OnAdColonyVideoFinished Called");

    if (OnVideoFinished != null) {
      OnVideoFinished( split_args[0].Equals("true") );
    }
    if (OnVideoFinishedWithInfo != null) {
      OnVideoFinishedWithInfo(new AdColonyAd(split_args));
    }
  }

  public void OnAdColonyV4VCResult( string args ) {
    if (OnV4VCResult != null) {
      //success | amount | name
      string[] split_args = args.Split('|');
      bool success = split_args[0].Equals("true");
      int amount = int.Parse(split_args[1]);
      string name = split_args[2];
      OnV4VCResult(success, name, amount);
    }
  }

  public void OnAdColonyAdAvailabilityChange( string args ) {
    if (OnAdAvailabilityChange != null) {
      //available | zone
      string[] split_args = args.Split('|');
      OnAdAvailabilityChange( split_args[0].Equals("true"), split_args[1] );
    }
  }

  //---------------------------------------------------------------------------
  //  IOS NATIVE INTERFACE
  //---------------------------------------------------------------------------
#if UNITY_IPHONE && !UNITY_EDITOR
  [DllImport ("__Internal")]
  extern static private void IOSSetCustomID( string custom_id );
  [DllImport ("__Internal")]
  extern static private string IOSGetCustomID();
  [DllImport ("__Internal")]
  extern static private void IOSConfigure( string app_version, string app_id,
      int zone_id_count, string[] zone_ids );
  [DllImport ("__Internal")]
  extern static private bool IOSIsVideoAvailable( string zone_id );
  [DllImport ("__Internal")]
  extern static private bool IOSIsV4VCAvailable( string zone_id );
  [DllImport ("__Internal")]
  extern static private string IOSGetOpenUDID();
  [DllImport ("__Internal")]
  extern static private string IOSGetDeviceID();
  [DllImport ("__Internal")]
  extern static private string IOSGetODIN1();
  [DllImport ("__Internal")]
  extern static private int IOSGetV4VCAmount( string zone_id );
  [DllImport ("__Internal")]
  extern static private string IOSGetV4VCName( string zone_id );
  [DllImport ("__Internal")]
  extern static private bool IOSShowVideoAd( string zone_id );
  [DllImport ("__Internal")]
  extern static private bool IOSShowV4VC( bool popup_result, string zone_id );
  [DllImport ("__Internal")]
  extern static private void IOSOfferV4VC( bool popup_result, string zone_id );
  [DllImport ("__Internal")]
  extern static private string IOSStatusForZone( string zone_id );
  [DllImport ("__Internal")]
  extern static private void IOSNotifyIAPComplete(string product_id, string trans_id, string currency_code, double price, int quantity );
#endif // UNITY_IPHONE

  //---------------------------------------------------------------------------
  //  ANDROID NATIVE INTERFACE
  //---------------------------------------------------------------------------
#if UNITY_ANDROID && !UNITY_EDITOR
  static bool adr_initialized = false;
  static AndroidJavaClass class_UnityPlayer;
  static IntPtr class_UnityADC           = IntPtr.Zero;
  static IntPtr method_configure         = IntPtr.Zero;
  static IntPtr method_pause             = IntPtr.Zero;
  static IntPtr method_resume            = IntPtr.Zero;
  static IntPtr method_setCustomID       = IntPtr.Zero;
  static IntPtr method_getCustomID       = IntPtr.Zero;
  static IntPtr method_isVideoAvailable  = IntPtr.Zero;
  static IntPtr method_isV4VCAvailable   = IntPtr.Zero;
  static IntPtr method_getDeviceID       = IntPtr.Zero;
  static IntPtr method_getV4VCAmount     = IntPtr.Zero;
  static IntPtr method_getV4VCName       = IntPtr.Zero;
  static IntPtr method_showVideo         = IntPtr.Zero;
  static IntPtr method_showV4VC          = IntPtr.Zero;
  static IntPtr method_offerV4VC         = IntPtr.Zero;
  static IntPtr method_statusForZone     = IntPtr.Zero;
  static IntPtr method_getAvailableViews = IntPtr.Zero;
  static IntPtr method_notifyIAPComplete = IntPtr.Zero;

  static void AndroidInitializePlugin() {
   bool success = true;
    IntPtr local_class_UnityADC = AndroidJNI.FindClass("com/jirbo/unityadc/UnityADC");
    if (local_class_UnityADC != IntPtr.Zero) {
      class_UnityADC = AndroidJNI.NewGlobalRef( local_class_UnityADC );
      AndroidJNI.DeleteLocalRef( local_class_UnityADC );
      var local_class_AdColony = AndroidJNI.FindClass("com/jirbo/adcolony/AdColony");
      if (local_class_AdColony != IntPtr.Zero) {
        AndroidJNI.DeleteLocalRef( local_class_AdColony );
      } else {
        success = false;
      }
    } else {
      success = false;
    }

    if (success) {

      class_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
      // Get additional method IDs for later use.
      method_configure = AndroidJNI.GetStaticMethodID( class_UnityADC, "configure",
          "(Landroid/app/Activity;Ljava/lang/String;Ljava/lang/String;[Ljava/lang/String;)V" );

      method_pause = AndroidJNI.GetStaticMethodID( class_UnityADC, "pause", "(Landroid/app/Activity;)V" );
      method_resume = AndroidJNI.GetStaticMethodID( class_UnityADC, "resume", "(Landroid/app/Activity;)V" );
      method_setCustomID = AndroidJNI.GetStaticMethodID( class_UnityADC, "setCustomID", "(Ljava/lang/String;)V" );
      method_getCustomID = AndroidJNI.GetStaticMethodID( class_UnityADC, "getCustomID", "()Ljava/lang/String;" );
      method_isVideoAvailable = AndroidJNI.GetStaticMethodID( class_UnityADC, "isVideoAvailable", "(Ljava/lang/String;)Z" );
      method_isV4VCAvailable = AndroidJNI.GetStaticMethodID( class_UnityADC, "isV4VCAvailable", "(Ljava/lang/String;)Z" );
      method_getDeviceID = AndroidJNI.GetStaticMethodID( class_UnityADC, "getDeviceID", "()Ljava/lang/String;" );
      method_getV4VCAmount = AndroidJNI.GetStaticMethodID( class_UnityADC, "getV4VCAmount", "(Ljava/lang/String;)I" );
      method_getV4VCName = AndroidJNI.GetStaticMethodID( class_UnityADC, "getV4VCName", "(Ljava/lang/String;)Ljava/lang/String;" );
      method_showVideo = AndroidJNI.GetStaticMethodID( class_UnityADC, "showVideo", "(Ljava/lang/String;)Z" );
      method_showV4VC = AndroidJNI.GetStaticMethodID( class_UnityADC, "showV4VC", "(ZLjava/lang/String;)Z" );
      method_offerV4VC = AndroidJNI.GetStaticMethodID( class_UnityADC, "offerV4VC", "(ZLjava/lang/String;)V" );
      method_statusForZone = AndroidJNI.GetStaticMethodID( class_UnityADC, "statusForZone", "(Ljava/lang/String;)Ljava/lang/String;" );
      method_getAvailableViews = AndroidJNI.GetStaticMethodID( class_UnityADC, "getAvailableViews", "(Ljava/lang/String;)I" );
      method_notifyIAPComplete = AndroidJNI.GetStaticMethodID( class_UnityADC, "notifyIAPComplete", "(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;D)V");

      adr_initialized = true;
    } else {
      // adcolony.jar and unityadc.jar most both be in Assets/Plugins/Android/ !
      Debug.LogError( "AdColony configuration error - make sure adcolony.jar and "
          + "unityadc.jar libraries are in your Unity project's Assets/Plugins/Android folder." );
    }
  }

  static void AndroidConfigure( string app_version, string app_id, string[] zone_ids ) {
    if(!adr_initialized) {
      AndroidInitializePlugin();
    }
    // Prepare call arguments.
    class_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

    var j_activity = class_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    var j_app_version = AndroidJNI.NewStringUTF( app_version );
    var j_app_id = AndroidJNI.NewStringUTF( app_id );
    var j_strings = AndroidJNIHelper.ConvertToJNIArray( zone_ids );

    // Call UnityADC.configure( version, app_version, app_id, ids )
    jvalue[] args = new jvalue[4];
    args[0].l = j_activity.GetRawObject();
    args[1].l = j_app_version;
    args[2].l = j_app_id;
    args[3].l = j_strings;

    AndroidJNI.CallStaticVoidMethod( class_UnityADC, method_configure, args );
    configured = true;
  }

  static public void AndroidSuspendToHomeScreen() {
    AndroidJavaClass class_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    AndroidJavaObject activity = class_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

    AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.MAIN");
    intent.Call<AndroidJavaObject>("addCategory", "android.intent.category.HOME");

    activity.Call("startActivity", intent);
  }

  static void AndroidResume() {
    var j_activity = class_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    jvalue[] args = new jvalue[1];
    args[0].l = j_activity.GetRawObject();

    AndroidJNI.CallStaticVoidMethod( class_UnityADC, method_resume, args );
  }

  static void AndroidPause() {
    var j_activity = class_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    jvalue[] args = new jvalue[1];
    args[0].l = j_activity.GetRawObject();

    AndroidJNI.CallStaticVoidMethod( class_UnityADC, method_pause, args );
  }

  static void AndroidSetCustomID( string custom_id ) {
    if(!adr_initialized) AndroidInitializePlugin();
    jvalue[] args = new jvalue[1];
    args[0].l = AndroidJNI.NewStringUTF( custom_id );
    AndroidJNI.CallStaticVoidMethod( class_UnityADC, method_setCustomID, args );
  }

  static string AndroidGetCustomID() {
    jvalue[] args = new jvalue[0];
    return AndroidJNI.CallStaticStringMethod( class_UnityADC, method_getCustomID, args );
  }

  static bool AndroidIsVideoAvailable( string zone_id ) {
    jvalue[] args = new jvalue[1];
    args[0].l = AndroidJNI.NewStringUTF( zone_id );
    return AndroidJNI.CallStaticBooleanMethod( class_UnityADC, method_isVideoAvailable, args );

  }

  static bool AndroidIsV4VCAvailable( string zone_id ) {
    jvalue[] args = new jvalue[1];
    args[0].l = AndroidJNI.NewStringUTF( zone_id );
    return AndroidJNI.CallStaticBooleanMethod( class_UnityADC, method_isV4VCAvailable, args );
  }

  static string AndroidGetDeviceID() {
    jvalue[] args = new jvalue[0];
    return AndroidJNI.CallStaticStringMethod( class_UnityADC, method_getDeviceID, args );
  }

  static string AndroidGetOpenUDID() {
    return "undefined";
  }

  static int AndroidGetV4VCAmount( string zone_id ) {
    jvalue[] args = new jvalue[1];
    args[0].l = AndroidJNI.NewStringUTF( zone_id );
    return AndroidJNI.CallStaticIntMethod( class_UnityADC, method_getV4VCAmount, args );
  }

  static string AndroidGetV4VCName( string zone_id ) {
    jvalue[] args = new jvalue[1];
    args[0].l = AndroidJNI.NewStringUTF( zone_id );
    return AndroidJNI.CallStaticStringMethod( class_UnityADC, method_getV4VCName, args );
  }

  static bool AndroidShowVideoAd( string zone_id ) {
    jvalue[] args = new jvalue[1];
    args[0].l = AndroidJNI.NewStringUTF( zone_id );
    AndroidJNI.CallStaticBooleanMethod( class_UnityADC, method_showVideo, args );
    return true;
  }

  static bool AndroidShowV4VC( bool popup_result, string zone_id ) {
    jvalue[] args = new jvalue[2];
    args[0].z = popup_result;
    args[1].l = AndroidJNI.NewStringUTF( zone_id );
    AndroidJNI.CallStaticBooleanMethod( class_UnityADC, method_showV4VC, args );
    return true;
  }

  static void AndroidOfferV4VC( bool popup_result, string zone_id ) {
    jvalue[] args = new jvalue[2];
    args[0].z = popup_result;
    args[1].l = AndroidJNI.NewStringUTF( zone_id );
    AndroidJNI.CallStaticVoidMethod( class_UnityADC, method_offerV4VC, args );
  }

  static string AndroidStatusForZone( string zone_id ) {
    jvalue[] args = new jvalue[1];
    args[0].l = AndroidJNI.NewStringUTF( zone_id );
    return AndroidJNI.CallStaticStringMethod( class_UnityADC, method_statusForZone, args );
  }

  static int AndroidGetAvailableViews( string zone_id ) {
    jvalue[] args = new jvalue[1];
    args[0].l = AndroidJNI.NewStringUTF( zone_id );
    return AndroidJNI.CallStaticIntMethod( class_UnityADC, method_getAvailableViews, args );
  }

  static void AndroidNotifyIAPComplete(string zone_id, string trans_id, string currency_code, double price) {
    jvalue[] args = new jvalue[4];
    args[0].l = AndroidJNI.NewStringUTF( zone_id );
    args[1].l = AndroidJNI.NewStringUTF( trans_id );
    args[2].l = AndroidJNI.NewStringUTF( currency_code );
    args[3].d = price;
    AndroidJNI.CallStaticVoidMethod( class_UnityADC, method_notifyIAPComplete, args);
  }

#endif // UNITY_ANDROID
}


public enum IAPEngagementType {NONE, OVERLAY, END_CARD, AUTOMATIC}

public class AdColonyAd {
  public bool shown;
  public bool iapEnabled;
  public string productID;
  public IAPEngagementType iapEngagementType;

  public string toString() {
    return "AdColonyAdInfo- Shown:" + shown + ", IAPEnabled: " + iapEnabled + ", productID:" + productID + ", IAPEngagementType: " + iapEngagementType;
  }
  public AdColonyAd(string[] split_args) {
    this.shown = split_args[0].Equals("true");
    this.iapEnabled = split_args[1].Equals("true");
    switch (split_args[2]) {
      case "NONE":
        this.iapEngagementType = IAPEngagementType.NONE;
        break;
      case "OVERLAY":
        this.iapEngagementType = IAPEngagementType.OVERLAY;
        break;
      case "END_CARD":
        this.iapEngagementType = IAPEngagementType.END_CARD;
        break;
      case "AUTOMATIC":
        this.iapEngagementType = IAPEngagementType.AUTOMATIC;
        break;
      default:
        this.iapEngagementType = IAPEngagementType.NONE;
        break;
      }
    this.productID = split_args[3];
  }
}
