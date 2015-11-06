////////////////////////////////////////////////////////////////////////////////
//  
// @module Mobile Social Plugin 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class InstagramUseExample : MonoBehaviour {

	public Texture2D imageForPosting;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		SPInstagram.OnPostingCompleteAction += OnPostingCompleteAction;
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	void OnGUI() {
		if(GUI.Button(new Rect(30, 70, 180, 50), "Share Image")) {
			SPInstagram.instance.Share(imageForPosting);
		}
		
		if(GUI.Button(new Rect(250, 70, 180, 50), "Share Image With Message")) {
			SPInstagram.instance.Share(imageForPosting, "I am posting from my app");
		}
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------


	void OnPostingCompleteAction (InstagramPostResult result) {
		if(result == InstagramPostResult.RESULT_OK) {
			Debug.Log("Posting Successful");
		} else {
			Debug.Log("Posting failed with error code " + result.ToString());
		}
	}
	

	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
