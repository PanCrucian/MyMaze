using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FB_GraphAPIRequest : MonoBehaviour   {

	public event Action<FB_APIResult> ActionComplete = delegate{};


	private bool IsFirst = true;
	private string GetParams = string.Empty;

	private string requestUrl = "https://graph.facebook.com";

	#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 
	private Hashtable Headers = new Hashtable();
	#else
	private  Dictionary<string, string> Headers = new Dictionary<string, string>();
	#endif

	private WWWForm form = new WWWForm();


	public static FB_GraphAPIRequest Create() {
		return new GameObject("FB_APIRequest§").AddComponent<FB_GraphAPIRequest>();
	}

	public void Send() {
		if(SPFacebook.instance.IsLoggedIn) {
			StartCoroutine(Request());
		}  else {
			ActionComplete(new FB_APIResult(false, "User not logged in"));
			Destroy(gameObject);
		}
	}



	public void AppendUrl(string url) {
		requestUrl += url;
	}


	public void AddParam(string name, int value) {
		AddParam(name, value.ToString());
	}
	
	public void AddParam(string name, string value) {

		form.AddField(name, value);
		
		if(!IsFirst) {
			GetParams += "&";
		} else {
			GetParams += "?";
		}
		
		GetParams += name + "=" + value;
		
		
		IsFirst = false;
	}





	private IEnumerator Request () {
		
		
		//requestUrl = requestUrl + GetParams;
		
		
		Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
		//Headers.Add("POST", GetParams.ToString());

		
		WWW www = new WWW(requestUrl, form.data,  Headers);
		yield return www;

		foreach(KeyValuePair<string, string> kv in www.responseHeaders) {
			Debug.Log(kv.Key + " : " + kv.Value);
		}

		
		if(www.error == null) {
			ActionComplete( new FB_APIResult(true, www.text));
			Destroy(gameObject);
		} else {
			ActionComplete( new FB_APIResult(false, www.error));
			Destroy(gameObject);
		}


		
	}



}
