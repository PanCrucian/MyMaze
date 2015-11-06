////////////////////////////////////////////////////////////////////////////////
//  
// @module <module_name>
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class MSPTwitterUseExample : MonoBehaviour {

	
	//Replace with your key and secret
	private static string TWITTER_CONSUMER_KEY = "wEvDyAUr2QabVAsWPDiGwg";
	private static string TWITTER_CONSUMER_SECRET = "igRxZbOrkLQPNLSvibNC3mdNJ5tOlVOPH3HNNKDY0";
	
	
	
	
	private static bool IsUserInfoLoaded = false;
	
	private static bool IsAuntifivated = false;
	
	public Texture2D ImageToShare;
	public DefaultPreviewButton connectButton;
	public SA_Texture avatar;
	public SA_Label Location;
	public SA_Label Language;
	public SA_Label Status;
	public SA_Label Name;
	public DefaultPreviewButton[] AuthDependedButtons;
	
	void Awake() {



		SPTwitter.Instance.OnTwitterInitedAction += OnInit;
		SPTwitter.Instance.OnAuthCompleteAction += OnAuth;

		SPTwitter.Instance.OnPostingCompleteAction += OnPost;

		SPTwitter.Instance.OnUserDataRequestCompleteAction += OnUserDataLoaded;
		
		
		//You can use:
		//SPTwitter.instance.Init();
		//if TWITTER_CONSUMER_KEY and TWITTER_CONSUMER_SECRET was alredy set in 
		//Window -> Mobile Social Plugin -> Edit Settings menu.
		
		
		SPTwitter.Instance.Init(TWITTER_CONSUMER_KEY, TWITTER_CONSUMER_SECRET);
		
		
		
	}
	
	void FixedUpdate() {
		if(IsAuntifivated) {
			connectButton.text = "Disconnect";
			Name.text = "Player Connected";
			foreach(DefaultPreviewButton button in AuthDependedButtons) {
				button.EnabledButton();
			}
		} else {
			foreach(DefaultPreviewButton button in AuthDependedButtons) {
				button.DisabledButton();
			}
			connectButton.text = "Connect";
			Name.text = "Player Disconnected";
			
			return;
		}

		
		
		if(IsUserInfoLoaded) {
			
			
			if(SPTwitter.instance.userInfo.profile_image != null) {
				avatar.texture = SPTwitter.instance.userInfo.profile_image;
			}
			
			Name.text = SPTwitter.instance.userInfo.name + " aka " + SPTwitter.instance.userInfo.screen_name;
			Location.text = SPTwitter.instance.userInfo.location;
			Language.text = SPTwitter.instance.userInfo.lang;
			Status.text = SPTwitter.instance.userInfo.status.text;
			
			
		}
		
	}
	
	private void Connect() {
		if(!IsAuntifivated) {
			SPTwitter.instance.AuthenticateUser();
		} else {
			LogOut();
		}
	}
	
	private void PostWithAuthCheck() {
		SPTwitter.instance.PostWithAuthCheck("Hello, I'am posting this from my app");
	}
	
	private void PostNativeScreenshot() {
		StartCoroutine(PostTWScreenshot());
	}
	
	private void PostMSG() {
		SPShareUtility.TwitterShare("This is my text to share");
	}
	
	
	private void PostImage() {
		SPShareUtility.TwitterShare("This is my text to share", ImageToShare);
	}
	
	
	
	private IEnumerator PostTWScreenshot() {
		
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();


		SPShareUtility.TwitterShare("This is my text to share", tex);
		
		Destroy(tex);
		
	}
	
	
	private void LoadUserData() {
		SPTwitter.instance.LoadUserData();
	}
	
	private void PostMessage() {
		SPTwitter.instance.Post("Hello, I'am posting this from my app");
	}
	
	private void PostScreehShot() {
		StartCoroutine(PostScreenshot());
	}
	

	// --------------------------------------
	// EVENTS
	// --------------------------------------
	
	private void OnUserDataLoaded(TWResult res) {
		if(res.IsSucceeded) {
			IsUserInfoLoaded = true;
			SPTwitter.Instance.userInfo.LoadProfileImage();
			SPTwitter.Instance.userInfo.LoadBackgroundImage();
		} else {
			Debug.Log("Opps, user data load failed, something was wrong");
		}
	}
	
	
	private void OnPost(TWResult res) {
		if(res.IsSucceeded) {
			Debug.Log("Congrats, you just postet something to twitter");
		} else {
			Debug.Log("Opps, post failed, something was wrong");
		}
	}

	
	private void OnInit(TWResult res) {
		if(SPTwitter.instance.IsAuthed) {
			OnAuth(null);
		}
	}
	
	
	private void OnAuth(TWResult res) {
		IsAuntifivated = true;
	}


	
	// --------------------------------------
	// Aplication Only API Maehtods
	// --------------------------------------
	

	private void RetrieveTimeLine() {
		TW_UserTimeLineRequest r =  TW_UserTimeLineRequest.Create();
		r.ActionComplete += OnTimeLineRequestComplete;
		r.AddParam("screen_name", "unity3d");
		r.AddParam("count", "1");
		r.Send();
	}
	
	
	private void UserLookUpRequest() {
		TW_UsersLookUpRequest r =  TW_UsersLookUpRequest.Create();
		r.ActionComplete += OnLookUpRequestComplete;
		r.AddParam("screen_name", "unity3d");
		r.Send();
	}
	
	
	private void FriedsidsRequest() {
		TW_FriendsIdsRequest r =  TW_FriendsIdsRequest.Create();
		r.ActionComplete += OnIdsLoaded;
		r.AddParam("screen_name", "unity3d");
		r.Send();
	}
	
	private void FollowersidsRequest() {
		TW_FollowersIdsRequest r =  TW_FollowersIdsRequest.Create();
		r.ActionComplete += OnIdsLoaded;
		r.AddParam("screen_name", "unity3d");
		r.Send();
	}
	
	private void TweetSearch() {
		TW_SearchTweetsRequest r =  TW_SearchTweetsRequest.Create();
		r.ActionComplete += OnSearchRequestComplete;
		r.AddParam("q", "@noradio");
		r.AddParam("count", "1");
		r.Send();
	}
	
	
	
	
	// --------------------------------------
	// Events
	// --------------------------------------
	
	private void OnIdsLoaded(TW_APIRequstResult result) {

		if(result.IsSucceeded) {
			Debug.Log( "Totals ids loaded: " + result.ids.Count);
		} else {
			Debug.Log(result.responce);
		}
	}
	
	
	private void OnLookUpRequestComplete(TW_APIRequstResult result) {
		
		if(result.IsSucceeded) {
			string msg = "User Id: ";
			msg+= result.users[0].id;
			msg+= "\n";
			msg+= "User Name:" + result.users[0].name;
			

			Debug.Log(msg);
		} else {
			Debug.Log(result.responce);

		}
	}
	
	
	private void OnSearchRequestComplete(TW_APIRequstResult result) {

		if(result.IsSucceeded) {
			string msg = "Tweet text:" + "\n";
			msg+= result.tweets[0].text;
			

			Debug.Log(msg);
		} else {
			Debug.Log(result.responce);
		}
		
	}
	
	
	private void OnTimeLineRequestComplete(TW_APIRequstResult result) {

		
		if(result.IsSucceeded) {

			string msg ;
			if(result.tweets.Count > 0) {
				msg = "Last Tweet text:" + "\n";
				msg+= result.tweets[0].text;
			} else {
				msg = "NO tweens found";
			}

			

			Debug.Log(msg);
		} else {
			Debug.Log(result.responce);
		}
		
	}




	// --------------------------------------
	// PRIVATE METHODS
	// --------------------------------------
	
	private IEnumerator PostScreenshot() {
		
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		SPTwitter.instance.Post("My app ScreehShot", tex);
		
		Destroy(tex);
		
	}
	
	private void LogOut() {
		IsUserInfoLoaded = false;
		
		IsAuntifivated = false;
		
		SPTwitter.instance.LogOut();
	}

}
