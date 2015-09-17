using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


#if UNITY_ANDROID
public class VungleAndroidManager : MonoBehaviour
{
	#region Constructor and Lifecycle

	static VungleAndroidManager()
	{
		// try/catch this so that we can warn users if they try to stick this script on a GO manually
		try
		{
			// create a new GO for our manager
			var go = new GameObject( "VungleAndroidManager" );
			go.AddComponent<VungleAndroidManager>();
			DontDestroyOnLoad( go );
		}
		catch( UnityException )
		{
			Debug.LogWarning( "It looks like you have the VungleAndroidManager on a GameObject in your scene. Please remove the script from your scene." );
		}
	}


	// used to ensure the VungleAndroidManager will always be in the scene to avoid SendMessage logs if the user isn't using any events
	public static void noop(){}

	#endregion



	// Fired when a Vungle ad starts
	public static event Action onAdStartEvent;

	// Fired when a Vungle ad finishes
	public static event Action onAdEndEvent;

	// Fired when a Vungle ad is cached and ready to be displayed
	public static event Action onCachedAdAvailableEvent;

	// Fired when a Vungle video is dismissed. Includes the watched and total duration in milliseconds.
	public static event Action<double,double> onVideoViewEvent;



	#region Native code will call these methods

	void onAdStart( string empty )
	{
		if( onAdStartEvent != null )
			onAdStartEvent();
	}


	void onAdEnd( string empty )
	{
		if( onAdEndEvent != null )
			onAdEndEvent();
	}


	void onCachedAdAvailable( string empty )
	{
		if( onCachedAdAvailableEvent != null )
			onCachedAdAvailableEvent();
	}


	void onVideoView( string str )
	{
		if( onVideoViewEvent != null )
		{
			var parts = str.Split( new char[] { '-' } );
			if( parts.Length == 2 )
				onVideoViewEvent( double.Parse( parts[0] ) / 1000, double.Parse( parts[1] ) / 1000 );
		}
	}

	#endregion

}
#endif
