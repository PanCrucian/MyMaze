////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class GameCenterManager : MonoBehaviour {
	
	//Actions
	public static event Action<ISN_Result> OnAuthFinished  = delegate{};

	public static event Action<ISN_Result> OnScoreSubmitted = delegate{};
	public static event Action<GK_PlayerScoreLoadedResult> OnPlayerScoreLoaded = delegate{};
	public static event Action<ISN_Result> OnScoresListLoaded = delegate{};

	public static event Action<ISN_Result> OnAchievementsReset = delegate{};
	public static event Action<ISN_Result> OnAchievementsLoaded  = delegate{};
	public static event Action<GK_AchievementProgressResult> OnAchievementsProgress  = delegate{};

	public static event Action<ISN_Result> OnLeaderboardSetsInfoLoaded = delegate{};


	public static event Action OnGameCenterViewDismissed = delegate{};
	public static event Action<ISN_Result> OnFriendsListLoaded = delegate{};
	public static event Action<GK_UserInfoLoadResult> OnUserInfoLoaded  = delegate{};
	public static event Action<GK_PlayerSignatureResult> OnPlayerSignatureRetrieveResult = delegate{};




	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _initGameCenter();
	
	[DllImport ("__Internal")]
	private static extern void _showLeaderboard(string leaderboardId, int timeSpan);

	[DllImport ("__Internal")]
	private static extern void _reportScore (string score, string leaderboardId);



	[DllImport ("__Internal")]
	private static extern void _showLeaderboards ();


	[DllImport ("__Internal")]
	private static extern void _getLeaderboardScore (string leaderboardId, int timeSpan, int collection);

	[DllImport ("__Internal")]
	private static extern void _loadLeaderboardScore (string leaderboardId, int timeSpan, int collection, int from, int to);
	
	[DllImport ("__Internal")]
	private static extern void _showAchievements();

	[DllImport ("__Internal")]
	private static extern void _resetAchievements();
	

	[DllImport ("__Internal")]
	private static extern void _submitAchievement(float percent, string achievementId, bool isCompleteNotification);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_issueLeaderboardChallenge(string leaderboardId, string message, string playerIds);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueLeaderboardChallengeWithFriendsPicker(string leaderboardId, string message);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueAchievementChallenge(string leaderboardId, string message, string playerIds);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueAchievementChallengeWithFriendsPicker(string leaderboardId, string message);

	[DllImport ("__Internal")]
	private static extern void _ISN_RetrieveFriends();

	[DllImport ("__Internal")]
	private static extern void _ISN_loadGKPlayerData(string playerId);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_loadGKPlayerPhoto(string playerId, int size);

	[DllImport ("__Internal")]
	private static extern void _ISN_getSignature();

	[DllImport ("__Internal")]
	private static extern void _ISN_loadLeaderboardSetInfo();
	
	[DllImport ("__Internal")]
	private static extern void _ISN_loadLeaderboardsForSet(string setId);


	
	#endif


	private  static bool _IsInitialized = false;
	private  static bool _IsPlayerAuthenticated = false;
	private  static bool _IsAchievementsInfoLoaded = false;



	private static List<GK_AchievementTemplate> _achievements = new List<GK_AchievementTemplate> ();

	private static Dictionary<string, GK_Leaderboard> _leaderboards =  new Dictionary<string, GK_Leaderboard>();
	private static Dictionary<string, GK_Player> _players =  new Dictionary<string, GK_Player>();
	private static List<string> _friendsList = new List<string>();
	private static List<GK_LeaderboardSet> _LeaderboardSets = new List<GK_LeaderboardSet>();


	private static GK_Player _player = null;


	private const string ISN_GC_PP_KEY = "ISN_GameCenterManager";

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------



	void Awake() {
		foreach(string aId in IOSNativeSettings.Instance.RegisteredAchievementsIds) {
			RegisterAchievement(aId);
		}
	}

	[System.Obsolete("init is deprecated, please use Init instead.")]
	public static void init() {
		Init();
	}

	public static void Init() {
		
		if(_IsInitialized) {
			return;
		}
		
		_IsInitialized = true;
		
		GameCenterInvitations.Instance.Init();
		
		GameObject go =  new GameObject("GameCenterManager");
		go.AddComponent<GameCenterManager>();
		DontDestroyOnLoad(go);
		
		
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_initGameCenter();
		#endif
		
	}


	public static void RetrievePlayerSignature() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_getSignature();
		#endif
	}



	public static void RegisterAchievement(string achievementId) {


		bool isContains = false;

		foreach(GK_AchievementTemplate t in _achievements) {
			if (t.Id.Equals (achievementId)) {
				isContains = true;
			}
		}


		if(!isContains) {
			GK_AchievementTemplate tpl = new GK_AchievementTemplate ();
			tpl.Id = achievementId;
			tpl.Progress = 0;
			_achievements.Add (tpl);
		}
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	

	public static void ShowLeaderboard(string leaderboardId) {
		ShowLeaderboard(leaderboardId, GK_TimeSpan.ALL_TIME);
	}


	public static void ShowLeaderboard(string leaderboardId, GK_TimeSpan timeSpan) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_showLeaderboard(leaderboardId, (int) timeSpan);
		#endif
	}

	public static void ShowLeaderboards() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_showLeaderboards ();
		#endif
	}
	

	public static void ReportScore(long score, string leaderboardId) {
		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			Debug.Log("unity reportScore: " + leaderboardId);

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_reportScore(score.ToString(), leaderboardId);
		#endif
	}


	public static void ReportScore(double score, string leaderboardId) {
		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			Debug.Log("unity reportScore double: " + leaderboardId);
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		long s = System.Convert.ToInt64(score * 100);
		_reportScore(s.ToString(), leaderboardId);
		#endif
	}



	public static void RetrieveFriends() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_RetrieveFriends();
		#endif
	}

	[System.Obsolete("LoadUsersData is deprecated, please use LoadGKPlayerInfo instead.")]
	public static void LoadUsersData(string[] UIDs) {
		LoadGKPlayerInfo(UIDs[0]);
	}
	
	public static void LoadGKPlayerInfo(string playerId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_loadGKPlayerData(playerId);
		#endif
	}
	
	public static void LoadGKPlayerPhoto(string playerId, GK_PhotoSize size) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_loadGKPlayerPhoto(playerId, (int) size);
		#endif
	}

	

	public static void LoadCurrentPlayerScore(string leaderboardId, GK_TimeSpan timeSpan = GK_TimeSpan.ALL_TIME, GK_CollectionType collection = GK_CollectionType.GLOBAL)  {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_getLeaderboardScore(leaderboardId, (int) timeSpan, (int) collection);
		#endif
	}
	

	private IEnumerator LoadCurrentPlayerScoreLocal(string leaderboardId, GK_TimeSpan timeSpan = GK_TimeSpan.ALL_TIME, GK_CollectionType collection = GK_CollectionType.GLOBAL ) {
		yield return new WaitForSeconds(4f);
		LoadCurrentPlayerScore(leaderboardId, timeSpan, collection);
	}



	
	public static void LoadScore(string leaderboardId, int from, int to, GK_TimeSpan timeSpan = GK_TimeSpan.ALL_TIME, GK_CollectionType collection = GK_CollectionType.GLOBAL) {

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_loadLeaderboardScore(leaderboardId, (int) timeSpan, (int) collection, from, to);
		#endif

	}


	public static void IssueLeaderboardChallenge(string leaderboardId, string message, string playerId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueLeaderboardChallenge(leaderboardId, message, playerId);
		#endif
	}

	public static void IssueLeaderboardChallenge(string leaderboardId, string message, string[] playerIds) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			string ids = "";
			int len = playerIds.Length;
			for(int i = 0; i < len; i++) {
				if(i != 0) {
					ids += ",";
				}
				
				ids += playerIds[i];
			}

		_ISN_issueLeaderboardChallenge(leaderboardId, message, ids);
		#endif
	}


	public static void IssueLeaderboardChallenge(string leaderboardId, string message) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueLeaderboardChallengeWithFriendsPicker(leaderboardId, message);
		#endif
	}




	public static void IssueAchievementChallenge(string achievementId, string message, string playerId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueAchievementChallenge(achievementId, message, playerId);
		#endif
	}

	public static void LoadLeaderboardSetInfo() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_loadLeaderboardSetInfo();
		#endif
	}

	public static void LoadLeaderboardsForSet(string setId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_loadLeaderboardsForSet(setId);
		#endif
	}




	public static void IssueAchievementChallenge(string achievementId, string message, string[] playerIds) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			string ids = "";
			int len = playerIds.Length;
			for(int i = 0; i < len; i++) {
				if(i != 0) {
					ids += ",";
				}
				
				ids += playerIds[i];
			}
			
		_ISN_issueAchievementChallenge(achievementId, message, ids);
		#endif
	}



	public static void IssueAchievementChallenge(string achievementId, string message) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueAchievementChallengeWithFriendsPicker(achievementId, message);
		#endif
	}


	public static void ShowAchievements() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_showAchievements();
		#endif
	}

	public static void ResetAchievements() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_resetAchievements();

			foreach(GK_AchievementTemplate tpl in _achievements) {
				tpl.Progress = 0f;
			}
		#endif

		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			ResetStoredProgress();
		}
	}


	public static void SubmitAchievement(float percent, string achievementId) {
		SubmitAchievement (percent, achievementId, true);
	}

	public static void SubmitAchievementNoCache(float percent, string achievementId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_submitAchievement(percent, achievementId, false);
		#endif
	}

	public static void SubmitAchievement(float percent, string achievementId, bool isCompleteNotification) {

		if(Application.internetReachability == NetworkReachability.NotReachable) {
			ISN_CacheManager.SaveAchievementRequest(achievementId, percent);
		}


		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			SaveAchievementProgress(achievementId, percent);
		}


		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_submitAchievement(percent, achievementId, isCompleteNotification);
		#endif
	}




	public static float GetAchievementProgress(string id) {
		float progress = 0f;

		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			progress = GetStoredAchievementProgress(id);
		} else {
			foreach(GK_AchievementTemplate tpl in _achievements) {
				if(tpl.Id == id) {
					return tpl.Progress;
				}
			}
		}

		return progress;
	}

	public static GK_Leaderboard GetLeaderboard(string id) {
		if(_leaderboards.ContainsKey(id)) {
			return _leaderboards[id];
		} else {
			return null;
		}
	}


	public static GK_Player GetPlayerById(string playerID) {
		if(_players.ContainsKey(playerID)) {
			//Debug.Log("Returning player object for id: " + playerID);
			return _players[playerID];
		} else {
			//Debug.Log("player not found with id: " + playerID);
			return null;
		}
	}
	

	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public static List<GK_AchievementTemplate> Achievements {
		get {
			return _achievements;
		}
	}


	public static Dictionary<string, GK_Player> Players {
		get {
			return _players;
		}
	}
	

	public static GK_Player Player {
		get {
			return _player;
		}
	}


	public static bool IsInitialized {
		get {
			return _IsInitialized;
		}
	}

	public static List<GK_LeaderboardSet> LeaderboardSets {
		get {
			return _LeaderboardSets;
		}
	}

	public static bool IsPlayerAuthenticated {
		get {
			return _IsPlayerAuthenticated;
		}
	}

	public static bool IsAchievementsInfoLoaded {
		get {
			return _IsAchievementsInfoLoaded;
		}
	}

	public static List<string> FriendsList {
		get {
			return _friendsList;
		}
	}



	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void onLeaderboardScoreFailed(string errorData) {
		GK_PlayerScoreLoadedResult result = new GK_PlayerScoreLoadedResult (errorData);
		OnPlayerScoreLoaded (result);
	}


	private void onLeaderboardScore(string array) {

		string[] data;
		data = array.Split("," [0]);

		string lbId = data[0];
		string scoreVal = data[1];
		int rank = System.Convert.ToInt32(data[2]);


		GK_TimeSpan timeSpan = (GK_TimeSpan) System.Convert.ToInt32(data[3]);
		GK_CollectionType collection = (GK_CollectionType) System.Convert.ToInt32(data[4]);

		GK_Leaderboard board;
		if(_leaderboards.ContainsKey(lbId)) {
			board = _leaderboards[lbId];
		} else {
			board =  new GK_Leaderboard(lbId);
			_leaderboards.Add(lbId, board);
		}


		GK_Score score =  new GK_Score(scoreVal, rank, timeSpan, collection, lbId, Player.Id);

		board.UpdateScore(score);
		board.UpdateCurrentPlayerRank(rank, timeSpan, collection);
	

		GK_PlayerScoreLoadedResult result = new GK_PlayerScoreLoadedResult (score);
		OnPlayerScoreLoaded (result);
	}

	
	public void onScoreSubmittedEvent(string array) {
		
		string[] data;
		data = array.Split("," [0]);
		
		string lbId = data[0];
		//string score =  data[1];


		StartCoroutine(LoadCurrentPlayerScoreLocal(lbId));

		
		ISN_Result result = new ISN_Result (true);
		OnScoreSubmitted (result);
	}




	
	public void onScoreSubmittedFailed(string errorData) {
		ISN_Result result = new ISN_Result (errorData);
		OnScoreSubmitted (result);
	}


	private void onLeaderboardScoreListLoaded(string array) {


		
		string[] data;
		data = array.Split("," [0]);

		string lbId = data[0];
		GK_TimeSpan timeSpan = (GK_TimeSpan) System.Convert.ToInt32(data[1]);
		GK_CollectionType collection = (GK_CollectionType) System.Convert.ToInt32(data[2]);

		GK_Leaderboard board;
		if(_leaderboards.ContainsKey(lbId)) {
			board = _leaderboards[lbId];
		} else {
			board =  new GK_Leaderboard(lbId);
			_leaderboards.Add(lbId, board);
		}


	
		
		
		for(int i = 3; i < data.Length; i+=3) {
			string playerId = data[i];
			string scoreVal = data[i + 1];
			int rank = System.Convert.ToInt32(data[i + 2]);

			GK_Score score =  new GK_Score(scoreVal, rank, timeSpan, collection, lbId, playerId);
			board.UpdateScore(score);
			if(Player != null) {
				if(Player.Id.Equals(playerId)) {
					board.UpdateCurrentPlayerRank(rank, timeSpan, collection);
				}
			}
		}
		


		ISN_Result result = new ISN_Result (true);
		OnScoresListLoaded (result);


	}

	private void onLeaderboardScoreListLoadFailed(string errorData) {

		ISN_Result result;
		if(errorData.Length > 0) {
			result = new ISN_Result (errorData);
		} else {
			result = new ISN_Result (false);
		}

		OnScoresListLoaded (result);
	}



	private void onAchievementsReset(string array) {

		ISN_Result result = new ISN_Result (true);
		OnAchievementsReset (result);

	}

	private void onAchievementsResetFailed(string errorData) {
		ISN_Result result = new ISN_Result (errorData);
		OnAchievementsReset (result);
	}


	private void onAchievementProgressChanged(string array) {
		string[] data;
		data = array.Split("," [0]);



		GK_AchievementTemplate tpl =  new GK_AchievementTemplate();
		tpl.Id = data [0];
		tpl.Progress = System.Convert.ToSingle(data [1]) ;
		GK_AchievementProgressResult result = new GK_AchievementProgressResult(tpl);

		submitAchievement (tpl);

		OnAchievementsProgress(result);

	}


	private void onAchievementsLoaded(string array) {

		ISN_Result result = new ISN_Result (true);
		if(array.Equals(string.Empty)) {
			OnAchievementsLoaded (result);
			return;
		}

		string[] data;
		data = array.Split("," [0]);


		for(int i = 0; i < data.Length; i+=3) {
			GK_AchievementTemplate tpl =  new GK_AchievementTemplate();
			tpl.Id 				= data[i];
			tpl.Description 	= data[i + 1];
			tpl.Progress 		= System.Convert.ToSingle(data[i + 2]);
			submitAchievement (tpl);
		}

		_IsAchievementsInfoLoaded = true;
		OnAchievementsLoaded (result);
	}


	private void onAchievementsLoadedFailed(string errorData) {
		ISN_Result result = new ISN_Result (errorData);
		OnAchievementsLoaded (result);
	}


	private void onAuthenticateLocalPlayer(string  array) {
		string[] data;
		data = array.Split("," [0]);

		_player = new GK_Player (data[0], data [1], data [2]);


		ISN_CacheManager.SendAchievementCachedRequest();

		_IsPlayerAuthenticated = true;


		ISN_Result result = new ISN_Result (_IsPlayerAuthenticated);
		OnAuthFinished (result);
	}
	
	
	private void onAuthenticationFailed(string  errorData) {

		_IsPlayerAuthenticated = false;

		ISN_Result result;
		if(errorData.Length > 0) {
			result = new ISN_Result(errorData);
		} else {
			result	= new ISN_Result (_IsPlayerAuthenticated);
		}

			
		OnAuthFinished (result);
	}


	private void OnUserInfoLoadedEvent(string array) {
		Debug.Log("OnUserInfoLoadedEvent");

		string[] data = array.Split(IOSNative.DATA_SPLITTER);

		string playerId = data[0];
		string alias = data[1];
		string displayName = data[2];


		GK_Player p =  new GK_Player(playerId, displayName, alias);


		if(_players.ContainsKey(playerId)) {
			_players[playerId] = p;
		} else {
			_players.Add(playerId, p);
		}

		if(p.Id == _player.Id) {
			_player = p;
		}

		Debug.Log("Player Info loaded, for player with id: " + p.Id);

		GK_UserInfoLoadResult result = new GK_UserInfoLoadResult (p);
		OnUserInfoLoaded (result);

	}    
	
	private void OnUserInfoLoadFailedEvent(string playerId) {
		
		GK_UserInfoLoadResult result = new GK_UserInfoLoadResult (playerId);
		OnUserInfoLoaded (result);
	}


	public void OnUserPhotoLoadedEvent(string array) {
		string[] data = array.Split(IOSNative.DATA_SPLITTER);
		
		string playerId = data[0];
		GK_PhotoSize size = (GK_PhotoSize) System.Convert.ToInt32(data[1]);
		string encodedImage = data[2];

		GK_Player player = GetPlayerById(playerId);
		if(player != null) {
			player.SetPhotoData(size, encodedImage);
		}

	}

	private void OnUserPhotoLoadFailedEvent(string data) {
		string[] DataArray = data.Split(new string[] { IOSNative.DATA_SPLITTER2 }, StringSplitOptions.None);

		string playerId = DataArray[0];
		GK_PhotoSize size = (GK_PhotoSize) System.Convert.ToInt32(DataArray[1]);
		string errorData = DataArray[2];

		GK_Player player = GetPlayerById(playerId);
		if(player != null) {
			player.SetPhotoLoadFailedEventData(size, errorData);
		}
		
	}


	private void OnFriendListLoadedEvent(string data) {


		string[] fl;
		fl = data.Split(IOSNative.DATA_SPLITTER);

		for(int i = 0; i < fl.Length; i++) {
			_friendsList.Add(fl[i]);
		}

		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			Debug.Log("Friends list loaded, total friends: " + _friendsList.Count);


		ISN_Result result = new ISN_Result (true);
		OnFriendsListLoaded (result);

	}

	private void OnFriendListLoadFailEvent(string errorData) {
		ISN_Result result = new ISN_Result (errorData);
		OnFriendsListLoaded (result);
	}


	private void OnGameCenterViewDismissedEvent(string data) {
		OnGameCenterViewDismissed();
	}



	private void VerificationSignatureRetrieveFailed(string array) {

		ISN_Error error =  new ISN_Error(array);
	

		GK_PlayerSignatureResult res =  new GK_PlayerSignatureResult(error);
		OnPlayerSignatureRetrieveResult(res);

	}

	private void VerificationSignatureRetrieved(string array) {
		string[] data;
		data = array.Split(IOSNative.DATA_SPLITTER);

		GK_PlayerSignatureResult res =  new GK_PlayerSignatureResult(data[0], data[1], data[2], data[3]);
		OnPlayerSignatureRetrieveResult(res);
	}



	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------

	private void submitAchievement(GK_AchievementTemplate tpl) {
		bool isContains = false;
		foreach(GK_AchievementTemplate t in _achievements) {
			if (t.Id.Equals (tpl.Id)) {
				isContains = true;
				t.Progress = tpl.Progress;
			}
		}

		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			SaveAchievementProgress(tpl.Id, tpl.Progress);
		}

		if(!isContains) {
			_achievements.Add (tpl);
		}
	}


	private static void ResetStoredProgress() {
		foreach(GK_AchievementTemplate t in _achievements) {
			PlayerPrefs.DeleteKey(ISN_GC_PP_KEY + t.Id);
		}
	}

	private static void SaveAchievementProgress(string achievementId, float progress) {

		float currentProgress =  GetStoredAchievementProgress(achievementId);
		if(progress > currentProgress) {
			PlayerPrefs.SetFloat(ISN_GC_PP_KEY + achievementId, progress);
		}
	}

	private static float GetStoredAchievementProgress(string achievementId) {
		float v = 0f;
		if(PlayerPrefs.HasKey(ISN_GC_PP_KEY + achievementId)) {
			v = PlayerPrefs.GetFloat(ISN_GC_PP_KEY + achievementId);
		} 

		return v;
	}

	private void ISN_OnLBSetsLoaded(string array) {

		string[] data = array.Split("|" [0]);

		for(int i = 0; i < data.Length; i+=3) {
			GK_LeaderboardSet lbSet =  new GK_LeaderboardSet();
			lbSet.Title = data[i];
			lbSet.Identifier = data[i + 1];
			lbSet.GroupIdentifier = data[i + 2];
			LeaderboardSets.Add(lbSet);
		}


		ISN_Result res =  new ISN_Result(true);
		OnLeaderboardSetsInfoLoaded(res);
	}

	private void ISN_OnLBSetsLoadFailed(string array) {
		ISN_Result res =  new ISN_Result(false);
		OnLeaderboardSetsInfoLoaded(res);
	}


	private void ISN_OnLBSetsBoardsLoadFailed(string identifier) {
		foreach(GK_LeaderboardSet lb in LeaderboardSets) {
			if(lb.Identifier.Equals(identifier)) {
				lb.SendFailLoadEvent();
				return;
			}
		}
	}


	private void ISN_OnLBSetsBoardsLoaded(string array) {


		string[] data = array.Split("|" [0]);

		string identifier = data[0];

		foreach(GK_LeaderboardSet lb in LeaderboardSets) {
			if(lb.Identifier.Equals(identifier)) {

				for(int i = 1; i < data.Length; i+=3) {
					GK_LeaderBoardInfo info =  new GK_LeaderBoardInfo();
					info.Title = data[i];
					info.Description = data[i + 1];
					info.Identifier = data[i + 2];
					lb.AddBoardInfo(info);
				}

				lb.SendSuccessLoadEvent();

				return;
			}
		}
		

	}

	private void OnLeaderboardMaxRangeUpdate(string array) {
		string[] data = array.Split("|" [0]);

		string identifier = data[0];
		int MaxRange = System.Convert.ToInt32(data[1]);

		GK_Leaderboard board;
		if(_leaderboards.ContainsKey(identifier)) {
			board = _leaderboards[identifier];
		} else {
			board =  new GK_Leaderboard(identifier);
		}

		board.UpdateMaxRange(MaxRange);
	}


	//--------------------------------------
	// UTILS
	//--------------------------------------

	public static List<GK_TBM_Participant>  ParseParticipantsData(string[] data, int index ) {
		
		List<GK_TBM_Participant> Participants =  new List<GK_TBM_Participant>();
		
		for(int i = index; i < data.Length; i += 5) {
			if(data[i] == IOSNative.DATA_EOF) {
				break;
			}
			
			GK_TBM_Participant p = ParseParticipanData(data, i);
			Participants.Add(p);
			
		}
		
		return Participants;
	}


	public static GK_TBM_Participant ParseParticipanData(string[] data, int index ) {
		GK_TBM_Participant participant =  new GK_TBM_Participant(data[index], data[index + 1], data[index + 2], data[index + 3], data[index + 4]);
		return participant;
	}


	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
