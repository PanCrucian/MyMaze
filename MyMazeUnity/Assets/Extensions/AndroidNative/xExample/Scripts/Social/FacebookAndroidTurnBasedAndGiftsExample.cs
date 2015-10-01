////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FacebookAndroidTurnBasedAndGiftsExample : MonoBehaviour {
	
	
	private static bool IsUserInfoLoaded = false;
	private static bool IsAuntificated = false;
	
	
	
	public DefaultPreviewButton[] ConnectionDependedntButtons;
	
	public DefaultPreviewButton connectButton;
	public SA_Texture avatar;
	public SA_Label Location;
	public SA_Label Language;
	public SA_Label Mail;
	public SA_Label Name;



	private string BombItemId = "993386627342473";
	
	void Awake() {
		

		SPFacebook.instance.OnInitCompleteAction += OnInit;
		SPFacebook.instance.OnFocusChangedAction += OnFocusChanged;


		SPFacebook.instance.OnAuthCompleteAction += OnAuth;
	


		
		SPFacebook.instance.Init();
		

		SA_StatusBar.text = "initializing Facebook";


		
	}
	
	void FixedUpdate() {
		if(IsAuntificated) {
			connectButton.text = "Disconnect";
			Name.text = "Player Connected";
			foreach(DefaultPreviewButton btn in ConnectionDependedntButtons) {
				btn.EnabledButton();
			}
		} else {
			foreach(DefaultPreviewButton btn in ConnectionDependedntButtons) {
				btn.DisabledButton();
			}
			connectButton.text = "Connect";
			Name.text = "Player Disconnected";
			
		
			return;
		}
		
		if(IsUserInfoLoaded) {
			if(SPFacebook.instance.userInfo.GetProfileImage(FacebookProfileImageSize.square) != null) {
				avatar.texture = SPFacebook.instance.userInfo.GetProfileImage(FacebookProfileImageSize.square);
				Name.text = SPFacebook.instance.userInfo.name + " aka " + SPFacebook.instance.userInfo.username;
				Location.text = SPFacebook.instance.userInfo.location;
				Language.text = SPFacebook.instance.userInfo.locale;
			}
		}
		
	}


	public void RetriveAppRequests() {
		SPFacebook.instance.LoadPendingRequests();
		SPFacebook.instance.OnAppRequestsLoaded += OnAppRequestsLoaded;
	}

	void OnAppRequestsLoaded (FBResult result) {
		if(result.Error ==  null) {

			//Printing all pending request's id's
			foreach(FBAppRequest request in SPFacebook.instance.AppRequests) {
				Debug.Log("--------------------------");
				Debug.Log(request.Id);
				Debug.Log(request.Message);
				Debug.Log(request.ActionType);
				Debug.Log(request.State);
				Debug.Log(request.Data);
				Debug.Log("--------------------------");
			}
		}

		SPFacebook.instance.OnAppRequestsLoaded -= OnAppRequestsLoaded;
	}



	public void SendTrunhRequest() {
		SPFacebook.instance.SendTrunRequest("Smaple title", "Smaple Message");
	}

	public void SendTrunhRequestToSpecifiedFriend() {
		string FriendId = "1405568046403868";
		SPFacebook.instance.SendTrunRequest("Sample Titile", "Sample message", "some_request_data", new string[]{FriendId, "716261781804613"});
		SPFacebook.instance.OnAppRequestCompleteAction += OnAppRequestCompleteAction;



		SPFacebook.instance.AppRequest("Play with me", new string[]{FriendId, "716261781804613"});

	}


	public void AskItem() {
		SPFacebook.instance.AskGift("Sample Titile", "Sample message", BombItemId);
	}

	public void SendItem() {

		SPFacebook.instance.SendGift("Sample Titile", "Sample message", BombItemId);
	}


	public void SendInv() {
		string title = "Hello";
		string message = "Play with me";

		SPFacebook.instance.SendInvite(title, message);
		SPFacebook.instance.OnAppRequestCompleteAction += OnAppRequestCompleteAction;
	}

	public void SendInvToSpecifayedFirend() {
		string FriendId = "1405568046403868";


		string title = "Hello";
		string message = "Play with me";

		//Additional freeform data you may pass for tracking. This will be stored as part of the request objects created. The maximum length is 255 characters. (Optional) 
		string requestDate = "some_request_data";

		SPFacebook.instance.SendInvite(title, message, requestDate, new string[]{FriendId});
		SPFacebook.instance.OnAppRequestCompleteAction += OnAppRequestCompleteAction;
	}


	public void SendToSpecifiedFriend() {
		string FriendId = "1405568046403868";
		SPFacebook.instance.SendGift("Sample Titile", "Sample message", BombItemId, "some_request_dara", new string[]{FriendId});
		SPFacebook.instance.OnAppRequestCompleteAction += OnAppRequestCompleteAction;
	}

	void OnAppRequestCompleteAction (FBAppRequestResult result) {

		if(result.IsSucceeded) {
			Debug.Log("App request succeeded");
			Debug.Log("ReuqetsId: " + result.ReuqestId);
			foreach(string UserId in result.Recipients) {
				Debug.Log(UserId);
			}

			Debug.Log("Original Facebook Responce: " + result.Result.Text);
		} else {
			Debug.Log("App request has failed");
		}


		SPFacebook.instance.OnAppRequestCompleteAction -= OnAppRequestCompleteAction;


	}
	
	
	private void Connect() {
		if(!IsAuntificated) {
			SPFacebook.instance.Login("email,publish_actions");
			SA_StatusBar.text = "Log in...";
		} else {
			LogOut();
			SA_StatusBar.text = "Logged out";
		}
	}
	
	private void LoadUserData() {
		SPFacebook.instance.OnUserDataRequestCompleteAction += OnUserDataLoaded;
		SPFacebook.instance.LoadUserData();
		SA_StatusBar.text = "Loadin user data..";
	}
	


	// --------------------------------------
	// EVENTS
	// --------------------------------------
	

	
	private void OnFocusChanged(bool focus) {
		if (!focus)  {                                                                                        
			// pause the game - we will need to hide                                             
			Time.timeScale = 0;                                                                  
		} else  {                                                                                        
			// start the game back up - we're getting focus again                                
			Time.timeScale = 1;                                                                  
		}   
	}
	

	
	private void OnUserDataLoaded(FBResult result) {

		SPFacebook.instance.OnUserDataRequestCompleteAction -= OnUserDataLoaded;

		if (result.Error == null)  { 
			SA_StatusBar.text = "User data loaded";
			IsUserInfoLoaded = true;

			//user data available, we can get info using
			//SPFacebook.instance.userInfo getter
			//and we can also use userInfo methods, for exmple download user avatar image
			SPFacebook.instance.userInfo.LoadProfileImage(FacebookProfileImageSize.square);


		} else {
			SA_StatusBar.text ="Opps, user data load failed, something was wrong";
			Debug.Log("Opps, user data load failed, something was wrong");
		}

	}
	


	private void OnInit() {
		if(SPFacebook.instance.IsLoggedIn) {
			IsAuntificated = true;
		} else {
			SA_StatusBar.text = "user Login -> fale";
		}
	}
	
	
	private void OnAuth(FBResult result) {
		if(SPFacebook.instance.IsLoggedIn) {
			IsAuntificated = true;
			LoadUserData();
			SA_StatusBar.text = "user Login -> true";
		} else {
			Debug.Log("Failed to log in");
		}

	}

	
	
	// --------------------------------------
	// PRIVATE METHODS
	// --------------------------------------
	
	// --------------------------------------
	// PRIVATE METHODS
	// --------------------------------------
	

	
	private void LogOut() {
		IsUserInfoLoaded = false;
		
		IsAuntificated = false;
		
		SPFacebook.instance.Logout();
	}
	
	
	
}
