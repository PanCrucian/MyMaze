////////////////////////////////////////////////////////////////////////////////
//  
// @module Mobile Social Plugin 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System;
using System.Collections;

#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class IOSInstagramManager : SA_Singleton<IOSInstagramManager> {


	public static event Action ActionPostSucceeded = delegate{};
	public static event Action<InstagramPostResult> ActionPostFailed = delegate{};


	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _MSP_InstaShare(string encodedMedia, string message);

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

	public void Share(Texture2D texture) {
		Share(texture, "");
	}


	public void Share(Texture2D texture, string message) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

		byte[] val = texture.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);
		

		_MSP_InstaShare(bytesString, message);

		#endif

	}




	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------


	private void OnInstaPostSuccess() {
		ActionPostSucceeded();
	}
	
	
	private void OnInstaPostFailed(string data) {

		int code = System.Convert.ToInt32(data);
		InstagramPostResult error = InstagramPostResult.NO_APPLICATION_INSTALLED;

		switch(code) {
		case 1:
			error = InstagramPostResult.NO_APPLICATION_INSTALLED;
			break;
		case 2:
			error = InstagramPostResult.USER_CANCELLED;
			break;
		case 3:
			error = InstagramPostResult.SYSTEM_VERSION_ERROR;
			break;
		}

		ActionPostFailed(error);
	}

	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
