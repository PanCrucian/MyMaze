using UnityEngine;
using System.Collections;

public class Ads : MonoBehaviour, ISavingElement {

    public Deligates.SimpleEvent OnMyMazeUILifeAds;

    /// <summary>
    /// Частота показа рекламы между уровнями
    /// </summary>
    public int frequency = 1;
    private int frequencyCounter = 0;

    /// <summary>
    /// Показывать интерстишал не чаще чем раз в ...
    /// </summary>
    public int cooldown = 60;

    /// <summary>
    /// Время последнего показа рекламы, штамп на закрытие
    /// </summary>
    private int lastAdsHideTime = 0;

    /// <summary>
    /// Время последнего показа интерстишела
    /// </summary>
    private int lastInterstitialHideTime = 0;

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
        Debug.Log("mymaze.onLaunch: " + HZInterstitialAd.chartboostIsAvailableForLocation("mymaze.onLaunch").ToString());
        if (adState.Equals("hide"))
        {
            MyMaze.Instance.Ads.lastAdsHideTime = Timers.Instance.UnixTimestamp;
            MyMaze.Instance.Ads.lastInterstitialHideTime = Timers.Instance.UnixTimestamp;

            HZInterstitialAd.chartboostFetchForLocation("mymaze.onLaunch");
            HZInterstitialAd.chartboostFetchForLocation("mymaze.onEndOfGame");
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
#if UNITY_IPHONE
            MyMaze.Instance.Sounds.UnMute();
#endif
            MyMaze.Instance.Ads.lastAdsHideTime = Timers.Instance.UnixTimestamp;
            if (adTag.Equals("life"))
            {
                MyMaze.Instance.Life.RestoreOneUnit();
                if(GameLevel.Instance == null)
                    MyMaze.Instance.LevelLoadAction(MyMaze.Instance.LastSelectedLevel, true);
                else
                {
                    AdsLifeUI adsLifeUI = GameObject.FindObjectOfType<AdsLifeUI>();
                    if (adsLifeUI != null)
                        adsLifeUI.Hide();
                    GameLevel.Instance.OnRestartRequest();
                }
            }
            else if (adTag.Equals("moves"))
                GameObject.FindObjectOfType<AdsMovesUI>().AddMovesAndClose();

            HZIncentivizedAd.fetch();
        }
    };
    
    void Awake()
    {
        HZInterstitialAd.setDisplayListener(HZInterstitialListener);
        HZIncentivizedAd.setDisplayListener(HZIncentivizedListener);
    }

    IEnumerator Start()
    {
        HZInterstitialAd.chartboostFetchForLocation("mymaze.onLaunch");
        HZInterstitialAd.chartboostFetchForLocation("mymaze.onEndOfGame");
        HZIncentivizedAd.fetch();
        yield return new WaitForSeconds(5f);
        if (MyMaze.Instance.IsFirstSceneLoad)
            StartCoroutine(ShowOnLaunchInterstitial());
    }

    /// <summary>
    /// Проверить и запустить интерстишал для OnGameEnd рекламы
    /// </summary>
    public void CheckAndLaunchOnEndGameAds()
    {
        //не считаем счетчик если текущий пак без рекламы
        if (!IsCurrentPackWithAds())
            return;

        frequencyCounter++;
        if (frequencyCounter >= frequency)
        {
            frequencyCounter = 0;
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
                    HZInterstitialAd.chartboostShowForLocation("mymaze.onLaunch"); //HZInterstitialAd.show(); //
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
                    if (Mathf.Abs(Timers.Instance.UnixTimestamp - lastInterstitialHideTime) > cooldown)
                    {
                        Debug.Log("Показываю рекламу");
                        lastInterstitialHideTime = Timers.Instance.UnixTimestamp;
                        HZInterstitialAd.chartboostShowForLocation("mymaze.onEndOfGame"); //HZInterstitialAd.show(); //
                    }
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
#if UNITY_IPHONE
        MyMaze.Instance.Sounds.Mute();
#endif
    }

    /// <summary>
    /// Показываем Reward видео
    /// </summary>
    public void ShowRewardVideoForMoves()
    {
        HZIncentivizedAd.show("moves");
#if UNITY_IPHONE
        MyMaze.Instance.Sounds.Mute();
#endif
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

    public void Save()
    {
        PlayerPrefs.SetInt("adsFrequency", frequency);
        PlayerPrefs.SetInt("adsCooldown", cooldown);
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("adsFrequency"))
            frequency = PlayerPrefs.GetInt("adsFrequency");

        if (PlayerPrefs.HasKey("adsCooldown"))
            cooldown = PlayerPrefs.GetInt("adsCooldown");
    }

    public void ResetSaves()
    {
        PlayerPrefs.DeleteKey("adsFrequency");
        PlayerPrefs.DeleteKey("adsCooldown");
    }
}
