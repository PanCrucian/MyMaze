using UnityEngine;
using System;
using System.Collections;

public class FBPostingTask : AsyncTask {

	private string _toId = "";
	private string _link = "";
	private string _linkName = "";
	private string _linkCaption = "";
	private string _linkDescription = "";
	private string _picture = "";
	private string _actionName = "";
	private string _actionLink = "";
	private string _reference = "";

	public event Action<FBPostResult> ActionComplete = delegate{};

	public static FBPostingTask Cretae() {
		return	new GameObject("PostingTask").AddComponent<FBPostingTask>();
	}


	public void Post(
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

		_toId = toId;
		_link = link;
		_linkName = linkName;
		_linkCaption = linkCaption;
		_linkDescription = linkDescription;
		_picture = picture;
		_actionName = actionName;
		_actionLink = actionLink;
		_reference = reference;


		if(SPFacebook.instance.IsInited) {
			OnFBInited();
		} else {
			SPFacebook.Instance.OnInitCompleteAction += OnFBInited;
			SPFacebook.instance.Init();
		}


	}


	private void OnFBInited() {
		SPFacebook.Instance.OnInitCompleteAction -= OnFBInited;
		if(SPFacebook.instance.IsLoggedIn) {
			OnFBAuth(null);
		} else {
			SPFacebook.Instance.OnAuthCompleteAction += OnFBAuth;
			SPFacebook.instance.Login();
		}
	}


	private void OnFBAuth(FBResult result) {

		SPFacebook.Instance.OnAuthCompleteAction -= OnFBAuth;


		if(SPFacebook.Instance.IsLoggedIn) {

			SPFacebook.Instance.OnPostingCompleteAction += HandleOnPostingCompleteAction;
			SPFacebook.instance.Post(_toId,
			                         _link,
			                         _linkName,
			                         _linkCaption,
			                         _linkDescription,
			                         _picture,
			                         _actionName,
			                         _actionLink,
			                         _reference);

		} else {
			FBResult res =  new FBResult("", "Auth failed");
			FBPostResult postResult =  new FBPostResult(res);

			ActionComplete(postResult);
		}



	}

	void HandleOnPostingCompleteAction (FBPostResult res) {
		SPFacebook.Instance.OnPostingCompleteAction -= HandleOnPostingCompleteAction;
		ActionComplete(res);
	}
	
}
