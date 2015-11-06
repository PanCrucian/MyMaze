////////////////////////////////////////////////////////////////////////////////
//  
// @module Mobile Social Plugin 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using System;
using UnityEngine;
using System.Collections;

public class SPInstagram : SA_Singleton<SPInstagram>  {

	public  static event Action<InstagramPostResult> OnPostingCompleteAction = delegate {};


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		Debug.Log("SPInstagram subscribed");
		switch(Application.platform) {

		case RuntimePlatform.IPhonePlayer:
			IOSInstagramManager.ActionPostSucceeded += OnPost; 
			IOSInstagramManager.ActionPostFailed += OnPostFailed; 
			break;
		case RuntimePlatform.Android:
			AndroidInstagramManager.OnPostingCompleteAction  += HandleOnPostingCompleteAction;
			break;
		}
		
		DontDestroyOnLoad(gameObject);
	}




	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	

	public void Share(Texture2D texture) {
		switch(Application.platform) {
		case RuntimePlatform.IPhonePlayer:
			IOSInstagramManager.instance.Share(texture);
			break;
		case RuntimePlatform.Android:
			Debug.Log(AndroidInstagramManager.instance);
			AndroidInstagramManager.instance.Share(texture);
			break;
		}

	}
	
	
	public void Share(Texture2D texture, string message) {
		switch(Application.platform) {
		case RuntimePlatform.IPhonePlayer:
			IOSInstagramManager.instance.Share(texture, message);
			break;
		case RuntimePlatform.Android:
			AndroidInstagramManager.instance.Share(texture, message);
			break;
		}
	}


	//--------------------------------------
	//  IOS Actions
	//--------------------------------------
	
	private void OnPost() {
		Debug.Log("SPInstagram OnPost");
		OnPostingCompleteAction(InstagramPostResult.RESULT_OK);
	}
	
	private void OnPostFailed(InstagramPostResult error) {
		OnPostingCompleteAction(error);
	}

	//--------------------------------------
	//  Android Actions
	//--------------------------------------


	void HandleOnPostingCompleteAction (InstagramPostResult res) {
		OnPostingCompleteAction(res);
	}

	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
