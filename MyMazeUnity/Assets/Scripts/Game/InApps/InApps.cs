using UnityEngine;
using System.Collections;

public class InApps : MonoBehaviour, ISavingElement {

    public Deligates.SimpleEvent OnPremiumBuyed;

    public interface IStoreMatch
    {

    }

    [System.Serializable]
    public class AppStoreMatching : IStoreMatch
    {
        public string productId;
        public ProductTypes type;
    }
    public AppStoreMatching[] appStoreProducts;

    [HideInInspector]
    public bool IsPremium;
    
    /// <summary>
    /// Попытаемся купить премиум
    /// </summary>
    public void BuyPremiumRequest()
    {
#if UNITY_IPHONE
        if (MyMaze.Instance.AppStore.IsInitalized)
        {
            IOSInAppPurchaseManager.instance.buyProduct(GetProduct<AppStoreMatching>(ProductTypes.Premium).productId);
            MyMaze.Instance.AppStore.OnPremiumTransactionSuccess += OnPremiumTransactionSuccess;
        }
        else
            Debug.Log("AppStore не инициализирован");
#endif
    }

    /// <summary>
    /// Успешно проведена покупка
    /// </summary>
    void OnPremiumTransactionSuccess()
    {
#if UNITY_IPHONE
        MyMaze.Instance.AppStore.OnPremiumTransactionSuccess -= OnPremiumTransactionSuccess;
#endif
        UnlockPremium();
    }

    /// <summary>
    /// Разблокируем премиум
    /// </summary>
    public void UnlockPremium()
    {
        if (!IsPremium)
        {
            Debug.Log("Купили премиум");
            IsPremium = true;
            if (OnPremiumBuyed != null)
                OnPremiumBuyed();
        }
        else
            Debug.Log("Премиум уже приобретен");
    }

    /// <summary>
    /// Получить ссылку на описание продукта
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public T GetProduct<T>(ProductTypes type) where T : IStoreMatch
    {
        foreach (AppStoreMatching product in appStoreProducts)
            if (product.type == ProductTypes.Premium)
                return (T)(IStoreMatch) product;

        return (T)(IStoreMatch) null;
    }

    /// <summary>
    /// Сохранить локально
    /// </summary>
    public void Save()
    {
        PlayerPrefs.SetInt("IsPremium", System.Convert.ToInt32(IsPremium));
    }

    /// <summary>
    /// Загрузить
    /// </summary>
    public void Load()
    {
        if (PlayerPrefs.HasKey("IsPremium"))
            IsPremium = System.Convert.ToBoolean(PlayerPrefs.GetInt("IsPremium"));
    }

    /// <summary>
    /// Сбросить сохранения
    /// </summary>
    public void ResetSaves()
    {
        throw new System.NotImplementedException();
    }
}
