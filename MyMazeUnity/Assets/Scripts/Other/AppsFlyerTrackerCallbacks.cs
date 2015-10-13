using UnityEngine;
using System.Collections;


public class AppsFlyerTrackerCallbacks : MonoBehaviour {

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

	// Use this for initialization
	void Start () {
		Debug.Log ("AppsFlyerTrackerCallbacks on Start");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void didReceiveConversionData(string conversionData) {
        Debug.Log("AppsFlyerTrackerCallbacks:: got conversion data = " + conversionData);
	}
	
	public void didReceiveConversionDataWithError(string error) {
        Debug.Log("AppsFlyerTrackerCallbacks:: got conversion data error = " + error);
	}
	
	public void didFinishValidateReceipt(string validateResult) {
        Debug.Log("AppsFlyerTrackerCallbacks:: got didFinishValidateReceipt  = " + validateResult);
		
	}
	
	public void didFinishValidateReceiptWithError (string error) {
        Debug.Log("AppsFlyerTrackerCallbacks:: got idFinishValidateReceiptWithError error = " + error);
		
	}
	
	public void onAppOpenAttribution(string validateResult) {
        Debug.Log("AppsFlyerTrackerCallbacks:: got onAppOpenAttribution  = " + validateResult);
		
	}
	
	public void onAppOpenAttributionFailure (string error) {
        Debug.Log("AppsFlyerTrackerCallbacks:: got onAppOpenAttributionFailure error = " + error);
		
	}
	
	public void onInAppBillingSuccess (string res) {
        Debug.Log("AppsFlyerTrackerCallbacks:: got onInAppBillingSuccess succcess var = " + res);
		
	}
	public void onInAppBillingFailure (string error) {
        Debug.Log("AppsFlyerTrackerCallbacks:: got onInAppBillingFailure error = " + error);
		
	}
}
