using UnityEngine;
using System.Collections;

public class FB_APIResult  {

	private bool _IsSucceeded = false;
	private string _data = string.Empty;

	public FBResult Unity_FB_Result;



	public FB_APIResult(bool IsResSucceeded, string resData) {
		_IsSucceeded = IsResSucceeded;
		_data = resData;	
	}
	
	public bool IsSucceeded {
		get {
			return _IsSucceeded;
		}
	} 
	
	public string responce {
		get {
			return _data;
		}
	}
}
