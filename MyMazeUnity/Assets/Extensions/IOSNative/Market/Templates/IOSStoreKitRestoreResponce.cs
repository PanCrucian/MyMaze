using UnityEngine;
using System.Collections;

public class IOSStoreKitRestoreResponce : ISN_Result {

	private IOSStoreKitError _Error;

	public IOSStoreKitRestoreResponce(bool IsResultSucceeded, IOSStoreKitError error):base(IsResultSucceeded) {
		_Error = error;
	}

	public IOSStoreKitError Error {
		get {
			return _Error;
		}
	}
}
