using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


#if UNITY_IPHONE
public class VungleManager : MonoBehaviour
{
	#region Constructor and Lifecycle

	static VungleManager()
	{
		// try/catch this so that we can warn users if they try to stick this script on a GO manually
		try
		{
			// create a new GO for our manager
			var go = new GameObject( "VungleManager" );
			go.AddComponent<VungleManager>();
			DontDestroyOnLoad( go );
		}
		catch( UnityException )
		{
			Debug.LogWarning( "It looks like you have the VungleManager on a GameObject in your scene. Please remove the script from your scene." );
		}
	}


	// used to ensure the VungleManager will always be in the scene to avoid SendMessage logs if the user isn't using any events
	public static void noop(){}

	#endregion



	// Fired when a video has finished playing. Includes the following keys: completedView (bool), playTime (double),
	// didDownload (bool) and willPresentProductSheet (bool).
	public static event Action<Dictionary<string,object>> vungleSDKwillCloseAdEvent;

	// Fired when the product sheet is dismissed
	public static event Action vungleSDKwillCloseProductSheetEvent;

	// Fired when the video is shown
	public static event Action vungleSDKwillShowAdEvent;

	// Fired when a Vungle ad is cached and ready to be displayed
	public static event Action vungleSDKhasCachedAdAvailableEvent;

	// Fired when a Vungle write log
	public static event Action<string> vungleSDKlogEvent;


	#region Native code will call these methods

	void vungleSDKwillCloseAd( string json )
	{
		if( vungleSDKwillCloseAdEvent != null )
			vungleSDKwillCloseAdEvent( (Dictionary<string,object>)MiniJSON.Json.Deserialize( json ) );
	}


	void vungleSDKwillCloseProductSheet( string empty )
	{
		if( vungleSDKwillCloseProductSheetEvent != null )
			vungleSDKwillCloseProductSheetEvent();
	}


	void vungleSDKwillShowAd( string empty )
	{
		if( vungleSDKwillShowAdEvent != null )
			vungleSDKwillShowAdEvent();
	}


	void vungleSDKhasCachedAdAvailable()
	{
		if( vungleSDKhasCachedAdAvailableEvent != null )
			vungleSDKhasCachedAdAvailableEvent();
	}

	void vungleSDKlog(string log)
	{
		if( vungleSDKlogEvent != null )
			vungleSDKlogEvent(log);
	}
	
	#endregion
}
#endif

