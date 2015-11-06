//#define SA_DEBUG_MODE
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////




using UnityEngine;
using System.Collections;

#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class MSPIOSSocialManager : SA_Singleton<MSPIOSSocialManager> {


	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _MSP_TwPost(string text, string url, string encodedMedia);
	
	[DllImport ("__Internal")]
	private static extern void _MSP_FbPost(string text, string url, string encodedMedia);
	
	[DllImport ("__Internal")]
	private static extern void _MSP_MediaShare(string text, string encodedMedia);
	
	[DllImport ("__Internal")]
	private static extern void _MSP_SendMail(string subject, string body,  string recipients, string encodedMedia);

	#endif

	

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}



	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void ShareMedia(string text) {
		ShareMedia(text, null);
	}

	public void ShareMedia(string text, Texture2D texture) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			if(texture != null) {
				byte[] val = texture.EncodeToPNG();
				string bytesString = System.Convert.ToBase64String (val);
			_MSP_MediaShare(text, bytesString);
			} else {
			_MSP_MediaShare(text, "");
			}
		#endif
	}

	public void TwitterPost(string text, string url = null, Texture2D texture = null) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		if(text == null) {
			text = "";
		}
		
		if(url == null) {
			url = "";
		}
		
		string encodedMedia = "";
		
		if(texture != null) {
			byte[] val = texture.EncodeToPNG();
			encodedMedia = System.Convert.ToBase64String (val);
		}
		
		
		_MSP_TwPost(text, url, encodedMedia);
		#endif
	}
	
	
	
	public void FacebookPost(string text, string url = null, Texture2D texture = null) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		if(text == null) {
			text = "";
		}
		
		if(url == null) {
			url = "";
		}
		
		string encodedMedia = "";
		
		if(texture != null) {
			byte[] val = texture.EncodeToPNG();
			encodedMedia = System.Convert.ToBase64String (val);
		}
		
		
		_MSP_FbPost(text, url, encodedMedia);
		#endif
	}



	public void SendMail(string subject, string body, string recipients) {
		SendMail(subject, body, recipients, null);
	}
	
	public void SendMail(string subject, string body, string recipients, Texture2D texture) {
		if(texture == null) {
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_MSP_SendMail(subject, body, recipients, "");
			#endif
		} else {
			
			
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			byte[] val = texture.EncodeToPNG();
			string bytesString = System.Convert.ToBase64String (val);
			_MSP_SendMail(subject, body, recipients, bytesString);
			#endif
		}
	}



	
	//--------------------------------------
	//  Actions
	//--------------------------------------

	private void OnTwitterPostFailed() {

	}

	private void OnTwitterPostSuccess() {
	
	}

	private void OnFacebookPostFailed() {

	}
	
	private void OnFacebookPostSuccess() {

	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------


	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
