using UnityEngine;
using System;
using System.Collections;

public class FBAppRequest {

	//Unique request id
	public string Id;

	//Your app's unique identifier.
	public string ApplicationId;
	
	//A plain-text message to be sent as part of the request. This text will surface in the App Center view of the request, but not on the notification jewel
	public string Message = string.Empty;

	//Request Type. Used when defining additional context about the nature of the request. Possible values are Send, AskFor, Turn and Undefined
	public FBAppRequestActionType ActionType = FBAppRequestActionType.Undefined;

	//Request State. Used to defining of request was already removed. Possible values are Pending and Deleted
	public FBAppRequestState State = FBAppRequestState.Pending;

	//Sender Facebook ID
	public string FromId;

	//Sender Name
	public string FromName;
	
	//Request Created Time
	public DateTime CreatedTime;
	public string CreatedTimeString;


	//Additional freeform data you may pass for tracking. This will be stored as part of the request objects created. The maximum length is 255 characters. (Optional)
	public string Data = string.Empty;

	//The Open Graph object being sent. Only if ActionType has been set to Send or AskFor 
	public FBObject Object;

	public Action<FBResult> OnDeleteRequestFinished = delegate {};



	public void SetCreatedTime(string time_string) {
		CreatedTimeString = time_string;
		CreatedTime =  DateTime.Parse(time_string);
	}




	public void Delete() {
		FB.API(Id, Facebook.HttpMethod.DELETE, OnDeleteActionFinish);  
		State = FBAppRequestState.Deleted;
	}


	private void OnDeleteActionFinish(FBResult result) {
	
		if(result.Error != null) {
			State = FBAppRequestState.Deleted;
		}


		OnDeleteRequestFinished(result);
	}

}
