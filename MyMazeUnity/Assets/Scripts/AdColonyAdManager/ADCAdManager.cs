using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summmary>
/// This is a static class that references a singleton object in order to provide a simple interface for
/// interacting with the AdColony functionality.
///
/// This object is self-composing, meanining that it doesn't need to be added to the unity hierarchy list
/// in order to make calls to it. It will create the object if a call is made to it and it doesn't exist.
///
/// If desired you can still add it to the scene, however changing to another scene with an AdManager declared
/// will throw an error because two AdManagers will exist. This is because this object has DontDestroyOnLoad
/// called on it so that the scene changes will not affect its state.
///
/// To destroy this object you must manually call Destroy() on it.
///
/// This object uses a dictionary to associate arbitrary string names with video zones.
/// These arbitrary string names serve as a way to retrieve the zone information easier,
/// so that the developer does not need to remember random numbers.
///
/// The dictionary is configured by the 'ConfigureZones()' which in turn is called by the
/// 'ConfigureADCPlugin()' method on Awake()
/// </summmary>
public class ADCAdManager : MonoBehaviour {
  //---------------------------------------------------------------------------
  // The single instance of the ADCAdManager component
  private static ADCAdManager _instance;

  public static ADCAdManager Instance
  {
    get {
      if(_instance == null)
      {
        _instance = FindObjectOfType( typeof(ADCAdManager) ) as ADCAdManager;
        if(_instance == null)
        {
          _instance = (new GameObject("ADCAdManager")).AddComponent<ADCAdManager>();
        }
      }
      return _instance;
    }
  }
  //---------------------------------------------------------------------------

  // Currency tracker for the player
  public int regularCurrency = 0;

  // Arbitrary version number
  public string version = "1.1";
  // Your application id
  public string appId = "";

  public Dictionary<string, ADCVideoZone> videoZones = new Dictionary<string, ADCVideoZone>();

  void Awake() {
    //This calls the ADColony SDK's configure method in order to set up the zones for playing ads
    ConfigureADCPlugin();

    // These set the delegate functions that are called when these events fired by the ADColony SDK.
    // This is done so that users can have custom methods to respond to these events.
    AddOnVideoStartedMethod(OnVideoStarted);
    AddOnVideoFinishedMethod(OnVideoFinished);
    AddOnVideoFinishedWithInfoMethod(OnVideoFinishedWithInfo);
    AddOnV4VCResultMethod(OnV4VCResult);
    AddOnAdAvailabilityChangeMethod(OnAdAvailabilityChange);

    DontDestroyOnLoad(this.gameObject);
  }

	void Start () {
	}

	void Update () {
	}

  public void Pause() {
    GameObject musicGameObject = GameObject.Find("MusicGameObject");
    if(musicGameObject != null) {
      musicGameObject.GetComponent<AudioSource>().Pause();
    }
  }

  public void Resume() {
    GameObject musicGameObject = GameObject.Find("MusicGameObject");
    if(musicGameObject != null) {
      musicGameObject.GetComponent<AudioSource>().Play();
    }
  }


  public void ConfigureADCPlugin() {
    //THIS MUST BE RUN BEFORE ADCOLONY.CONFIGURE() IN ORDER FOR THE AD MANAGER TO BE AWARE OF WHAT INFORMATION TO PASS TO THE ADCOLONY PLUGIN
    ConfigureZones();

    // This configures the AdColony SDK so that the application is targetting the correct zone for generating ads
    AdColony.Configure (version, // Arbitrary app version
                        appId,   // ADC App ID from adcolony.com
                        GetVideoZoneIdsAsStringArray());
  }
  /// <summary>
  /// This method uses platform dependent compilation to determine what type of app id and zone id to use for the buttons. There are other ways to do this, but platform dependent compliation makes it easier for the code to stay all in one place for configuration.
  /// Reference: http://docs.unity3d.com/Manual/PlatformDependentCompilation.html
  /// </summary>
  public void ConfigureZones() {
    //Detects if running android and uses the android zone strings
    #if UNITY_ANDROID
      // App ID
      appId= "app185a7e71e1714831a49ec7";
      // Video zones
      AddZoneToManager("VideoZone1", "vz06e8c32a037749699e7050", ADCVideoZoneType.Interstitial);
      AddZoneToManager("VideoZone2", "vz06e8c32a037749699e7050", ADCVideoZoneType.Interstitial);
      // V4VC zones
      AddZoneToManager("V4VCZone1", "vz1fd5a8b2bf6841a0a4b826", ADCVideoZoneType.V4VC);
    //If not android defaults to setting the zone strings for iOS
    #else
      // App ID
      appId = "appbdee68ae27024084bb334a";
      // Video zones
      AddZoneToManager("VideoZone1", "vzf8fb4670a60e4a139d01b5", ADCVideoZoneType.Interstitial);
      AddZoneToManager("VideoZone2", "vzf8fb4670a60e4a139d01b5", ADCVideoZoneType.Interstitial);
      // V4VC zones
      AddZoneToManager("V4VCZone1", "vzf8e4e97704c4445c87504e", ADCVideoZoneType.V4VC);
    #endif
  }

  //---------------------------------------------------------------------------
  // Default Delegate Methods
  //---------------------------------------------------------------------------
  private void OnVideoStarted() {
    Debug.Log("On Video Started");
    Pause();
  }

  private void OnVideoFinished( bool adWasShown ) {
    Debug.Log("On Video Finished, and Ad was shown: " + adWasShown);
    Resume();
  }

  private void OnVideoFinishedWithInfo( AdColonyAd ad ) {
    Debug.Log("On Video Finished With Info, ad Played: " + ad.toString() );
    if(ad.iapEnabled) {
      Debug.Log("Calling Notify IAP Complete");
      AdColony.notifyIAPComplete("ProductID", "TransactionID");
      Debug.Log("Notified of IAP Complete");
    }
    Resume();
  }

  private void OnV4VCResult(bool success, string name, int amount) {
    if(success) {
      Debug.Log("V4VC SUCCESS: name = " + name + ", amount = " + amount);
      AddToCurrency(amount);
    } else {
      Debug.LogWarning("V4VC FAILED!");
    }
  }

  private void OnAdAvailabilityChange( bool avail, string zoneId) {
    Debug.Log("Ad Availability Changed to available=" + avail + " In zone: "+ zoneId);
  }

  //---------------------------------------------------------------------------
  // AdManager Delegate Wrapper
  //---------------------------------------------------------------------------
  public static void AddOnVideoStartedMethod(AdColony.VideoStartedDelegate onVideoStarted) {
    AdColony.OnVideoStarted += onVideoStarted;
  }
  public static void RemoveOnVideoStartedMethod(AdColony.VideoStartedDelegate onVideoStarted) {
    AdColony.OnVideoStarted -= onVideoStarted;
  }
  //-----------------
  public static void AddOnVideoFinishedMethod(AdColony.VideoFinishedDelegate onVideoFinished) {
    AdColony.OnVideoFinished += onVideoFinished;
  }
  public static void RemoveOnVideoFinishedMethod(AdColony.VideoFinishedDelegate onVideoFinished) {
    AdColony.OnVideoFinished -= onVideoFinished;
  }
  //-----------------
  public static void AddOnVideoFinishedWithInfoMethod(AdColony.VideoFinishedWithInfoDelegate onVideoFinishedWithInfo) {
    AdColony.OnVideoFinishedWithInfo += onVideoFinishedWithInfo;
  }
  public static void RemoveOnVideoFinishedWithInfoMethod(AdColony.VideoFinishedWithInfoDelegate onVideoFinishedWithInfo) {
    AdColony.OnVideoFinishedWithInfo -= onVideoFinishedWithInfo;
  }
  //-----------------
  public static void AddOnV4VCResultMethod(AdColony.V4VCResultDelegate onV4VCResult) {
    AdColony.OnV4VCResult += onV4VCResult;
  }
  public static void RemoveOnV4VCResultMethod(AdColony.V4VCResultDelegate onV4VCResult) {
    AdColony.OnV4VCResult -= onV4VCResult;
  }
  //-----------------
  public static void AddOnAdAvailabilityChangeMethod(AdColony.AdAvailabilityChangeDelegate onAdAvailabilityChange) {
    AdColony.OnAdAvailabilityChange += onAdAvailabilityChange;
  }
  public static void RemoveOnAdAvailabilityChangeMethod(AdColony.AdAvailabilityChangeDelegate onAdAvailabilityChange) {
    AdColony.OnAdAvailabilityChange -= onAdAvailabilityChange;
  }

  //---------------------------------------------------------------------------
  // ADCAdManager Property/Attribute Interaction
  //---------------------------------------------------------------------------
  public static void AddToCurrency(int amountToAddToCurrency) {
    ADCAdManager.Instance.regularCurrency += amountToAddToCurrency;
  }

  public static int GetRegularCurrencyAmount() {
    return ADCAdManager.Instance.regularCurrency;
  }

  //---------------------------------------------------------------------------
  // Zone Manager General Methods
  //---------------------------------------------------------------------------
  public static void ResetADCAdManagerZones() {
    ADCAdManager.GetVideoZonesDictionary().Clear();
  }

  public static bool IsVideoAvailableByZoneKey(string zoneKey) {
    bool isZoneAvailable = false;
    if(ContainsZoneKey(zoneKey)) {
      ADCVideoZone videoZone = ADCAdManager.GetVideoZoneObjectByKey(zoneKey);
      if(videoZone != null) {
        if(videoZone.zoneType == ADCVideoZoneType.Interstitial) {
          if(AdColony.IsVideoAvailable(videoZone.zoneId)) {
            isZoneAvailable = true;
          }
        }
        else if(videoZone.zoneType == ADCVideoZoneType.V4VC) {
          if(AdColony.IsV4VCAvailable(videoZone.zoneId)) {
            isZoneAvailable = true;
          }
        }
        else {
          //Check nothing, video zone type isn't correct
        }
      }
    }
    return isZoneAvailable;
  }

  public static void AddZoneToManager(string zoneKey, string zoneId, ADCVideoZoneType videoZoneType) {
    zoneKey = zoneKey.ToLower();
    if(ContainsZoneKey(zoneKey)) {
      Debug.LogWarning("The ad manager overwrote the previous video zoneId: " + GetZoneIdByKey(zoneKey) + " for the video zone named " + zoneKey + " with the new video zoneId of: " + zoneId);
    }
    else {
      Debug.LogWarning("The ad manager has added the video zone named " + zoneKey + " with the video zoneId of: " + zoneId);
      ADCAdManager.GetVideoZonesDictionary().Add(zoneKey, new ADCVideoZone(zoneId, videoZoneType));
    }
  }

  public static ADCVideoZone GetVideoZoneObjectByKey(string key) {
    key = key.ToLower();
    if(ContainsZoneKey(key)) {
      return ADCAdManager.GetVideoZonesDictionary()[key];
    }
    else {
      return null;
    }
  }

  public static string GetZoneIdByKey(string key) {
    key = key.ToLower();
    if(ContainsZoneKey(key)) {
      return ADCAdManager.GetVideoZonesDictionary()[key].zoneId;
    }
    else {
      return "";
    }
  }

  public static bool ContainsZoneKey(string key) {
    key = key.ToLower();
    if(GetVideoZonesDictionary().ContainsKey(key)) {
      return true;
    }
    else {
      return false;
    }
  }

  public static void RemoveZoneFromManager(string zoneKey) {
    zoneKey = zoneKey.ToLower();
    ADCAdManager.GetVideoZonesDictionary().Remove(zoneKey);
  }

  public static string[] GetVideoZoneIdsAsStringArray() {
    Dictionary<string, ADCVideoZone> videoZones = GetVideoZonesDictionary();
    string[] allZones = new string[GetVideoZonesDictionary().Count];
    int currentKeyValuePair = 0;
    foreach(KeyValuePair<string, ADCVideoZone> keyValuePair in videoZones) {
      allZones[currentKeyValuePair] = keyValuePair.Value.zoneId;
      currentKeyValuePair++;
    }

    return allZones;
  }

  public static Dictionary<string, ADCVideoZone> GetVideoZonesDictionary() {
    return ADCAdManager.Instance.videoZones;
  }

  public static void ShowVideoAdByZoneKey(string zoneIdKey, bool offerV4VCBeforePlay = false, bool showPopUpAfter = false) {
    ADCVideoZone videoZone = GetVideoZoneObjectByKey(zoneIdKey);
    string zoneId = GetZoneIdByKey(zoneIdKey);

    if(IsVideoAvailableByZoneKey(zoneIdKey)) {
      if(videoZone.zoneType == ADCVideoZoneType.Interstitial) {
        AdColony.ShowVideoAd(zoneId);
      }
      else if(videoZone.zoneType == ADCVideoZoneType.V4VC) {
        if(offerV4VCBeforePlay) {
          AdColony.OfferV4VC(showPopUpAfter, zoneId);
        } else {
          AdColony.ShowV4VC(showPopUpAfter, zoneId);
        }
      } else {
        //Check nothing, video zone type isn't correct
        Debug.Log("An incorrect video zone type was requested to play. Please resolve this issue.");
      }
      Debug.Log("The zone '" + zoneId + "' was requested to play.");
    } else {
      Debug.Log("The zone '" + zoneId + "' was requested to play, but it is NOT ready to play yet.");
    }
  }
}
