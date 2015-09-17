using UnityEngine;
using System.Collections;

public class AppStore : MonoBehaviour {
    #region Apple AppStore methods and variables
#if UNITY_IPHONE
    public Deligates.RecieptTransactionEvent OnTransactionSuccess;
    public Deligates.SimpleEvent OnRestoreCompleteSuccess;

    /// <summary>
    /// был ли магазин инициализирован
    /// </summary>
    public bool IsInitalized
    {
        get
        {
            return _isInitalized;
        }
    }
    private bool _isInitalized;

    /// <summary>
    /// Инициализируем магазин
    /// </summary>
    void Start()
    {
        foreach (InApps.MarketMatching product in MyMaze.Instance.InApps.appStoreProducts)
            IOSInAppPurchaseManager.Instance.addProductId(product.productId);

        IOSInAppPurchaseManager.OnStoreKitInitComplete += OnStoreKitInitComplete;
        IOSInAppPurchaseManager.OnTransactionComplete += OnTransactionComplete;
        IOSInAppPurchaseManager.OnRestoreComplete += OnRestoreComplete;

        IOSInAppPurchaseManager.Instance.loadStore();
    }

    /// <summary>
    /// Методы прошли инициализацию
    /// </summary>
    /// <param name="result"></param>
    private void OnStoreKitInitComplete(ISN_Result result)
    {
        _isInitalized = result.IsSucceeded;
        if (_isInitalized)
            Debug.Log("StoreKit Init Succeeded" + "\n" + "Available products count: " + IOSInAppPurchaseManager.instance.products.Count.ToString());
        else
            Debug.LogWarning("StoreKit Init Failed" + "\n" + "Error code: " + result.Error.Code + "\n" + "Error description:" + result.Error.Description);
    }

    /// <summary>
    /// Ответ от магазина о состоянии последней транзации
    /// </summary>
    /// <param name="response"></param>
    private void OnTransactionComplete(IOSStoreKitResult response)
    {
        switch (response.State)
        {
            case InAppPurchaseState.Purchased:
            case InAppPurchaseState.Restored:
                //Успешно или купили или восстановили покупку                    
                InApps.MarketMatching product = MyMaze.Instance.InApps.GetProduct<InApps.MarketMatching>(response.ProductIdentifier);
                if(product != null)
                    if (OnTransactionSuccess != null)
                        OnTransactionSuccess(product.type, response.Receipt);
                break;
            case InAppPurchaseState.Deferred:
                //iOS 8 introduces Ask to Buy, which lets parents approve any purchases initiated by children
                //You should update your UI to reflect this deferred state, and expect another Transaction Complete  to be called again with a new transaction state 
                //reflecting the parent’s decision or after the transaction times out. Avoid blocking your UI or gameplay while waiting for the transaction to be updated.
                break;
            case InAppPurchaseState.Failed:
                //Покупка не прошла
                Debug.LogWarning(
                    "Transaction failed" + "\n" + 
                    "error code: " + response.Error.Code + "\n" +
                    "description: " + response.Error.Description + "\n" +
                    "product " + response.ProductIdentifier + "\n" + 
                    "state: " + response.State.ToString());
                break;
        }
    }
    
    /// <summary>
    /// Ответ от магазина после запроса о восстановлении покупок
    /// </summary>
    /// <param name="res"></param>
    private void OnRestoreComplete(IOSStoreKitRestoreResult res)
    {
        if (res.IsSucceeded)
        {
            Debug.Log("Success Restore Compleated");
            if (OnRestoreCompleteSuccess != null)
                OnRestoreCompleteSuccess();
        }
        else
            Debug.LogWarning("Restore Failed Error: \n" +
                "code: " + res.Error.Code + "\n" +
                "description: " + res.Error.Description);
    }	
#endif
    #endregion
}
