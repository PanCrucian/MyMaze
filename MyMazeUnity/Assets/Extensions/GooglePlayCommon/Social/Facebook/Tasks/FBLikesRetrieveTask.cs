using UnityEngine;
using System;
using System.Collections;

public class FBLikesRetrieveTask : MonoBehaviour {

	private string _userId;

	public event Action<FBResult, FBLikesRetrieveTask> ActionComplete = delegate{};


	public static FBLikesRetrieveTask Create(){
		return new GameObject("FBLikesRetrieveTask").AddComponent<FBLikesRetrieveTask>();
	}

	public void LoadLikes(string userId) {
		_userId =  userId;
		FB.API("/" + userId + "/likes", Facebook.HttpMethod.GET, OnUserLikesResult);  
	}
	
	public void LoadLikes(string userId, string pageId ) {
		_userId =  userId;
		FB.API("/" + userId + "/likes/" + pageId, Facebook.HttpMethod.GET, OnUserLikesResult);  
	}



	public string userId {
		get {
			return _userId;
		}
	}
	

	private void OnUserLikesResult(FBResult result) {
		ActionComplete(result, this);
		Destroy(gameObject);
	}
}
