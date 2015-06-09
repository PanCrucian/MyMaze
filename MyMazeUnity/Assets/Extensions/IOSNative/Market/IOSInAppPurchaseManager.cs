////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System;
using UnionAssets.FLE;
using System.Collections;
using System.Collections.Generic;


public class IOSInAppPurchaseManager : ISN_Singleton<IOSInAppPurchaseManager> {


	public const string APPLE_VERIFICATION_SERVER   = "https://buy.itunes.apple.com/verifyReceipt";
	public const string SANDBOX_VERIFICATION_SERVER = "https://sandbox.itunes.apple.com/verifyReceipt";


	//Events
	public const string TRANSACTION_COMPLETE 	= "transaction_complete";
	public const string RESTORE_TRANSACTION_FAILED 	= "restore_transaction_failed";
	public const string RESTORE_TRANSACTION_COMPLETE 	= "restore_transaction_complete";
	

	public const string VERIFICATION_RESPONSE 	= "verification_response";
	public const string STORE_KIT_INITIALIZED	= "store_kit_initialized";
	public const string STORE_KIT_INITI_FAILED	= "store_kit_init_failed";


	//Actions
	public Action<IOSStoreKitResponse> OnTransactionComplete = delegate{};
	public Action<IOSStoreKitRestoreResponce> OnRestoreComplete = delegate{};
	public Action<ISN_Result> OnStoreKitInitComplete = delegate{};
	public Action<bool> OnPurchasesStateSettingsLoaded = delegate{};
	public Action<IOSStoreKitVerificationResponse> OnVerificationComplete = delegate{};

	
	private bool _IsStoreLoaded = false;
	private bool _IsWaitingLoadResult = false;
	private bool _IsInAppPurchasesEnabled = true;
	private static int _nextId = 1;
	
	private List<string> _productsIds =  new List<string>();
	private List<IOSProductTemplate> _products    =  new List<IOSProductTemplate>();
	private Dictionary<int, IOSStoreProductView> _productsView =  new Dictionary<int, IOSStoreProductView>(); 
	
	
	private static IOSInAppPurchaseManager _instance;
	private static string lastPurchasedProduct;
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	public void loadStore() {

		if(_IsStoreLoaded) {
			Invoke("FireSuccessInitEvent", 1f);
			return;
		}

		if(_IsWaitingLoadResult) {
			return;
		}

		_IsWaitingLoadResult = true;
		
		foreach(string pid in IOSNativeSettings.Instance.InAppProducts) {
			addProductId(pid);
		}
		
		string ids = "";
		int len = _productsIds.Count;
		for(int i = 0; i < len; i++) {
			if(i != 0) {
				ids += ",";
			}
			
			ids += _productsIds[i];
		}


		#if !UNITY_EDITOR
		IOSNativeMarketBridge.loadStore(ids);
		#else
		if(IOSNativeSettings.Instance.SendFakeEventsInEditor) {
			Invoke("FireSuccessInitEvent", 1f);
		}
		#endif
		
	}

	public void RequestInAppSettingState() {
		IOSNativeMarketBridge.ISN_RequestInAppSettingState();
	}


	public void buyProduct(string productId) {

		#if !UNITY_EDITOR

		if(!_IsStoreLoaded) {

			if(!IOSNativeSettings.Instance.DisablePluginLogs) 
				Debug.LogWarning("buyProduct shouldn't be called before StoreKit is initialized"); 
			SendTransactionFailEvent(productId, "StoreKit not yet initialized", IOSTransactionErrorCode.SKErrorPaymentNotAllowed);

			return;
		} 

		IOSNativeMarketBridge.buyProduct(productId);

		#else
		if(IOSNativeSettings.Instance.SendFakeEventsInEditor) {
			FireProductBoughtEvent(productId, "", "", false);
		}
		#endif
	}
	
	public void addProductId(string productId) {
		if(_productsIds.Contains(productId)) {
			return;
		}

		_productsIds.Add(productId);
	}

	public IOSProductTemplate GetProductById(string id) {
		foreach(IOSProductTemplate tpl in products) {
			if(tpl.id.Equals(id)) {
				return tpl;
			}
		}

		return null;
	}

	
	public void restorePurchases() {

		if(!_IsStoreLoaded) {

			IOSStoreKitError e = new IOSStoreKitError();
			e.code = IOSTransactionErrorCode.SKErrorPaymentServiceNotInitialized;
			e.description = "Store Kit Initilizations required";

			IOSStoreKitRestoreResponce r =  new IOSStoreKitRestoreResponce(false, e);
			OnRestoreComplete(r);
			dispatch(RESTORE_TRANSACTION_FAILED, r);
			return;
		}

		#if !UNITY_EDITOR
		IOSNativeMarketBridge.restorePurchases();
		#else
		if(IOSNativeSettings.Instance.SendFakeEventsInEditor) {
			foreach(string productId in _productsIds) {
				Debug.Log("Restored: " + productId);
				FireProductBoughtEvent(productId, "", "", true);
			}
			FireRestoreCompleteEvent();
		}
		#endif
	}

	public void verifyLastPurchase(string url) {
		IOSNativeMarketBridge.verifyLastPurchase (url);
	}

	public void RegisterProductView(IOSStoreProductView view) {
		view.SetId(nextId);
		_productsView.Add(view.id, view);
	}
	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public List<IOSProductTemplate> products {
		get {
			return _products;
		}
	}

	public bool IsStoreLoaded {
		get {
			return _IsStoreLoaded;
		}
	}

	public bool IsInAppPurchasesEnabled {
		get {
			return _IsInAppPurchasesEnabled;
		}
	}

	public bool IsWaitingLoadResult {
		get {
			return _IsWaitingLoadResult;
		}
	}

	public static int nextId {
		get {
			_nextId++;
			return _nextId;
		}
	}

	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void onStoreKitStart(string data) {
		int satus = System.Convert.ToInt32(data);
		if(satus == 1) {
			_IsInAppPurchasesEnabled = true;
		} else {
			_IsInAppPurchasesEnabled = false;
		}

		OnPurchasesStateSettingsLoaded(_IsInAppPurchasesEnabled);
	}

	private void OnStoreKitInitFailed(string data) {


		string[] storeData;
		storeData = data.Split("|" [0]);
		ISN_Error e =  new ISN_Error();
		e.code = System.Convert.ToInt32(storeData[0]);
		e.description = storeData[1];


		_IsStoreLoaded = false;
		_IsWaitingLoadResult = false;


		ISN_Result res = new ISN_Result (false);
		res.error = e;
		dispatch(STORE_KIT_INITI_FAILED, res);
		OnStoreKitInitComplete (res);


		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			Debug.Log("STORE_KIT_INIT_FAILED Error: " + e.description);
	}
	
	private void onStoreDataReceived(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("InAppPurchaseManager, no products avaiable: " + _products.Count.ToString());
			ISN_Result res = new ISN_Result(true);
			dispatch (STORE_KIT_INITIALIZED, res);
			OnStoreKitInitComplete(res);
			return;
		}


		string[] storeData;
		storeData = data.Split("|" [0]);
		
		for(int i = 0; i < storeData.Length; i+=7) {
			IOSProductTemplate tpl =  new IOSProductTemplate();
			tpl.id 				= storeData[i];
			tpl.title 			= storeData[i + 1];
			tpl.description 	= storeData[i + 2];
			tpl.localizedPrice 	= storeData[i + 3];
			tpl.price 			= storeData[i + 4];
			tpl.currencyCode 	= storeData[i + 5];
			tpl.currencySymbol 	= storeData[i + 6];
			_products.Add(tpl);
		}
		
		Debug.Log("InAppPurchaseManager, total products loaded: " + _products.Count.ToString());
		FireSuccessInitEvent();
	}
	
	private void onProductBought(string array) {

		string[] data;
		data = array.Split("|" [0]);

		bool IsRestored = false;
		if(data [1].Equals("0")) {
			IsRestored = true;
		}

		FireProductBoughtEvent(data [0], data [2], data [3], IsRestored);


	}

	private void onProductStateDeferred(string productIdentifier) {
		IOSStoreKitResponse response = new IOSStoreKitResponse ();
		response.productIdentifier = productIdentifier;
		response.state = InAppPurchaseState.Deferred;
		
		dispatch(TRANSACTION_COMPLETE, response);
		OnTransactionComplete (response);
	}

	
	private void onTransactionFailed(string array) {

		string[] data;
		data = array.Split("|" [0]);

		SendTransactionFailEvent(data [0], data [1], (IOSTransactionErrorCode) System.Convert.ToInt32( data [2]));
	}
	
	
	private void onVerificationResult(string array) {

		string[] data;
		data = array.Split("|" [0]);

		IOSStoreKitVerificationResponse response = new IOSStoreKitVerificationResponse ();
		response.status = System.Convert.ToInt32(data[0]);
		response.originalJSON = data [1];
		response.receipt = data [2];
		response.productIdentifier = lastPurchasedProduct;

		dispatch (VERIFICATION_RESPONSE, response);
		OnVerificationComplete (response);

	}

	public void onRestoreTransactionFailed(string array) {

		string[] data;
		data = array.Split("|" [0]);


		IOSStoreKitError e = new IOSStoreKitError();
		e.code = (IOSTransactionErrorCode) Convert.ToInt32(data[0]);
		e.description = data[1];
		
		IOSStoreKitRestoreResponce r =  new IOSStoreKitRestoreResponce(false, e);


		dispatch(RESTORE_TRANSACTION_FAILED, r);
		OnRestoreComplete (r);
	}

	public void onRestoreTransactionComplete(string array) {
		FireRestoreCompleteEvent();
	}



	private void OnProductViewLoaded(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnContentLoaded();
		}
	}

	private void OnProductViewLoadedFailed(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnContentLoadFailed();
		}
	}

	private void OnProductViewDismissed(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnProductViewDismissed();
		}
	}

	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------

	private void FireSuccessInitEvent() {
		_IsStoreLoaded = true;
		_IsWaitingLoadResult = false;
		ISN_Result r = new ISN_Result(true);
		dispatch (STORE_KIT_INITIALIZED);
		OnStoreKitInitComplete(r);
	}


	private void FireRestoreCompleteEvent() {

		IOSStoreKitRestoreResponce r =  new IOSStoreKitRestoreResponce(true, null);

		dispatch(RESTORE_TRANSACTION_COMPLETE, r);
		OnRestoreComplete (r);
	}

	private void FireProductBoughtEvent(string productIdentifier, string receipt, string transactionIdentifier, bool IsRestored) {
		IOSStoreKitResponse response = new IOSStoreKitResponse ();
		response.productIdentifier = productIdentifier;
		response.receipt = receipt;
		response.transactionIdentifier = transactionIdentifier;
		if(IsRestored) {
			response.state = InAppPurchaseState.Restored;
		} else {
			response.state = InAppPurchaseState.Purchased;
		}
		
		lastPurchasedProduct = response.productIdentifier;
		dispatch(TRANSACTION_COMPLETE, response);
		OnTransactionComplete (response);
	}


	private void SendTransactionFailEvent(string productIdentifier, string errorDescribtion, IOSTransactionErrorCode errorCode) {
		IOSStoreKitResponse response = new IOSStoreKitResponse ();
		response.productIdentifier = productIdentifier;
		response.state = InAppPurchaseState.Failed;


		response.error =  new IOSStoreKitError();
		response.error.description = errorDescribtion;
		response.error.code = errorCode;


		
		
		dispatch(TRANSACTION_COMPLETE, response);
		OnTransactionComplete (response);
	}
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
