using UnityEngine;
using System.Collections;
using Heyzap;

public class Ads : GentleMonoBeh, ISavingElement {

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
    /// Прошло кадров с момента запуска игры
    /// </summary>
    private int gentleFrames = 0;

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
        Debug.Log("HZInterstitialAd: " + adState + ", Tag: " + adTag);
        if (adState.Equals("hide"))
        {
            MyMaze.Instance.Ads.lastAdsHideTime = Timers.Instance.UnixTimestamp;
            MyMaze.Instance.Ads.lastInterstitialHideTime = Timers.Instance.UnixTimestamp;

            switch (adTag)
            {
                case "mymaze-onpause":
                case "mymaze-onendofgame":
                    MyMaze.Instance.Ads.FetchInterstitialAd(adTag);
                    break;
                case "mymaze-onlaunch":
                    MyMaze.Instance.Ads.FetchInterstitialAd(adTag, true);
                    break;
            }
        }
        if (adState.Equals("fetch_failed"))
        {
            switch (adTag)
            {
                case "mymaze-onpause":
                case "mymaze-onendofgame":
                    MyMaze.Instance.Ads.FetchInterstitialAd(adTag, false, UnityEngine.Random.Range(15f, 30f));
                    break;
                case "mymaze-onlaunch":
                    MyMaze.Instance.Ads.FetchInterstitialAd(adTag, true, UnityEngine.Random.Range(15f, 30f));
                    break;
            }
        }
    };

    /// <summary>
    /// Слушатель на Reward видео
    /// </summary>
    HZIncentivizedAd.AdDisplayListener HZIncentivizedListener = delegate(string adState, string adTag)
    {
        Debug.Log("HZIncentivizedAd: " + adState + ", Tag: " + adTag);
        if (adState.Equals("hide"))
        {
#if UNITY_IPHONE
            MyMaze.Instance.Sounds.UnMute();
#endif
            MyMaze.Instance.Ads.lastAdsHideTime = Timers.Instance.UnixTimestamp;
            switch (adTag)
            {
                case "mymaze-adlife":
                    MyMaze.Instance.Ads.AdLifeClosed();
                    break;
                case "mymaze-admoves":
                    MyMaze.Instance.Ads.AdMovesClosed();
                    break;
            }
            if (GameLevel.Instance != null)
                MyMaze.Instance.Ads.OffSwitchersOnGameLevel();

            MyMaze.Instance.Ads.FetchIncentivizedAd(adTag);
        }
    };

    /// <summary>
    /// Закрылась ревард видео для ходов
    /// </summary>
    public void AdMovesClosed()
    {
        GameObject.FindObjectOfType<AdsMovesUI>().AddMovesAndClose();
    }

    /// <summary>
    /// Закрылось ревард видео для жизней
    /// </summary>
    public void AdLifeClosed()
    {
        MyMaze.Instance.Life.RestoreOneUnit();
        if (GameLevel.Instance == null)
            MyMaze.Instance.LevelLoadAction(MyMaze.Instance.LastSelectedLevel, true);
        else
        {
            MyMaze.Instance.Life.RestoreOneUnit();
            AdsLifeUI adsLifeUI = GameObject.FindObjectOfType<AdsLifeUI>();
            if (adsLifeUI != null)
                adsLifeUI.Hide();
            GameLevel.Instance.OnRestartRequest();
        }
    }

    /// <summary>
    /// Костыль. Выключает все переключатели на уровне
    /// </summary>
    public void OffSwitchersOnGameLevel()
    {
        //StartCoroutine(OffSwitchersOnGameLevelNumerator());
    }
    IEnumerator OffSwitchersOnGameLevelNumerator()
    {
        yield return new WaitForEndOfFrame();
        Switcher[] switchers = GameObject.FindObjectsOfType<Switcher>();
        foreach (Switcher switcher in switchers)
        {
            switcher.Off();
            switcher.ReleaseButton();
        }
    }

    void Awake()
    {
        HZInterstitialAd.SetDisplayListener(HZInterstitialListener);
        HZIncentivizedAd.SetDisplayListener(HZIncentivizedListener);
    }

    IEnumerator Start()
    {
        SetGentleCPURate(30);
        yield return new WaitForEndOfFrame();
        FetchInterstitialAd("mymaze-onlaunch", true);
        yield return new WaitForSeconds(0.5f);
        FetchInterstitialAd("mymaze-onpause");
        yield return new WaitForSeconds(0.5f);
        FetchInterstitialAd("mymaze-onendofgame");
        yield return new WaitForSeconds(0.5f);
        FetchIncentivizedAd("mymaze-adlife");
        yield return new WaitForSeconds(0.5f);
        FetchIncentivizedAd("mymaze-admoves");
        yield return new WaitForSeconds(2f);

        StartCoroutine(ShowOnLaunchInterstitial());
    }

    public override void GentleUpdate()
    {
        base.GentleUpdate();
        gentleFrames++;
    }

    IEnumerator FetchInterstitialNumerator(string tag, bool isChartboost, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (!isChartboost)
            HZInterstitialAd.Fetch(tag);
        else
            HZInterstitialAd.ChartboostFetchForLocation(tag);
    }

    /// <summary>
    /// Кешируем рекламу картинкой для хейзапа
    /// </summary>
    /// <param name="tag"></param>
    public void FetchInterstitialAd(string tag)
    {
        FetchInterstitialAd(tag, false);
    }

    /// <summary>
    /// Кешируем рекламу картинкой для чартбуста
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="isChartboost"></param>
    public void FetchInterstitialAd(string tag, bool isChartboost)
    {
        FetchInterstitialAd(tag, false, 0.5f);
    }

    public void FetchInterstitialAd(string tag, bool isChartboost, float delayTime)
    {
        StartCoroutine(FetchInterstitialNumerator(tag, isChartboost, delayTime));
    }

    /// <summary>
    /// Кешируем видео для награды
    /// </summary>
    /// <param name="tag"></param>
    public void FetchIncentivizedAd(string tag)
    {
        StartCoroutine(FetchIncentivizedAdNumerator(tag));
    }

    IEnumerator FetchIncentivizedAdNumerator(string tag)
    {
        yield return new WaitForSeconds(0.25f);
        HZIncentivizedAd.Fetch(tag);
    }

    /// <summary>
    /// Проверить и запустить интерстишал для OnGameEnd рекламы
    /// </summary>
    public void CheckAndLaunchOnEndGameAds()
    {
        if (Mathf.Abs(Timers.Instance.UnixTimestamp - lastAdsHideTime) < adsShowTrashhold)
            return;
		
		frequencyCounter++;

        //если текущий пак без рекламы
        if (!IsCurrentPackWithAds())
            return;

        //если колдаун не позволяет показать рекламу
        if (Mathf.Abs(Timers.Instance.UnixTimestamp - lastInterstitialHideTime) < cooldown)
            return;

        //если куплен пакет без рекламы
        if (MyMaze.Instance.InApps.IsOwned(ProductTypes.NoAds))
            return;

        if (frequencyCounter >= frequency)
        {            
            frequencyCounter = 0;
            StartCoroutine(ShowOnGameEndInterstitial());
        }
	}
	
	/// <summary>
	/// Показать интерстишал для onEndOfGame
	/// </summary>
	IEnumerator ShowOnGameEndInterstitial()
	{
		yield return new WaitForSeconds(0.5f);
		HZShowOptions showOptions = new HZShowOptions();
		showOptions.Tag = "mymaze-onendofgame";
        if(HZInterstitialAd.IsAvailable(showOptions.Tag))
		    HZInterstitialAd.ShowWithOptions(showOptions);
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
                    HZInterstitialAd.ChartboostShowForLocation("mymaze-onlaunch");
    }

    /// <summary>
    /// Показать интерстишал для OnPause
    /// </summary>
    IEnumerator ShowOnPauseInterstitial()
    {
        yield return new WaitForSeconds(0.5f);

        if (IsCurrentPackWithAds())
            if (!MyMaze.Instance.InApps.IsOwned(ProductTypes.NoAds))
                if (Mathf.Abs(Timers.Instance.UnixTimestamp - lastAdsHideTime) > adsShowTrashhold)
                    if (Mathf.Abs(Timers.Instance.UnixTimestamp - lastInterstitialHideTime) > cooldown)
                    {
                        HZShowOptions showOptions = new HZShowOptions();
                        showOptions.Tag = "mymaze-onpause";
                        if (HZInterstitialAd.IsAvailable(showOptions.Tag))
                            HZInterstitialAd.ShowWithOptions(showOptions);
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
        HZIncentivizedShowOptions showOptions = new HZIncentivizedShowOptions();
        showOptions.Tag = "mymaze-adlife";
        if (HZIncentivizedAd.IsAvailable(showOptions.Tag))
            HZIncentivizedAd.ShowWithOptions(showOptions);
#if UNITY_IPHONE && !UNITY_EDITOR
        MyMaze.Instance.Sounds.Mute();
#endif
    }

    /// <summary>
    /// Показываем Reward видео
    /// </summary>
    public void ShowRewardVideoForMoves()
    {
        HZIncentivizedShowOptions showOptions = new HZIncentivizedShowOptions();
        showOptions.Tag = "mymaze-admoves";
        if (HZIncentivizedAd.IsAvailable(showOptions.Tag))
            HZIncentivizedAd.ShowWithOptions(showOptions);
#if UNITY_IPHONE && !UNITY_EDITOR
        MyMaze.Instance.Sounds.Mute();
#endif
    }

    /// <summary>
    /// Приложение сварачивается или разварачивается
    /// </summary>
    /// <param name="pause"></param>
    void OnApplicationPause(bool pause)
    {
        if (gentleFrames < 2)
            return;
        if (!pause)
            StartCoroutine(ShowOnPauseInterstitial());
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
