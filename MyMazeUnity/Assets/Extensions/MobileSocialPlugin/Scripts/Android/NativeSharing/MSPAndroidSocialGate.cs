using UnityEngine;
using System.Collections;

public class MSPAndroidSocialGate  {

	public static void StartGooglePlusShare(string text, Texture2D texture = null) {
		AN_SocialSharingProxy.StartGooglePlusShareIntent(text, texture == null ? string.Empty : System.Convert.ToBase64String(texture.EncodeToPNG()));
	}

	public static void StartShareIntent(string caption, string message, string packageNamePattern = "") {
		StartShareIntentWithSubject(caption, message, "", packageNamePattern);
	}
	
	public static void StartShareIntentWithSubject(string caption, string message, string subject, string packageNamePattern = "") {
		AN_SocialSharingProxy.StartShareIntent(caption, message, subject, packageNamePattern);
	}	
	
	public static void StartShareIntent(string caption, string message, Texture2D texture,  string packageNamePattern = "") {
		StartShareIntentWithSubject(caption, message, "", texture, packageNamePattern);
	}
	
	public static void StartShareIntentWithSubject(string caption, string message, string subject, Texture2D texture,  string packageNamePattern = "") {
		if(texture != null) {
			byte[] val = texture.EncodeToPNG();
			string bytesString = System.Convert.ToBase64String (val);			
			AN_SocialSharingProxy.StartShareIntent(caption, message, subject, bytesString, packageNamePattern, SocialPlatfromSettings.Instance.SaveImageToGallery);
		} else {
			AN_SocialSharingProxy.StartShareIntent(caption, message, subject, packageNamePattern);
		}
	}

	public static void SendMail(string caption, string message,  string subject, string recipients, Texture2D texture = null) {
		if(texture != null) {
			byte[] val = texture.EncodeToPNG();
			string mdeia = System.Convert.ToBase64String (val);
			AN_SocialSharingProxy.SendMailWithImage(caption, message, subject, recipients, mdeia, SocialPlatfromSettings.Instance.SaveImageToGallery);
		} else {
			AN_SocialSharingProxy.SendMail(caption, message, subject, recipients);
		}		
	}
}

