////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////




using UnityEngine;
using Facebook;
using System;
using System.Collections;
using System.Collections.Generic;

public class SPFacebook : SA_Singleton<SPFacebook> {
	
	
	private FacebookUserInfo _userInfo = null;
	private Dictionary<string,  FacebookUserInfo> _friends;
	private bool _IsInited = false;
	
	
	private  Dictionary<string,  FBScore> _userScores =  new Dictionary<string, FBScore>();
	private  Dictionary<string,  FBScore> _appScores  =  new Dictionary<string, FBScore>();
	
	private int lastSubmitedScore = 0;
	
	
	private  Dictionary<string,  Dictionary<string, FBLikeInfo>> _likes =  new Dictionary<string, Dictionary<string, FBLikeInfo>>();
	
	private List<FBAppRequest> _AppRequests =  new List<FBAppRequest>();
	
	
	//Actinos
	public event Action OnInitCompleteAction = delegate {};
	public event Action<FBPostResult> OnPostingCompleteAction = delegate {};
	
	
	public event Action<bool> OnFocusChangedAction = delegate {};
	public event Action<FBResult> OnAuthCompleteAction = delegate {};
	public event Action<FBResult> OnPaymentCompleteAction = delegate {};
	public event Action<FBResult> OnUserDataRequestCompleteAction = delegate {};
	public event Action<FBResult> OnFriendsDataRequestCompleteAction = delegate {};
	
	
	public event Action<FBAppRequestResult> OnAppRequestCompleteAction = delegate {};
	public event Action<FBResult> OnAppRequestsLoaded = delegate {};
	
	
	//--------------------------------------
	//  Scores API 
	//  https://developers.facebook.com/docs/games/scores
	//------------------------------------
	
	public event Action<FB_APIResult> OnAppScoresRequestCompleteAction 			= delegate {};
	public event Action<FB_APIResult> OnPlayerScoresRequestCompleteAction   		= delegate {};
	public event Action<FB_APIResult> OnSubmitScoreRequestCompleteAction   		= delegate {};
	public event Action<FB_APIResult> OnDeleteScoresRequestCompleteAction   		= delegate {};
	
	
	//--------------------------------------
	//  Likes API 
	//  https://developers.facebook.com/docs/graph-api/reference/v2.0/user/likes
	//------------------------------------
	
	public event Action<FB_APIResult> OnLikesListLoadedAction = delegate {};
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	
	public void Init() {
		FB.Init(OnInitComplete, OnHideUnity);
	}
	
	
	public void Login() {
		Login(SocialPlatfromSettings.Instance.fb_scopes);
	}
	
	
	private bool IsLoginRequestSent = false;
	public void Login(string scopes) {
		
		Debug.Log("SPFacebook: making login with teh scopes: "  + scopes);
		if(!IsLoginRequestSent && !FB.IsLoggedIn) {
			Debug.Log("AndroidNative login");
			IsLoginRequestSent = true;
			FB.Login(scopes, LoginCallback);
		}
		
		BroadcastLoginResult();
	}
	
	
	//--------------------------------------
	//  API METHODS
	//--------------------------------------
	
	
	public void Logout() {
		FB.Logout();
		IsLoginRequestSent = false;
		LoginCallbackResult = null;
	}
	
	
	
	public void LoadUserData() {
		if(IsLoggedIn) {
			
			FB.API("/me", Facebook.HttpMethod.GET, UserDataCallBack);  
			
		} else {
			
			Debug.LogWarning("Auth user before loadin data, fail event generated");
			FBResult res = new FBResult("","User isn't authed");
			OnUserDataRequestCompleteAction(res);
		}
	}
	
	public void LoadFrientdsInfo(int limit) {
		if(IsLoggedIn) {
			
			FB.API("/me?fields=friends.limit(" + limit.ToString() + ").fields(first_name,id,last_name,name,link,locale,location)", Facebook.HttpMethod.GET, FriendsDataCallBack);  
			
		} else {
			Debug.LogWarning("Auth user before loadin data, fail event generated");
			FBResult res = new FBResult("","User isn't authed");
			OnFriendsDataRequestCompleteAction(res);
		}
	}
	
	public FacebookUserInfo GetFriendById(string id) {
		if(_friends != null) {
			if(_friends.ContainsKey(id)) {
				return _friends[id];
			}
		}
		
		return null;
	}
	
	
	
	public void PostImage(string caption, Texture2D image) {
		
		byte[] imageBytes = image.EncodeToPNG();
		
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("message", caption);
		wwwForm.AddBinaryData("image", imageBytes, "InteractiveConsole.png");
		
		FB.API("me/photos", Facebook.HttpMethod.POST, PostCallBack, wwwForm);
		
	}
	
	
	public void PostText(string message) {
		
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("message", message);
		
		FB.API("me/feed", Facebook.HttpMethod.POST, PostCallBack, wwwForm);
	}
	
	
	public FBPostingTask PostWithAuthCheck(
		string toId = "",
		string link = "",
		string linkName = "",
		string linkCaption = "",
		string linkDescription = "",
		string picture = "",
		string actionName = "",
		string actionLink = "",
		string reference = ""
		) {
		
		FBPostingTask task = FBPostingTask.Cretae();
		
		task.Post(toId,
		          link,
		          linkName,
		          linkCaption,
		          linkDescription,
		          picture,
		          actionName,
		          actionLink,
		          reference);
		
		
		return task;
		
	} 
	
	
	
	public void Post (
		string toId = "",
		string link = "",
		string linkName = "",
		string linkCaption = "",
		string linkDescription = "",
		string picture = "",
		string actionName = "",
		string actionLink = "",
		string reference = ""
		) 
	{
		
		if(!IsLoggedIn) { 
			Debug.LogWarning("Auth user before posting, fail event generated");
			
			FBResult res = new FBResult("","User isn't authed");
			FBPostResult pr =  new FBPostResult(res);
			OnPostingCompleteAction(pr);
			return;
		}
		
		FB.Feed(
			toId: toId,
			link: link,
			linkName: linkName,
			linkCaption: linkCaption,
			linkDescription: linkDescription,
			picture: picture,
			actionName : actionName,
			actionLink : actionLink,
			reference : reference,
			callback: PostCallBack
			);
		
	}
	
	
	public void SendTrunRequest(string title, string message, string data = "", string[] to = null) {
		
		string resp = "";
		if(to != null) {
			resp = string.Join(",", to);
		}
		
		AN_FBProxy.SendTrunRequest(title, message, data, resp);
	}
	
	public void SendGift(string title, string message, string objectId, string data = "", string[] to = null ) {
		
		FB.AppRequest(message, OGActionType.Send, objectId,  to, data, title, AppRequestCallBack);
	}
	
	public void AskGift(string title, string message, string objectId, string data = "", string[] to = null ) {
		FB.AppRequest(message, OGActionType.AskFor, objectId,  to, data, title, AppRequestCallBack);
	}
	
	public void SendInvite(string title, string message, string data = "", string[] to = null) {
		FB.AppRequest(message, to, null, null, default(int?), data, title, AppRequestCallBack);
	}
	
	
	private void AppRequestCallBack(FBResult result) {
		
		FBAppRequestResult r =  new FBAppRequestResult();
		r.Result = result;
		
		if(result.Error == null) {
			Dictionary<string, object> JSON = ANMiniJSON.Json.Deserialize(result.Text) as Dictionary<string, object>;
			if(JSON.ContainsKey("request")) {
				r.IsSucceeded = true;
				r.ReuqestId = System.Convert.ToString(JSON["request"]);
			}
			
			
			if(JSON.ContainsKey("to")) {
				List<object> Users = JSON["to"]  as List<object>;
				foreach(object userId in  Users) {
					r.Recipients.Add(System.Convert.ToString(userId));
				}
			}
			
		}
		
		OnAppRequestCompleteAction(r);
		
		Debug.Log("AppRequestCallBack");
		Debug.Log(result.Text);
	}
	
	private void OnAppRequestFailed_AndroidCB(string error) {
		FBResult res = new FBResult("",error);
		
		FBAppRequestResult r =  new FBAppRequestResult();
		r.Result = res;
		OnAppRequestCompleteAction(r);
	}
	
	
	private void OnAppRequestCompleted_AndroidCB(string data) {
		Debug.Log("OnAppRequestCompleted_AndroidCB: " + data);
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		string requestId = storeData[0];
		string to = storeData[1];
		
		
		FBResult result = new FBResult("", "");
		FBAppRequestResult r =  new FBAppRequestResult();
		r.Result = result;
		
		if(requestId.Length > 0) {
			r.IsSucceeded = true;
			r.ReuqestId = requestId;
		}
		string[] list = to.Split(',');
		r.Recipients = new List<string>(list);
		
		OnAppRequestCompleteAction(r);
		
	}
	
	
	public void AppRequest(
		
		string message,
		OGActionType actionType,
		string objectId,
		string[] to,
		string data = "",
		string title = "")
	{
		
		if(!IsLoggedIn) { 
			Debug.LogWarning("Auth user before AppRequest, fail event generated");
			FBResult res = new FBResult("","User isn't authed");
			
			FBAppRequestResult r =  new FBAppRequestResult();
			r.Result = res;
			OnAppRequestCompleteAction(r);
			return;
		}
		
		FB.AppRequest(message, actionType, objectId, to, data, title, AppRequestCallBack);
	}
	
	public void AppRequest(
		string message,
		OGActionType actionType,
		string objectId,
		List<object> filters = null,
		string[] excludeIds = null,
		int? maxRecipients = null,
		string data = "",
		string title = "")
	{
		if(!IsLoggedIn) { 
			Debug.LogWarning("Auth user before AppRequest, fail event generated");
			FBResult res = new FBResult("","User isn't authed");
			
			FBAppRequestResult r =  new FBAppRequestResult();
			r.Result = res;
			OnAppRequestCompleteAction(r);
			
			return;
		}
		
		
		
		FB.AppRequest(message, actionType, objectId, filters, excludeIds, maxRecipients, data, title, AppRequestCallBack);
	}
	
	
	
	public void AppRequest(
		string message,
		string[] to = null,
		List<object> filters = null,
		string[] excludeIds = null,
		int? maxRecipients = null,
		string data = "",
		string title = "")
	{
		if(!IsLoggedIn) { 
			Debug.LogWarning("Auth user before AppRequest, fail event generated");
			FBResult res = new FBResult("","User isn't authed");
			
			FBAppRequestResult r =  new FBAppRequestResult();
			r.Result = res;
			OnAppRequestCompleteAction(r);
			return;
		}
		
		FB.AppRequest(message, to, filters, excludeIds, maxRecipients, data, title, AppRequestCallBack);
	}
	
	
	
	//--------------------------------------
	//  Requests API 
	//--------------------------------------
	
	
	public void LoadPendingRequests() {
		FB.API("/" + FB.UserId + "/apprequests?fields=id,application,data,message,action_type,from,object", Facebook.HttpMethod.GET, OnRequestsLoadComplete);
		
	}
	
	private void OnRequestsLoadComplete(FBResult result) {
		if(result.Error == null) {
			//			Debug.Log(result.Text);
			
			Dictionary<string, object> JSON = ANMiniJSON.Json.Deserialize(result.Text) as Dictionary<string, object>;
			List<object> data = JSON["data"]  as List<object>;
			
			
			AppRequests.Clear();
			foreach(object row in data) {
				
				FBAppRequest request =  new FBAppRequest();
				Dictionary<string, object> dataRow = row as Dictionary<string, object>;
				
				
				
				Dictionary<string, object> AppInfo = dataRow["application"]  as Dictionary<string, object>;
				request.ApplicationId = System.Convert.ToString(AppInfo["id"]);
				if(!request.ApplicationId.Equals(FB.AppId)) {
					break;
				}
				
				
				
				Dictionary<string, object> FromInfo = dataRow["from"]  as Dictionary<string, object>;
				request.FromId = System.Convert.ToString(FromInfo["id"]);
				request.FromName = System.Convert.ToString(FromInfo["name"]);
				
				
				
				
				request.Id = System.Convert.ToString(dataRow["id"]);
				request.SetCreatedTime(System.Convert.ToString(dataRow["created_time"]));
				
				if(dataRow.ContainsKey("data")) {
					request.Data = System.Convert.ToString(dataRow["data"]);
				}
				
				if(dataRow.ContainsKey("message")) {
					request.Message = System.Convert.ToString(dataRow["message"]);
				}
				
				
				if(dataRow.ContainsKey("message")) {
					request.Message = System.Convert.ToString(dataRow["message"]);
				}
				
				
				
				
				if(dataRow.ContainsKey("action_type")) {
					string action_type = System.Convert.ToString(dataRow["action_type"]);
					switch(action_type) {
					case "send":
						request.ActionType = FBAppRequestActionType.Send;
						break;
						
					case "askfor":
						request.ActionType = FBAppRequestActionType.AskFor;
						break;
						
					case "turn":
						request.ActionType = FBAppRequestActionType.Turn;
						break;
					}
					
				}
				
				
				if(dataRow.ContainsKey("object")) {
					FBObject obj = new FBObject();
					Dictionary<string, object> objectData = dataRow["object"] as Dictionary<string, object>;
					
					obj.Id = System.Convert.ToString(objectData["id"]);
					obj.Title = System.Convert.ToString(objectData["id"]);
					obj.Type = System.Convert.ToString(objectData["id"]);
					obj.SetCreatedTime(System.Convert.ToString(objectData["created_time"]));
					
					if(objectData.ContainsKey("image")) {
						
						List<object> images = objectData["image"] as List<object>;
						Debug.Log(objectData["image"]);
						foreach(object img in images) {
							Dictionary<string, object>imgData = img as Dictionary<string, object>;
							obj.AddImageUrl(System.Convert.ToString(imgData["url"]));
						}
						
						
					}
					
					request.Object = obj;
					
				} 
				
				
				AppRequests.Add(request);
			}
			
			Debug.Log("SPFacebook: " + AppRequests.Count +  "App request Loaded");
			
		} else {
			Debug.LogWarning("SPFacebook: App requests failed to load");
			Debug.LogWarning(result.Error.ToString());
		}
		
		
		OnAppRequestsLoaded(result);
	}
	
	
	//--------------------------------------
	//  Scores API 
	//  https://developers.facebook.com/docs/games/scores
	//------------------------------------
	
	
	
	//Read score for a player
	public void LoadPlayerScores() {
		FB.API("/" + FB.UserId + "/scores", Facebook.HttpMethod.GET, OnLoaPlayrScoresComplete);  
	}
	
	//Read scores for players and friends
	public void LoadAppScores() {
		FB.API("/" + FB.AppId + "/scores", Facebook.HttpMethod.GET, OnAppScoresComplete);  
	}
	
	//Create or update a score
	public void SubmitScore(int score) {
		lastSubmitedScore = score;
		FB.API("/" + FB.UserId + "/scores?score=" + score, Facebook.HttpMethod.POST, OnScoreSubmited);  
	}
	
	//Delete scores for a player
	public void DeletePlayerScores() {
		FB.API("/" + FB.UserId + "/scores", Facebook.HttpMethod.DELETE, OnScoreDeleted); 
		
		
	}
	
	
	
	//--------------------------------------
	//  Likes API 
	//  https://developers.facebook.com/docs/graph-api/reference/v2.0/user/likes
	//------------------------------------
	
	public void LoadCurrentUserLikes() {
		LoadLikes(FB.UserId);
	}
	
	
	public void LoadLikes(string userId) {
		FBLikesRetrieveTask task = FBLikesRetrieveTask.Create();
		task.ActionComplete += OnUserLikesResult;
		task.LoadLikes(userId);
	}
	
	public void LoadLikes(string userId, string pageId) {
		FBLikesRetrieveTask task = FBLikesRetrieveTask.Create();
		task.ActionComplete += OnUserLikesResult;
		task.LoadLikes(userId, pageId);
	}
	
	
	//--------------------------------------
	//  Payment API 
	//  https://developers.facebook.com/docs/unity/reference/current/FB.Canvas.Pay
	//------------------------------------
	
	
	public void Pay (string product,  int quantity = 1) {
		Pay (product, "purchaseitem", quantity);
	}
	
	public void Pay ( string product,
	                 string action = "purchaseitem",
	                 int quantity = 1,
	                 int? quantityMin = null,
	                 int? quantityMax = null,
	                 string requestId = null,
	                 string pricepointId = null,
	                 string testCurrency = null
	                 ) {
		
		
		
		FB.Canvas.Pay (product, action, quantity, quantityMin, quantityMax, requestId, pricepointId, testCurrency, FBPaymentCallBack);
	}
	
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	
	
	public FBScore GetCurrentPlayerScoreByAppId(string appId) {
		if(_userScores.ContainsKey(appId)) {
			return _userScores[appId];
		} else {
			FBScore score =  new FBScore();
			score.UserId = FB.UserId;
			score.AppId = appId;
			score.value = 0;
			
			return score;
		}
	}
	
	
	public int GetCurrentPlayerIntScoreByAppId(string appId) {
		return GetCurrentPlayerScoreByAppId(appId).value;
	}
	
	
	
	
	public int GetScoreByUserId(string userId) {
		if(_appScores.ContainsKey(userId)) {
			return _appScores[userId].value;
		} else {
			return 0;
		}
	}
	
	public FBScore GetScoreObjectByUserId(string userId) {
		if(_appScores.ContainsKey(userId)) {
			return _appScores[userId];
		} else {
			return null;
		}
	}
	
	
	
	public List<FBLikeInfo> GerUserLikesList(string userId){
		
		List<FBLikeInfo>  result = new List<FBLikeInfo>();
		
		if(_likes.ContainsKey(userId)) {
			foreach(KeyValuePair<string,  FBLikeInfo>  pair in _likes[userId]) {
				result.Add(pair.Value);
			}
		}
		
		return result;
	}
	
	public bool IsUserLikesPage(string userId, string pageId) {
		if(_likes.ContainsKey(userId)) {
			if(_likes[userId].ContainsKey(pageId)) {
				return true;
			}
		}
		
		return false;
	}
	
	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	
	public bool IsInited  {
		get {
			return _IsInited;
		}
	}
	
	public bool IsLoggedIn {
		get {
			return FB.IsLoggedIn;
		}
	}
	
	
	public string UserId {
		get {
			return FB.UserId;
		}
	}
	
	public string AccessToken {
		get {
			return FB.AccessToken;
		}
	}
	
	public FacebookUserInfo userInfo {
		get {
			return _userInfo;
		}
	}
	
	public Dictionary<string,  FacebookUserInfo> friends {
		get {
			return _friends;
		}
	}
	
	public List<string> friendsIds {
		get {
			if(_friends == null) {
				return null;
			}
			
			List<string> ids = new List<string>();
			foreach(KeyValuePair<string, FacebookUserInfo> item in _friends) {
				ids.Add(item.Key);
			}
			
			return ids;
		}
	}
	
	public List<FacebookUserInfo> friendsList {
		get {
			if(_friends == null) {
				return null;
			}
			
			List<FacebookUserInfo> flist = new List<FacebookUserInfo>();
			foreach(KeyValuePair<string, FacebookUserInfo> item in _friends) {
				flist.Add(item.Value);
			}
			
			return flist;
		}
	}
	
	
	
	public  Dictionary<string,  FBScore> userScores  {
		get {
			return _userScores;
		}
	}
	
	public  Dictionary<string,  FBScore>  appScores{
		get {
			return _appScores;
		}
	}
	
	
	public List<FBScore> applicationScoreList {
		get {
			List<FBScore>  result = new List<FBScore>();
			foreach(KeyValuePair<string,  FBScore>  pair in _appScores) {
				result.Add(pair.Value);
			}
			
			return result;
		}
	}
	
	public List<FBAppRequest> AppRequests {
		get {
			return _AppRequests;
		}
	}
	
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	private void OnUserLikesResult(FBResult result, FBLikesRetrieveTask task) {
		
		
		FB_APIResult r;
		if(result.Error != null) {
			r = new FB_APIResult(false, result.Error);
			r.Unity_FB_Result = result;
			OnLikesListLoadedAction(r);
			return;
		}
		
		
		Dictionary<string, object> JSON = ANMiniJSON.Json.Deserialize(result.Text) as Dictionary<string, object>;
		List<object> data = JSON["data"]  as List<object>;
		
		
		Dictionary<string, FBLikeInfo> userLikes = null;
		if(_likes.ContainsKey(task.userId)) {
			userLikes = _likes[task.userId];
		} else {
			userLikes =  new Dictionary<string, FBLikeInfo>();
			_likes.Add(task.userId, userLikes);
		}
		
		foreach(object row in data) {
			Dictionary<string, object> dataRow = row as Dictionary<string, object>;
			
			FBLikeInfo tpl =  new FBLikeInfo();
			tpl.id 			= System.Convert.ToString(dataRow["id"]);
			tpl.name 		= System.Convert.ToString(dataRow["name"]);
			tpl.category 	= System.Convert.ToString(dataRow["category"]);
			
			if(userLikes.ContainsKey(tpl.id)) {
				userLikes[tpl.id] = tpl;
			} else {
				userLikes.Add(tpl.id, tpl);
			}
		}
		
		r = new FB_APIResult(true, result.Text);
		r.Unity_FB_Result = result;
		OnLikesListLoadedAction(r);
	}
	
	
	
	
	private void OnScoreDeleted(FBResult result) {
		FB_APIResult r;
		if(result.Error != null) {
			r = new FB_APIResult(false, result.Error);
			r.Unity_FB_Result = result;
			OnDeleteScoresRequestCompleteAction(r);
			return;
		}
		
		
		if(result.Text.Equals("true")) {
			r = new FB_APIResult(true, result.Text);
			r.Unity_FB_Result = result;
			
			FBScore score = new FBScore();
			score.AppId = FB.AppId;
			score.UserId = FB.UserId;
			score.value = 0;
			
			if(_appScores.ContainsKey(FB.UserId)) {
				_appScores[FB.UserId].value = 0;
			}  else {
				_appScores.Add(score.UserId, score);
			}
			
			
			if(_userScores.ContainsKey(FB.AppId)) {
				_userScores[FB.AppId].value = 0;
			} else {
				_userScores.Add(FB.AppId, score); 
			}
			
			
			OnDeleteScoresRequestCompleteAction(r);
			
			
		} else {
			r = new FB_APIResult(false, result.Error);
			r.Unity_FB_Result = result;
			OnDeleteScoresRequestCompleteAction(r);
		}
		
		
	}
	
	private void OnScoreSubmited(FBResult result) {
		
		FB_APIResult r;
		if(result.Error != null) {
			r = new FB_APIResult(false, result.Error);
			r.Unity_FB_Result = result;
			OnSubmitScoreRequestCompleteAction(r);
			return;
		}
		
		
		if(result.Text.Equals("true")) {
			r = new FB_APIResult(true, result.Text);
			r.Unity_FB_Result = result;
			
			FBScore score = new FBScore();
			score.AppId = FB.AppId;
			score.UserId = FB.UserId;
			score.value = lastSubmitedScore;
			
			if(_appScores.ContainsKey(FB.UserId)) {
				_appScores[FB.UserId].value = lastSubmitedScore;
			}  else {
				_appScores.Add(score.UserId, score);
			}
			
			
			if(_userScores.ContainsKey(FB.AppId)) {
				_userScores[FB.AppId].value = lastSubmitedScore;
			} else {
				_userScores.Add(FB.AppId, score); 
			}
			
			
			OnSubmitScoreRequestCompleteAction(r);
			
			
		} else {
			r = new FB_APIResult(false, result.Error);
			r.Unity_FB_Result = result;
			OnSubmitScoreRequestCompleteAction(r);
		}
	}
	
	
	private void OnAppScoresComplete(FBResult result) {
		FB_APIResult r;
		if(result.Error != null) {
			r = new FB_APIResult(false, result.Error);
			r.Unity_FB_Result = result;
			OnAppScoresRequestCompleteAction(r);
			return;
		}
		
		Dictionary<string, object> JSON = ANMiniJSON.Json.Deserialize(result.Text) as Dictionary<string, object>;
		List<object> data = JSON["data"]  as List<object>;
		
		foreach(object row in data) {
			FBScore score =  new FBScore();
			Dictionary<string, object> dataRow = row as Dictionary<string, object>;

			if (dataRow.ContainsKey("user")) {
			
				Dictionary<string, object> userInfo = dataRow["user"]  as Dictionary<string, object>;

				if (userInfo.ContainsKey("id")) {
					score.UserId = System.Convert.ToString(userInfo["id"]);
				}

				if (userInfo.ContainsKey("name")) {
					score.UserName = System.Convert.ToString(userInfo["name"]);
				}
				
				
			}


			if (dataRow.ContainsKey("score")) {
				score.value = System.Convert.ToInt32(dataRow["score"]); 
			}


			if (dataRow.ContainsKey("application")) {
				Dictionary<string, object> AppInfo = dataRow["application"]  as Dictionary<string, object>;

				if (AppInfo.ContainsKey("id")) {
					score.AppId = System.Convert.ToString(AppInfo["id"]);
				}

				if (AppInfo.ContainsKey("name")) {
					score.AppName = System.Convert.ToString(AppInfo["name"]);
				}

			}

			
			AddToAppScores(score);
			
			
		}
		
		r = new FB_APIResult(true, result.Text);
		r.Unity_FB_Result = result;
		OnAppScoresRequestCompleteAction(r);
	}
	
	private void AddToAppScores(FBScore score) {
		
		if(_appScores.ContainsKey(score.UserId)) {
			_appScores[score.UserId] = score;
		} else {
			_appScores.Add(score.UserId, score);
		}
		
		if(_userScores.ContainsKey(score.AppId)) {
			_userScores[score.AppId] = score;
		} else {
			_userScores.Add(score.AppId, score);
		}
		
		
		
		
	}
	
	private void AddToUserScores(FBScore score) {
		if(_userScores.ContainsKey(score.AppId)) {
			_userScores[score.AppId] = score;
		} else {
			_userScores.Add(score.AppId, score);
		}
		
		
		if(_appScores.ContainsKey(score.UserId)) {
			_appScores[score.UserId] = score;
		} else {
			_appScores.Add(score.UserId, score);
		}
		
	}
	
	private void OnLoaPlayrScoresComplete(FBResult result) {
		
		
		FB_APIResult r;
		if(result.Error != null) {
			r = new FB_APIResult(false, result.Error);
			r.Unity_FB_Result = result;
			OnPlayerScoresRequestCompleteAction(r);
			return;
		}
		
		Dictionary<string, object> JSON = ANMiniJSON.Json.Deserialize(result.Text) as Dictionary<string, object>;
		List<object> data = JSON["data"]  as List<object>;
		
		foreach(object row in data) {
			FBScore score =  new FBScore();
			Dictionary<string, object> dataRow = row as Dictionary<string, object>;
			
			Dictionary<string, object> userInfo = dataRow["user"]  as Dictionary<string, object>;
			
			score.UserId = System.Convert.ToString(userInfo["id"]);
			score.UserName = System.Convert.ToString(userInfo["name"]);
			
			
			score.value = System.Convert.ToInt32(dataRow["score"]); 
			
			
			Dictionary<string, object> AppInfo = dataRow["application"]  as Dictionary<string, object>;
			
			score.AppId = System.Convert.ToString(AppInfo["id"]);
			score.AppName = System.Convert.ToString(AppInfo["name"]);
			
			
			AddToUserScores(score);
			
		}
		
		r = new FB_APIResult(true, result.Text);
		r.Unity_FB_Result = result;
		OnPlayerScoresRequestCompleteAction(r);
		
	}
	
	private void resultTest(FBResult result) {
		Debug.Log(result.Error);
		Debug.Log(result.Text);
	}
	
	
	
	
	
	private void PostCallBack(FBResult result) {
		FBPostResult pr = new FBPostResult(result);
		OnPostingCompleteAction(pr);
		
	}
	
	
	private void FriendsDataCallBack(FBResult result) {
		if (result.Error != null)  {                                                                                                                          
			Debug.LogWarning(result.Error);
		}  else {
			ParceFriendsData(result.Text);
		}        
		
		OnFriendsDataRequestCompleteAction(result);
	}
	
	
	public void ParceFriendsData(string data) {
		
		Debug.Log("ParceFriendsData");
		Debug.Log(data);
		
		try {
			_friends =  new Dictionary<string, FacebookUserInfo>();
			IDictionary JSON =  ANMiniJSON.Json.Deserialize(data) as IDictionary;	
			IDictionary f = JSON["friends"] as IDictionary;
			IList flist = f["data"] as IList;
			
			
			for(int i = 0; i < flist.Count; i++) {
				FacebookUserInfo user = new FacebookUserInfo(flist[i] as IDictionary);
				_friends.Add(user.id, user);
			}
			
		} catch(System.Exception ex) {
			Debug.LogWarning("Parceing Friends Data failed");
			Debug.LogWarning(ex.Message);
		}
		
	}
	
	private void ScoreLoadResult(FBResult result) {
		Debug.Log(result.Text);
	}
	
	private void UserDataCallBack(FBResult result) {
		if (result.Error != null)  {         
			Debug.LogWarning(result.Error);
		}   else {
			_userInfo = new FacebookUserInfo(result.Text);
		}       
		
		
		OnUserDataRequestCompleteAction(result);
		
	}
	
	
	private void OnInitComplete() {
		_IsInited = true;
		IsLoginRequestSent = false;
		OnInitCompleteAction();
		Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
	}
	
	
	
	private void OnHideUnity(bool isGameShown) {
		OnFocusChangedAction(isGameShown);
	}
	
	
	
	private FBResult LoginCallbackResult = null;
	
	private void LoginCallback(FBResult result) {
		LoginCallbackResult = result;
		BroadcastLoginResult();
	}
	
	
	private void BroadcastLoginResult() {
		if(LoginCallbackResult != null) {
			IsLoginRequestSent = false;
			OnAuthCompleteAction(LoginCallbackResult);
		}
	}
	
	private void FBPaymentCallBack (FBResult result) {
		OnPaymentCompleteAction(result);
	}	
	
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------
	
}
