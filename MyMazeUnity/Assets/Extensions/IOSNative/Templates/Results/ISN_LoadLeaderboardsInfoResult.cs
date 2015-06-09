using UnityEngine;
using System.Collections;

public class ISN_LoadSetLeaderboardsInfoResult : ISN_Result {

	public GCLeaderboardSet _LeaderBoardsSet;

	public ISN_LoadSetLeaderboardsInfoResult(GCLeaderboardSet lbset, bool IsResultSucceeded):base(IsResultSucceeded) {
		_LeaderBoardsSet = lbset;
	}

	public GCLeaderboardSet LeaderBoardsSet {
		get {
			return _LeaderBoardsSet;
		}
	}
}
