using UnityEngine;
using System.Collections;

public class Ads : MonoBehaviour {

    public Deligates.SimpleEvent OnMyMazeUILifeAds;

    /// <summary>
    /// Частота показа рекламы между уровнями
    /// example: 4 значит что через 4 сыграных или перезапущенных уровня покажется реклама
    /// </summary>
    public int levelFrequency = 4;

    private int levelFrequencyCounter = 0;

    /// <summary>
    /// Время последнего показа рекламы, штамп на закрытие
    /// </summary>
    private static int lastAdsHideTime = 0;

    /// <summary>
    /// Время в секундах как минимальный интервал времени перед показом следующей рекламы
    /// </summary>
    public int adsShowTrashhold = 5;
    
    /// <summary>
    /// Слушатель состояния интерстишалов
    /// </summary>
    HZInterstitialAd.AdDisplayListener HZInterstitialListener = delegate(string adState, string adTag)
    {
        if (adState.Equals("show"))
        {
            
        }
        else if (adState.Equals("hide"))
        {
            lastAdsHideTime = Timers.Instance.UnixTimestamp;
        }
    };

    /// <summary>
    /// Слушатель состояния Reward видео для жизней
    /// </summary>
    HZIncentivizedAd.AdDisplayListener HZIncentivizedLifeListener = delegate(string adState, string adTag)
    {
        if (adState.Equals("hide"))
        {
            lastAdsHideTime = Timers.Instance.UnixTimestamp;
            MyMaze.Instance.Life.RestoreOneUnit();
            HZIncentivizedAd.fetch();
        }
    };

    /// <summary>
    /// Слушатель состояния Reward видео для ходов
    /// </summary>
    HZIncentivizedAd.AdDisplayListener HZIncentivizedMovesListener = delegate(string adState, string adTag)
    {
        if (adState.Equals("hide"))
        {
            lastAdsHideTime = Timers.Instance.UnixTimestamp;
            GameObject.FindObjectOfType<AdsMovesUI>().AddMovesAndClose();
            HZIncentivizedAd.fetch();
        }
    };
    
    void Awake()
    {        
        HZInterstitialAd.setDisplayListener(HZInterstitialListener);
    }

    void Start()
    {
        if (MyMaze.Instance.IsFirstSceneLoad)
            StartCoroutine(ShowOnLaunchInterstitial());

        MyMaze.Instance.OnLevelLoad += OnLevelLoadOrRestarted;
        MyMaze.Instance.OnLevelRestarted += OnLevelLoadOrRestarted;
    }

    /// <summary>
    /// Игровой уровень был загружен или перезапущен
    /// </summary>
    /// <param name="level"></param>
    void OnLevelLoadOrRestarted(Level level)
    {
        levelFrequencyCounter++;

        if (levelFrequencyCounter >= levelFrequency)
        {
            levelFrequencyCounter = 0;
            StartCoroutine(ShowOnGameEndInterstitial());
        }
    }

    /// <summary>
    /// Показать интерстишал для OnLaunch
    /// </summary>
    IEnumerator ShowOnLaunchInterstitial()
    {
        yield return new WaitForSeconds(0.5f);

        if (!MyMaze.Instance.InApps.IsOwned(ProductTypes.NoAds))
            if (Mathf.Abs(Timers.Instance.UnixTimestamp - lastAdsHideTime) > adsShowTrashhold)
                HZInterstitialAd.show(); //HZInterstitialAd.chartboostShowForLocation("mymaze.onLaunch");
    }

    /// <summary>
    /// Показать интерстишал для onEndOfGame
    /// </summary>
    IEnumerator ShowOnGameEndInterstitial()
    {
        yield return new WaitForSeconds(0.5f);

        if (!MyMaze.Instance.InApps.IsOwned(ProductTypes.NoAds))
            if (Mathf.Abs(Timers.Instance.UnixTimestamp - lastAdsHideTime) > adsShowTrashhold)
                HZInterstitialAd.show(); //HZInterstitialAd.chartboostShowForLocation("mymaze.onEndOfGame");
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
        HZIncentivizedAd.setDisplayListener(HZIncentivizedLifeListener);
        HZIncentivizedAd.show();        
    }

    /// <summary>
    /// Показываем Reward видео
    /// </summary>
    public void ShowRewardVideoForMoves()
    {
        HZIncentivizedAd.setDisplayListener(HZIncentivizedMovesListener);
        HZIncentivizedAd.show();
    }


    void OnApplicationPause(bool pause)
    {
        if (!pause)
            StartCoroutine(ShowOnLaunchInterstitial());
    }
}
