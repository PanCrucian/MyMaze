using UnityEngine;
using System.Collections;

public class InApps : MonoBehaviour, ISavingElement {

    public Deligates.SimpleEvent OnNoAdsBuyed;
    public Deligates.SimpleEvent OnTimeMachineBuyed;
    public Deligates.SimpleEvent OnUnlimitedLivesBuyed;

    public interface IStoreMatch
    {

    }

    [System.Serializable]
    public class AppStoreMatching : IStoreMatch
    {
        public string productId;
        public ProductTypes type;
    }
    /// <summary>
    /// Сопаставления продуктов с идетификаторами магазина Apple
    /// </summary>
    public AppStoreMatching[] appStoreProducts;

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
    }

    /// <summary>
    /// Просим купить продукт
    /// </summary>
    /// <param name="type">Тип продукта</param>
    public void BuyRequest(ProductTypes type)
    {
#if UNITY_IPHONE
        if (MyMaze.Instance.AppStore.IsInitalized)
        {
            Debug.Log("Отправляю запрос на покупку продукта: " + type.ToString("g"));
            IOSInAppPurchaseManager.instance.buyProduct(GetProduct<AppStoreMatching>(type).productId);
        }
        else
            Debug.Log("AppStore не инициализирован");
#endif
    }

    /// <summary>
    /// Какаято транзакция была успешно проведена
    /// </summary>
    /// <param name="pType"></param>
    void OnTransactionSuccess(ProductTypes pType)
    {
        switch (pType)
        {
            case ProductTypes.NoAds:
                OnNoAdsTransactionSuccess();
                break;
            case ProductTypes.BoosterTimeMachine:
                OnTimeMachineSuccess();
                break;
            case ProductTypes.UnlimitedLives:
                OnUnlimitedLivesSuccess();
                break;
        }
        Save();
    }

    /// <summary>
    /// Успешно проведена покупка пакета "Без рекламы"
    /// </summary>
    void OnNoAdsTransactionSuccess()
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
        foreach (AppStoreMatching product in appStoreProducts)
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
        foreach (AppStoreMatching product in appStoreProducts)
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
#if UNITY_IPHONE
        IOSInAppPurchaseManager.instance.restorePurchases();
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
