using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ANMiniJSON;

public class FBPostResult  {

	//FB result from Unity Facebok SDK
	private FBResult _Result = null;

	//The request object ID. 
	private string _PostId = string.Empty;

	//Flag to indicate of request was sent successfully
	private bool _IsSucceeded = false;


	public FBPostResult(FBResult r) {

		_Result = r;

		if(_Result.Error == null) {
			try {
				Debug.Log("Posy Result");
				Dictionary<string, object> data =   ANMiniJSON.Json.Deserialize(_Result.Text) as Dictionary<string, object>;
				_PostId = System.Convert.ToString(data["id"]);
				_IsSucceeded = true;
			} catch(System.Exception ex) {
				_IsSucceeded = false;
				Debug.Log("No Post Id: "  + ex.Message);
			}
				
			SA_StatusBar.text = "Posting complete";
		} else {
			_IsSucceeded = false;
		}
	}


	public FBResult Result {
		get {
			return _Result;
		}
	}

	public string PostId {
		get {
			return _PostId;
		}
	}

	public bool IsSucceeded {
		get {
			return _IsSucceeded;
		}
	}
}
