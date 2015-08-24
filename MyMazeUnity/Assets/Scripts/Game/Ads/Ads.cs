using UnityEngine;
using System.Collections;

public class Ads : MonoBehaviour {

    public Deligates.SimpleEvent OnMyMazeUILifeAds;
    public static Deligates.SimpleEvent OnIncentivizedEnd; //это же статик а у меня получется 2 копии ADS будет изза этого

    /// <summary>
    /// Флаг который отслеживает нужно ли давать жизнь после просмотра видео рекламы
    /// </summary>
    private bool oneLifeReward = false;

    /// <summary>
    /// Показана ли сейчас реклама
    /// </summary>
    private static bool isAdsShowed = false;

    /// <summary>
    /// Время последнего показа рекламы, штамп на закрытие
    /// </summary>
    private static int lastAdsHideTime = 0;

    /// <summary>
    /// Время в секундах как минимальный интервал времени перед показом следующей рекламы
    /// </summary>
    public int adsShowTrashhold = 3;
    
    /// <summary>
    /// Слушатель состояния интерстишалов
    /// </summary>
    HZInterstitialAd.AdDisplayListener HZInterstitialListener = delegate(string adState, string adTag)
    {
        Debug.Log("\n" +
            "HZInterstitialAd..." + "\n" +
            "adState: " + adState + "\n" +
            "adTag: " + adTag + "\n");
        if (adState.Equals("show"))
        {
            isAdsShowed = true;
        }
        else if (adState.Equals("hide"))
        {
            isAdsShowed = false;
            lastAdsHideTime = Timers.Instance.UnixTimestamp;
        }
    };

    /// <summary>
    /// Слушатель состояния Reward видео
    /// </summary>
    HZIncentivizedAd.AdDisplayListener HZIncentivizedListener = delegate(string adState, string adTag)
    {
        Debug.Log("\n" +
            "HZIncentivizedAd..." + "\n" +
            "adState: " + adState + "\n" +
            "adTag: " + adTag + "\n");

        if (adState.Equals("show"))
        {
            isAdsShowed = true;
        }
        else if (adState.Equals("hide"))
        {
            isAdsShowed = false;
            lastAdsHideTime = Timers.Instance.UnixTimestamp;

            if (OnIncentivizedEnd != null)
                OnIncentivizedEnd();
        }
    };
    
    void Awake()
    {
        //HZInterstitialAd.chartboostShowForLocation("mymaze.onLaunch");
        HZInterstitialAd.setDisplayListener(HZInterstitialListener);
        HZIncentivizedAd.setDisplayListener(HZIncentivizedListener);
    }

    void Start()
    {
        if (MyMaze.Instance.IsFirstSceneLoad)
            StartCoroutine(ShowOnLaunchInterstitial());

        OnIncentivizedEnd += RestoreLifeWhenIncentivizedAdEnd;
        OnIncentivizedEnd += FetchHZIncentivizedAdWhenEnd;
    }

    /// <summary>
    /// проверить и добавить 1 жизнь когда кончилось ревард видео
    /// </summary>
    void RestoreLifeWhenIncentivizedAdEnd()
    {
        if (oneLifeReward)
        {
            oneLifeReward = false;
            MyMaze.Instance.Life.RestoreOneUnit();
        }
    }

    /// <summary>
    /// Кешируем видео по окончании просмотра
    /// </summary>
    void FetchHZIncentivizedAdWhenEnd()
    {
        HZIncentivizedAd.fetch();
    }

    /// <summary>
    /// Показать интерстишал для OnLaunch
    /// </summary>
    IEnumerator ShowOnLaunchInterstitial()
    {
        yield return new WaitForSeconds(0.5f);

        if (!MyMaze.Instance.InApps.IsOwned(ProductTypes.NoAds))
            if (!isAdsShowed)
                if (Mathf.Abs(Timers.Instance.UnixTimestamp - lastAdsHideTime) > adsShowTrashhold)
                    HZInterstitialAd.show();
    }

    /// <summary>
    /// Вызывает событие "Показано окно с предложением посмотреть рекламу для восстановления жизней"
    /// </summary>
    public void CallMyMazeUILifeAdsShowEvent()
    {
        if (OnMyMazeUILifeAds != null)
            OnMyMazeUILifeAds();
    }

    /// <summary>
    /// Показываем Reward видео для получения одной жизни
    /// </summary>
    public void ShowRewardVideoForLife()
    {
        HZIncentivizedAd.show();
        oneLifeReward = true;
    }

    /// <summary>
    /// Показываем Reward видео
    /// </summary>
    public void ShowRewardVideo()
    {
        HZIncentivizedAd.show();
    }

    void OnApplicationPause(bool pause)
    {
        if (!pause)
            StartCoroutine(ShowOnLaunchInterstitial());
    }
}
