﻿using UnityEngine;
using System;
using System.Collections;

public class AndroidCamera : SA_Singleton<AndroidCamera>  {

	//Actions
	public Action<AndroidImagePickResult> OnImagePicked = delegate{};
	public Action<GallerySaveResult> OnImageSaved = delegate{};


	private static string _lastImageName = string.Empty;

	void Awake() {
		DontDestroyOnLoad(gameObject);

		AndroidNative.InitCameraAPI(AndroidNativeSettings.Instance.GalleryFolderName,
		                            AndroidNativeSettings.Instance.MaxImageLoadSize,
		                            (int) AndroidNativeSettings.Instance.CameraCaptureMode,
		                            (int) AndroidNativeSettings.Instance.ImageFormat);
	}


	[Obsolete("SaveImageToGalalry is deprecated, please use SaveImageToGallery instead.")]
	public void SaveImageToGalalry(Texture2D image, String name = "Screenshot") {
		SaveImageToGallery(image, name);
	}

	public void SaveImageToGallery(Texture2D image, String name = "Screenshot") {
		if(image != null) {
			byte[] val = image.EncodeToPNG();
			string mdeia = System.Convert.ToBase64String (val);
			AndroidNative.SaveToGalalry(mdeia, name);
		}  else {
			Debug.LogWarning("AndroidCamera::SaveToGalalry:  image is null");
		}
	}




	public void SaveScreenshotToGallery(String name = "") {
		_lastImageName = name;
		SA_ScreenShotMaker.instance.OnScreenshotReady += OnScreenshotReady;
		SA_ScreenShotMaker.instance.GetScreenshot();
	}


	public void GetImageFromGallery() {
		AndroidNative.GetImageFromGallery();
	}
	
	
	
	public void GetImageFromCamera() {
		AndroidNative.GetImageFromCamera(AndroidNativeSettings.Instance.SaveCameraImageToGallery);
	}




	private void OnImagePickedEvent(string data) {

		Debug.Log("OnImagePickedEvent");
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		string codeString = storeData[0];
		string ImagePathInfo = storeData[1];
		string ImageData = storeData[2];

		AndroidImagePickResult result =  new AndroidImagePickResult(codeString, ImageData, ImagePathInfo);

		OnImagePicked(result);

	}

	private void OnImageSavedEvent(string data) {
		GallerySaveResult res =  new GallerySaveResult(data, true);
		OnImageSaved(res);
	}

	private void OnImageSaveFailedEvent(string data) {
		GallerySaveResult res =  new GallerySaveResult("", false);
		OnImageSaved(res);
	}



	private void OnScreenshotReady(Texture2D tex) {
		SA_ScreenShotMaker.instance.OnScreenshotReady -= OnScreenshotReady;
		SaveImageToGallery(tex, _lastImageName);

	}
}
