using UnityEngine;
using System.Collections;

public class FB_APIResult  {

	private bool _IsSucceeded = false;
	private string _Data = string.Empty;
	private string _Error = string.Empty;
	



	public FB_APIResult(FBResult result) {
		if(result.Error == null) {
			_IsSucceeded = true;
			_Data = result.Text;
		} else {
			_IsSucceeded = false;
			_Error = result.Error;
		}

	}

	




	public bool IsSucceeded {
		get {
			return _IsSucceeded;
		}
	} 

	[System.Obsolete("responce is deprecated, please use Responce instead.")]
	public string responce {
		get {
			return _Data;
		}
	}

	public string Responce {
		get {
			return _Data;
		}
	}

	public string Error {
		get {
			return _Error;
		}
	}
	
}
