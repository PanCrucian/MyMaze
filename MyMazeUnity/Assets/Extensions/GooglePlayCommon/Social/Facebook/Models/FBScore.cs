using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FBScore  {
	public string UserId;
	public string UserName;

	public string AppId;
	public string AppName;


	public int value;

	private Dictionary<FacebookProfileImageSize, Texture2D> profileImages =  new Dictionary<FacebookProfileImageSize, Texture2D>();
	public event Action<FBScore> OnProfileImageLoaded = delegate {};


	public string GetProfileUrl(FacebookProfileImageSize size) {
		return  "https://graph.facebook.com/" + UserId + "/picture?type=" + size.ToString();
	} 
	
	public Texture2D  GetProfileImage(FacebookProfileImageSize size) {
		if(profileImages.ContainsKey(size)) {
			return profileImages[size];
		} else {
			return null;
		}
	}
	
	public void LoadProfileImage(FacebookProfileImageSize size) {
		if(GetProfileImage(size) != null) {
			Debug.LogWarning("Profile image already loaded, size: " + size);
			OnProfileImageLoaded(this);
		}
		
		
		WWWTextureLoader loader = WWWTextureLoader.Create();
		
		switch(size) {
		case FacebookProfileImageSize.large:
			loader.OnLoad += OnLargeImageLoaded;
			break;
		case FacebookProfileImageSize.normal:
			loader.OnLoad += OnNormalImageLoaded;
			break;
		case FacebookProfileImageSize.small:
			loader.OnLoad += OnSmallImageLoaded;
			break;
		case FacebookProfileImageSize.square:
			loader.OnLoad += OnSquareImageLoaded;
			break;
			
		}
		
		Debug.Log("LOAD IMAGE URL: " + GetProfileUrl(size));
		
		loader.LoadTexture(GetProfileUrl(size));
		
		
	}

	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	private void OnSquareImageLoaded(Texture2D image) {
		
		if(image != null && !profileImages.ContainsKey(FacebookProfileImageSize.square)) {
			profileImages.Add(FacebookProfileImageSize.square, image);
		}
		
		OnProfileImageLoaded(this);
	}
	
	private void OnLargeImageLoaded(Texture2D image) {
		if(image != null && !profileImages.ContainsKey(FacebookProfileImageSize.large)) {
			profileImages.Add(FacebookProfileImageSize.large, image);
		}
		
		OnProfileImageLoaded(this);
	}
	
	
	private void OnNormalImageLoaded(Texture2D image) {
		if(image != null && !profileImages.ContainsKey(FacebookProfileImageSize.normal)) {
			profileImages.Add(FacebookProfileImageSize.normal, image);
		}
		
		OnProfileImageLoaded(this);
	}
	
	private void OnSmallImageLoaded(Texture2D image) {
		if(image != null && !profileImages.ContainsKey(FacebookProfileImageSize.small)) {
			profileImages.Add(FacebookProfileImageSize.small, image);
		}
		
		OnProfileImageLoaded(this);
	}


}
