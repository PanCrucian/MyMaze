using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FBAppRequestResult  {

	//FB result from Unity Facebok SDK
	public FBResult Result = null;

	//The request object ID. 
	public string ReuqestId = string.Empty;

	//An array of the recipient user IDs for the request that was created.
	public List<string> Recipients =  new List<string>();


	//Flag to indicate of request was sent successfully
	public bool IsSucceeded = false;
	
}
