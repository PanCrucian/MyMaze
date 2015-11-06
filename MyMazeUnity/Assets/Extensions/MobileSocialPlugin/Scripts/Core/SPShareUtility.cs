using UnityEngine;
using System.Collections;

public class SPShareUtility  {

	public static void TwitterShare(string status) {
		TwitterShare(status, null);
	}
	
	public static void TwitterShare(string status, Texture2D texture) {
		switch(Application.platform) {
		case RuntimePlatform.Android:
			MSPAndroidSocialGate.StartShareIntent("Share", status, texture, "twi");
			break;
		case RuntimePlatform.IPhonePlayer:
			MSPIOSSocialManager.instance.TwitterPost(status, null, texture);
			break;
		}
	}


	public static void FacebookShare(string message) {
		FacebookShare(message, null);
	}
	
	public static void FacebookShare(string message, Texture2D texture) {
		switch(Application.platform) {
		case RuntimePlatform.Android:
			MSPAndroidSocialGate.StartShareIntent("Share", message, texture, "facebook.katana");
			break;
		case RuntimePlatform.IPhonePlayer:
			MSPIOSSocialManager.instance.FacebookPost(message, null, texture);
			break;
		}
	}


	public static void ShareMedia(string caption, string message) {
		ShareMedia(caption, message, null);
	}
	
	public static void ShareMedia(string caption, string message, Texture2D texture) {
		switch(Application.platform) {
		case RuntimePlatform.Android:
			MSPAndroidSocialGate.StartShareIntent(caption, message, texture);
			break;
		case RuntimePlatform.IPhonePlayer:
			MSPIOSSocialManager.instance.ShareMedia(message, texture);
			break;
		}
	}

	
	public static void SendMail(string subject, string body, string recipients) {
		SendMail(subject, body, recipients, null);
	}
	
	public static void SendMail(string subject, string body, string recipients, Texture2D texture) {

		switch(Application.platform) {
		case RuntimePlatform.Android:
			MSPAndroidSocialGate.SendMail("Send Mail", body, subject, recipients, texture);
			break;
		case RuntimePlatform.IPhonePlayer:
			MSPIOSSocialManager.instance.SendMail(subject, body, recipients, texture);
			break;
		}

	}



}
