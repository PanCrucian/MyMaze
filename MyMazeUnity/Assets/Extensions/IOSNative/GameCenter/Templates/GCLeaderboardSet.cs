using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GCLeaderboardSet  {

	public string Title;
	public string Identifier;
	public string GroupIdentifier;

	public List<GCLeaderBoardInfo> _BoardsInfo =  new List<GCLeaderBoardInfo>();

	public event Action<ISN_LoadSetLeaderboardsInfoResult> OnLoaderboardsInfoLoaded = delegate {};


	public void LoadLeaderBoardsInfo() {
		GameCenterManager.LoadLeaderboardsForSet(Identifier);
	}


	public void AddBoardInfo(GCLeaderBoardInfo info) {
		_BoardsInfo.Add(info);
	}

	public void SendFailLoadEvent() {
		ISN_LoadSetLeaderboardsInfoResult res =  new ISN_LoadSetLeaderboardsInfoResult(this, false);
		OnLoaderboardsInfoLoaded(res);
	}

	public void SendSuccessLoadEvent() {
		ISN_LoadSetLeaderboardsInfoResult res =  new ISN_LoadSetLeaderboardsInfoResult(this, true);
		OnLoaderboardsInfoLoaded(res);
	}





	
	public List<GCLeaderBoardInfo> BoardsInfo {
		get {
			return _BoardsInfo;
		}
	}
}
