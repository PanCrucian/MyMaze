////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using System;
using UnityEngine;

public class GameCenterPlayerTemplate {

	private string _playerId;
	private string _displayName;
	private string _alias;
	private Texture2D _avatar = null;


	public event Action<Texture2D> OnPlayerAvatarLoaded =  delegate {};


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public GameCenterPlayerTemplate (string pId, string pName, string pAlias) {
		_playerId = pId;
		_displayName = pName;
		_alias = pAlias;

	}

	public void SetAvatar(string base64String) {

		if(base64String.Length == 0) {
			return;
		}

		byte[] decodedFromBase64 = System.Convert.FromBase64String(base64String);
		_avatar = new Texture2D(1, 1);
		_avatar.LoadImage(decodedFromBase64);

		OnPlayerAvatarLoaded(_avatar);
	}

	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public string PlayerId {
		get {
			return _playerId;
		}
	}

	public string Alias {
		get {
			return _alias;
		}
	}


	public string DisplayName {
		get {
			return _displayName;
		}
	}

	public Texture2D Avatar {
		get {
			return _avatar;
		}
	}


}


