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

    public Pack[] noAdsPacks;
    
    /// <summary>
    /// Слушатель состояния интерстишалов
    /// </summary>
    HZInterstitialAd.AdDisplayListener HZInterstitialListener = delegate(string adState, string adTag)
    {
        Debug.Log("HZInterstitialAd: " + adState);
        if (adState.Equals("hide"))
        {
            lastAdsHideTime = Timers.Instance.UnixTimestamp;
            HZInterstitialAd.fetch();
        }
    };

    /// <summary>
    /// Слушатель на Reward видео
    /// </summary>
    HZIncentivizedAd.AdDisplayListener HZIncentivizedListener = delegate(string adState, string adTag)
    {
        Debug.Log("HZIncentivizedAd: " + adState);
        if (adState.Equals("hide"))
        {
            if (adTag.Equals("life"))
            {
                lastAdsHideTime = Timers.Instance.UnixTimestamp;
                MyMaze.Instance.Life.RestoreOneUnit();
            }
            else if (adTag.Equals("moves"))
            {
                lastAdsHideTime = Timers.Instance.UnixTimestamp;
                GameObject.FindObjectOfType<AdsMovesUI>().AddMovesAndClose();
            }
            HZIncentivizedAd.fetch();
        }
    };
    
    void Awake()
    {
        HZInterstitialAd.setDisplayListener(HZInterstitialListener);
        HZIncentivizedAd.setDisplayListener(HZIncentivizedListener);
    }

    void Start()
    {
        if (MyMaze.Instance.IsFirstSceneLoad)
            StartCoroutine(ShowOnLaunchInterstitial());

        MyMaze.Instance.OnLevelLoad += OnLevelLoadOrRestarted;
        MyMaze.Instance.OnLevelRestarted += OnLevelLoadOrRestarted;

        levelFrequencyCounter = levelFrequency;
    }

    /// <summary>
    /// Игровой уровень был загружен или перезапущен
    /// </summary>
    /// <param name="level"></param>
    void OnLevelLoadOrRestarted(Level level)
    {
        //не считаем счетчик если текущий пак без рекламы
        if (!IsCurrentPackWithAds())
            return;

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

        if (IsCurrentPackWithAds())
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

        if (IsCurrentPackWithAds())
            if (!MyMaze.Instance.InApps.IsOwned(ProductTypes.NoAds))
                if (Mathf.Abs(Timers.Instance.UnixTimestamp - lastAdsHideTime) > adsShowTrashhold)
                    HZInterstitialAd.show(); //HZInterstitialAd.chartboostShowForLocation("mymaze.onEndOfGame");
    }

    /// <summary>
    /// Текущий пак позволяет показывать рекламу?
    /// </summary>
    /// <returns></returns>
    bool IsCurrentPackWithAds()
    {
        bool allowedAds = true;
        foreach (Pack pack in noAdsPacks)
            if (MyMaze.Instance.LastSelectedPack.packName.Equals(pack.packName))
                if (!pack.IsAllreadyPassed)
                {
                    allowedAds = false;
                    break;
                }

        return allowedAds;
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
        HZIncentivizedAd.show("life");        
    }

    /// <summary>
    /// Показываем Reward видео
    /// </summary>
    public void ShowRewardVideoForMoves()
    {
        HZIncentivizedAd.show("moves");
    }

    /// <summary>
    /// Приложение сварачивается или разварачивается
    /// </summary>
    /// <param name="pause"></param>
    void OnApplicationPause(bool pause)
    {
        if (!pause)
            StartCoroutine(ShowOnLaunchInterstitial());
    }
}
