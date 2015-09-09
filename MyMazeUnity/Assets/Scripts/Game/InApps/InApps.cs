using UnityEngine;
using System.Collections;

public class InApps : MonoBehaviour, ISavingElement {

    public Deligates.SimpleEvent OnNoAdsBuyed;
    public Deligates.SimpleEvent OnTimeMachineBuyed;
    public Deligates.SimpleEvent OnUnlimitedLivesBuyed;
    public Deligates.SimpleEvent OnFiveLivesBuyed;
    public Deligates.SimpleEvent OnTeleportBuyed;

    public Deligates.TransactionEvent OnBuyed;
    public Deligates.TransactionEvent OnBuyRequest;

    public interface IStoreMatch
    {

    }

    [System.Serializable]
    public class MarketMatching : IStoreMatch
    {
        public string productId;
        public ProductTypes type;
        public int cost = 0;
    }
    /// <summary>
    /// Сопаставления продуктов с идетификаторами магазина Apple
    /// </summary>
    public MarketMatching[] appStoreProducts;

    /// <summary>
    /// Сопаставления продуктов с индентификаторами магазина Google Play
    /// </summary>
    public MarketMatching[] playMarketProducts;

    [System.Serializable]
    public class BasketItem
    {
        public bool isOwned = false;
        public ProductTypes type;
    }
    /// <summary>
    /// Корзина покупок
    /// </summary>
    public BasketItem[] basket;

    void Start()
    {
#if UNITY_IPHONE
        MyMaze.Instance.AppStore.OnTransactionSuccess += OnTransactionSuccess;
#endif
#if UNITY_ANDROID
        MyMaze.Instance.PlayMarket.OnTransactionSuccess += OnTransactionSuccess;
#endif
    }

    /// <summary>
    /// Просим купить продукт
    /// </summary>
    /// <param name="type">Тип продукта</param>
    public void BuyRequest(ProductTypes type)
    {
        if (OnBuyRequest != null)
            OnBuyRequest(type);
        Debug.Log("Отправляю запрос на покупку продукта: " + type.ToString("g"));
#if UNITY_IPHONE
        if (MyMaze.Instance.AppStore.IsInitalized)
            IOSInAppPurchaseManager.Instance.buyProduct(GetProduct<MarketMatching>(type).productId);
        else
            Debug.Log("AppStore не инициализирован");
#endif
#if UNITY_ANDROID
        if (MyMaze.Instance.PlayMarket.IsInitalized)
            MyMaze.Instance.PlayMarket.Purchase(GetProduct<MarketMatching>(type).productId);
        else
            Debug.LogWarning("Play market не инициализирован");        
#endif
#if UNITY_EDITOR
        OnTransactionSuccess(type);
#endif
    }

    /// <summary>
    /// Какаято транзакция была успешно проведена
    /// </summary>
    /// <param name="type">Тип продукта</param>
    void OnTransactionSuccess(ProductTypes type)
    {
        switch (type)
        {
            case ProductTypes.NoAds:
                OnNoAdsSuccess();
                break;
            case ProductTypes.BoosterTimeMachine:
                OnTimeMachineSuccess();
                break;
            case ProductTypes.UnlimitedLives:
                OnUnlimitedLivesSuccess();
                break;
            case ProductTypes.FiveLives:
                OnFiveLivesSuccess();
                break;
            case ProductTypes.BoosterTeleport:
                OnTeleportSuccess();
                break;
        }
        if (OnBuyed != null)
            OnBuyed(type);
        Save();
    }

    /// <summary>
    /// Успешно проведена покупка пакета "Без рекламы"
    /// </summary>
    void OnNoAdsSuccess()
    {
        BasketItem item = GetBasketItem(ProductTypes.NoAds);
        if (!item.isOwned)
        {
            Debug.Log("Купили отсутствие рекламы");
            item.isOwned = true;
            if (OnNoAdsBuyed != null)
                OnNoAdsBuyed();
        }
        else
            Debug.Log("Вы уже покупали отсутсвие рекламы");
    }

    /// <summary>
    /// Успешно проведена покупка бустера "Машина времени"
    /// </summary>
    void OnTimeMachineSuccess()
    {
        BasketItem item = GetBasketItem(ProductTypes.BoosterTimeMachine);
        if (!item.isOwned)
        {
            Debug.Log("Купили бустер Машина Времени");
            item.isOwned = true;
            if (OnTimeMachineBuyed != null)
                OnTimeMachineBuyed();
        }
        else
            Debug.Log("Вы уже покупали бустер Машина Времени");
    }

    /// <summary>
    /// Успешно проведена покупка бесконечных жизней
    /// </summary>
    void OnUnlimitedLivesSuccess()
    {
        BasketItem item = GetBasketItem(ProductTypes.UnlimitedLives);
        if (!item.isOwned)
        {
            Debug.Log("Купили бесконечные жизни");
            item.isOwned = true;
            if (OnUnlimitedLivesBuyed != null)
                OnUnlimitedLivesBuyed();
        }
        else
            Debug.Log("Вы уже покупали бесконечные жизни");
    }

    /// <summary>
    /// Успешно проведена покупка 5 жизней
    /// </summary>
    void OnFiveLivesSuccess()
    {
        BasketItem item = GetBasketItem(ProductTypes.FiveLives);
        if (!item.isOwned)
        {
            Debug.Log("Купили 5 жизней");
            item.isOwned = true;
            if (OnFiveLivesBuyed != null)
                OnFiveLivesBuyed();
        }
        else
            Debug.Log("Вы уже покупали 5 жизней");
    }

    /// <summary>
    /// Успешно проведена покупка пакета "Бустер телепорт"
    /// </summary>
    void OnTeleportSuccess()
    {
        BasketItem item = GetBasketItem(ProductTypes.BoosterTeleport);
        if (!item.isOwned)
        {
            Debug.Log("Купили бустер телепорт");
            item.isOwned = true;
            if (OnTeleportBuyed != null)
                OnTeleportBuyed();
        }
        else
            Debug.Log("Вы уже покупали бустер телепорт");
    }

    /// <summary>
    /// Потребить покупку в 5 жизней
    /// </summary>
    /// <returns></returns>
    public bool ConsumeFiveLives()
    {
        BasketItem item = GetBasketItem(ProductTypes.FiveLives);
        if (item.isOwned)
        {
            item.isOwned = false;
            Debug.Log("Успешно потребили покупку: 5 жизней");
            return true;
        }
        Debug.Log("Не могу потребить покупку в 5 жизней, т.к. она не была куплена");
        return false;
    }

    /// <summary>
    /// Потребить покупку Бустер телепорт
    /// </summary>
    /// <returns></returns>
    public bool ConsumeTeleport()
    {
        BasketItem item = GetBasketItem(ProductTypes.BoosterTeleport);
        if (item.isOwned)
        {
            item.isOwned = false;
            Debug.Log("Успешно потребили покупку: бустер телепорт");
            return true;
        }
        Debug.Log("Не могу потребить покупку бустер телепорт, т.к. она не была куплена");
        return false;
    }

    /// <summary>
    /// Получить элемент корзины через тип продукта
    /// </summary>
    /// <param name="type">Тип продукта</param>
    /// <returns></returns>
    BasketItem GetBasketItem(ProductTypes type)
    {
        foreach (BasketItem item in basket)
            if (item.type == type)
                return item;

        return null;
    }

    /// <summary>
    /// Получить ссылку на описание продукта через тип
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type">Тип продукта</param>
    /// <returns></returns>
    public T GetProduct<T>(ProductTypes type) where T : IStoreMatch
    {
#if UNITY_IPHONE
        foreach (MarketMatching product in appStoreProducts)
            if (product.type == type)
                return (T)(IStoreMatch) product;
#endif
#if UNITY_ANDROID
        foreach (MarketMatching product in playMarketProducts)
            if (product.type == type)
                return (T)(IStoreMatch) product;
#endif
        return (T)(IStoreMatch) null;
    }
    /// <summary>
    /// Получить ссылку на описание продукта через строковый идентификатор в магазине
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id">строковый идентификатор в магазине</param>
    /// <returns></returns>
    public T GetProduct<T>(string id) where T : IStoreMatch
    {
#if UNITY_IPHONE
        foreach (MarketMatching product in appStoreProducts)
            if (product.productId.Equals(id))
                return (T)(IStoreMatch)product;
#endif
#if UNITY_ANDROID
        foreach (MarketMatching product in playMarketProducts)
            if (product.productId.Equals(id))
                return (T)(IStoreMatch)product;
#endif
        return (T)(IStoreMatch)null;
    }

    /// <summary>
    /// Проверяем приобретен ли элемент корзины
    /// </summary>
    /// <param name="type">Тип продукта</param>
    /// <returns></returns>
    public bool IsOwned(ProductTypes type)
    {
        foreach (BasketItem item in basket)
            if (item.type == type)
                return item.isOwned;

        return false;
    }

    public void RestoreIAPs()
    {
        Debug.Log("Пытаюсь восстановить покупки");
#if UNITY_EDITOR
        foreach (BasketItem item in basket)
            OnTransactionSuccess(item.type);        
#elif UNITY_IPHONE        
        IOSInAppPurchaseManager.instance.restorePurchases();
#elif UNITY_ANDROID
        MyMaze.Instance.PlayMarket.CheckPurchasedProductsAndConsume();
#endif
    }

    /// <summary>
    /// Сохранить локально
    /// </summary>
    public void Save()
    {
        foreach(BasketItem item in basket)
            PlayerPrefs.SetInt("BasketItem#" + item.type.ToString("g"), System.Convert.ToInt32(item.isOwned));
    }

    /// <summary>
    /// Загрузить
    /// </summary>
    public void Load()
    {
        foreach (BasketItem item in basket)
            item.isOwned = System.Convert.ToBoolean(PlayerPrefs.GetInt("BasketItem#" + item.type.ToString("g")));
    }

    /// <summary>
    /// Сбросить сохранения
    /// </summary>
    public void ResetSaves()
    {
        throw new System.NotImplementedException();
    }
}
