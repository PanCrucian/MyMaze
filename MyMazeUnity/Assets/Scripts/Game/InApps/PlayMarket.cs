using UnityEngine;
using System.Collections;

public class PlayMarket : MonoBehaviour {
#if UNITY_ANDROID
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

    void Start()
    {
        foreach (InApps.MarketMatching product in MyMaze.Instance.InApps.playMarketProducts)
            AndroidInAppPurchaseManager.Instance.AddProduct(product.productId);

        AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;
        AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
        AndroidInAppPurchaseManager.ActionProductPurchased += OnProductPurchased;
        AndroidInAppPurchaseManager.ActionProductConsumed += OnProductConsumed;

        AndroidInAppPurchaseManager.Instance.LoadStore();
    }

    /// <summary>
    /// Соеденились с Play Market
    /// </summary>
    /// <param name="result"></param>
    void OnBillingConnected(BillingResult result)
    {
        if (result.isSuccess)
        {
            Debug.Log("Connection Success: " + result.response.ToString() + " " + result.message);
            AndroidInAppPurchaseManager.Instance.RetrieveProducDetails();
        }else
            Debug.LogWarning("Connection Error: " + result.response.ToString() + " " + result.message);
    }

    /// <summary>
    /// Получили информацию о продуктах из магазина
    /// </summary>
    /// <param name="result"></param>
    void OnRetrieveProductsFinised(BillingResult result)
    {
        AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;
        if (result.isSuccess)
        {
            _isInitalized = true;
            Debug.Log("Retrieve Products Success: " + result.response.ToString() + " " + result.message);
            CheckPurchasedProductsAndConsume();
        }
        else
            Debug.LogWarning("Retrieve Products Error: " + result.response.ToString() + " " + result.message);
    }

    /// <summary>
    /// Проверям были ли приобретенные продукты и потребляем их если забыли или баг какой
    /// </summary>
    public void CheckPurchasedProductsAndConsume()
    {
        if (!IsInitalized)
            return;
        foreach (GoogleProductTemplate p in AndroidInAppPurchaseManager.Instance.inventory.products)
            if (AndroidInAppPurchaseManager.Instance.inventory.IsProductPurchased(p.SKU))
                Consume(p.SKU);
        if (OnRestoreCompleteSuccess != null)
            OnRestoreCompleteSuccess();
    }

    /// <summary>
    /// Покупаем продукт
    /// </summary>
    /// <param name="SKU"></param>
    public void Purchase(string SKU)
    {
        Debug.Log("Try for Purchase: " + SKU);
        AndroidInAppPurchaseManager.Instance.Purchase(SKU);
    }

    /// <summary>
    /// Потребляем продукт
    /// </summary>
    /// <param name="SKU"></param>
    public void Consume(string SKU)
    {
        Debug.Log("Try for consume: " + SKU);
        AndroidInAppPurchaseManager.Instance.Consume(SKU);
    }

    /// <summary>
    /// Продукт был проебретен
    /// this flag will tell you if purchase is available result.isSuccess
    /// infomation about purchase stored here result.purchase
    /// here is how for example you can get product SKU result.purchase.SKU
    /// </summary>
    /// <param name="result"></param>
    void OnProductPurchased(BillingResult result)
    {
        if (result.isSuccess)
        {
            Debug.Log("Purchased Success: " + result.response.ToString() + ", message: " + result.message);
            Consume(result.purchase.SKU);
        }
        else
        {
            Debug.LogWarning("Product Purchase Failed! Response: " + result.response.ToString() + ", message: " + result.message);
            if (result.response == 7)
                Consume(result.purchase.SKU);
        }
    }

    /// <summary>
    /// Продукт был потреблен
    /// </summary>
    /// <param name="result"></param>
    void OnProductConsumed(BillingResult result)
    {

        if (result.isSuccess)
        {
            Debug.Log("Cousume Success: " + result.response.ToString() + ", message: " + result.message);
            if (OnTransactionSuccess != null)
                OnTransactionSuccess(MyMaze.Instance.InApps.GetProduct<InApps.MarketMatching>(result.purchase.SKU).type, result.purchase.originalJson, result.purchase.orderId);
        }
        else
        {
            Debug.LogWarning("Product Purchase Failed! Response: " + result.response.ToString() + ", message: " + result.message);
        }
    }
#endif
}
