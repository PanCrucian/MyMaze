using UnityEngine;
using System.Collections;
using DeltaDNA;
using System.Collections.Generic;
public class MyDDNA : MonoBehaviour {

    private bool uploadRoutine = false;
    private float uploadDelay = 1f;

    enum TransactionTypes
    {
        PURCHASE,
        SALE,
        TRADE
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        foreach (Pack pack in MyMaze.Instance.packs)
            pack.OnFirstTimePassed += OnPackFirstTimePassed;

        foreach (Level level in MyMaze.Instance.levels)
        {
            level.OnPassed += OnLevelPassed;
            level.OnFailed += OnLevelFailed;
            level.OnStarted += OnLevelStarted;
        }

        MyMaze.Instance.Notifications.OnNewNotice += OnNewNotification;
        MyMaze.Instance.InApps.OnBuyRequest += OnBuyRequest;
        MyMaze.Instance.InApps.OnBuyed += OnBuyed;
        EngageOnGameStart();
    }

    bool IsDDNAInitalized
    {
        get
        {
            Debug.LogWarning("DDNA не инициализирован");
            return DDNA.Instance.IsInitialised;
        }
    }

    /// <summary>
    /// Запросим параметры из DDNA для события onGameStart
    /// </summary>
    void EngageOnGameStart()
    {
        if (!IsDDNAInitalized)
            return;

        Debug.Log("Запрашиваю из DDNA параметры на onGameStart");
        DDNA.Instance.RequestEngagement("onGameStart", null, (response) =>
        {
            if (response != null && response.ContainsKey("parameters"))
            {
                Dictionary<string, object> parameters = (Dictionary<string, object>)response["parameters"];

                //Частота показа
                if (parameters.ContainsKey("adsFrequency"))
                {
                    int adsFrequency = System.Convert.ToInt32(parameters["adsFrequency"]);
                    if (MyMaze.Instance.Ads.frequency != adsFrequency)
                    {
                        MyMaze.Instance.Ads.frequency = adsFrequency;
                        MyMaze.Instance.Ads.Save();
                        RecordAdsFreqency();
                    }
                }

                //Частота показа во времени
                if (parameters.ContainsKey("adsCooldown"))
                {
                    int adsCooldown = System.Convert.ToInt32(parameters["adsCooldown"]);
                    if (MyMaze.Instance.Ads.cooldown != adsCooldown)
                    {
                        MyMaze.Instance.Ads.cooldown = adsCooldown;
                        MyMaze.Instance.Ads.Save();
                        RecordAdsFreqency();
                    }
                }
            }
        });
    }

    /// <summary>
    /// Пытаемся что-то купить
    /// </summary>
    /// <param name="type"></param>
    void OnBuyRequest(ProductTypes type)
    {
        RecordThumbPurchaseAttempt(type);
    }

    /// <summary>
    /// Что-то купили
    /// </summary>
    /// <param name="type"></param>
    void OnBuyed(ProductTypes type)
    {
        RecordTransaction(type);
    }

    /// <summary>
    /// пак был пройден в первый раз
    /// </summary>
    /// <param name="pack"></param>
    void OnPackFirstTimePassed(Pack pack)
    {
        Pack nextPack = MyMaze.Instance.GetNextPack(pack);
        if (nextPack == null)
            return;
        RecordLevelUp(nextPack);
    }

    /// <summary>
    /// был пройден какйто уровень
    /// </summary>
    /// <param name="level"></param>
    void OnLevelPassed(Level level)
    {
        RecordMissionCompleted(level);
    }

    /// <summary>
    /// Не удалось пройти уровень
    /// </summary>
    /// <param name="level"></param>
    void OnLevelFailed(Level level)
    {
        RecordMissionFailed(level);
    }

    /// <summary>
    /// Стартовал игровой уровень
    /// </summary>
    /// <param name="level"></param>
    void OnLevelStarted(Level level)
    {
        RecordMissionStarted(level);
    }

    /// <summary>
    /// Сгенерировали новое уведомление
    /// </summary>
    /// <param name="id"></param>
    /// <param name="launchTime"></param>
    /// <param name="name"></param>
    void OnNewNotification(int id, System.DateTime launchTime, string name)
    {
        if (name.Equals(MyMaze.Instance.Notifications.restoredLivesName))
            RecordNotificationOpened(id, launchTime, name);
    }

    /// <summary>
    /// Запишем стату о частоте рекламы
    /// </summary>
    void RecordAdsFreqency()
    {
        if (!IsDDNAInitalized)
            return;

        DDNA.Instance.RecordEvent(
            "adsFreqSet",
            new EventBuilder()
                .AddParam("adsFrequency", MyMaze.Instance.Ads.frequency)
                .AddParam("adsCooldown", MyMaze.Instance.Ads.cooldown)
        );
        Upload();
    }

    /// <summary>
    /// Запишем стату о том что открылся новый пак
    /// </summary>
    /// <param name="pack"></param>
    void RecordLevelUp(Pack pack)
    {
        if (!IsDDNAInitalized)
            return;

        DDNA.Instance.RecordEvent(
            "levelUp",
            new EventBuilder()
                .AddParam("levelUpName", pack.packName)
        );
        Upload();
    }

    /// <summary>
    /// Запишем стату о прохождении уровня
    /// </summary>
    /// <param name="level"></param>
    void RecordMissionCompleted(Level level)
    {
        if (!IsDDNAInitalized)
            return;

        DDNA.Instance.RecordEvent(
            "missionCompleted",
            new EventBuilder()
                .AddParam("isTutorial", System.Convert.ToBoolean("FALSE"))
                .AddParam("missionID", level.displayText)
                .AddParam("missionName", level.levelName)
                .AddParam("starsAchieved", level.GetCollectedStarsCount())
                .AddParam("userLevel", MyMaze.Instance.PacksPassedCount)
                .AddParam("userScore", MyMaze.Instance.StarsRecived)
        );
        Upload();
    }

    /// <summary>
    /// Запишем стату о провале уровня
    /// </summary>
    /// <param name="level"></param>
    void RecordMissionFailed(Level level)
    {
        if (!IsDDNAInitalized)
            return;

        DDNA.Instance.RecordEvent(
            "missionFailed",
            new EventBuilder()
                .AddParam("isTutorial", System.Convert.ToBoolean("FALSE"))
                .AddParam("missionID", level.displayText)
                .AddParam("missionName", level.levelName)
                .AddParam("userLevel", MyMaze.Instance.PacksPassedCount)
                .AddParam("userScore", MyMaze.Instance.StarsRecived)
        );
        Upload();
    }

    /// <summary>
    /// Запишем стату о запуске игрового уровня
    /// </summary>
    /// <param name="level"></param>
    void RecordMissionStarted(Level level)
    {
        if (!IsDDNAInitalized)
            return;

        DDNA.Instance.RecordEvent(
            "missionStarted",
            new EventBuilder()
                .AddParam("isTutorial", System.Convert.ToBoolean("FALSE"))
                .AddParam("missionID", level.displayText)
                .AddParam("missionName", level.levelName)
                .AddParam("userLevel", MyMaze.Instance.PacksPassedCount)
                .AddParam("userScore", MyMaze.Instance.StarsRecived)
        );
        Upload();   
    }

    /// <summary>
    /// Запишем стату о том что было создано новое уведомление
    /// </summary>
    /// <param name="id"></param>
    /// <param name="launchTime"></param>
    /// <param name="name"></param>
    void RecordNotificationOpened(int id, System.DateTime launchTime, string name)
    {
        if (!IsDDNAInitalized)
            return;

        DDNA.Instance.RecordEvent(
            "notificationOpened",
            new EventBuilder()
                .AddParam("notificationId", id)
                .AddParam("notificationLaunch", true)
                .AddParam("notificationName", name)
        );
        Upload();
    }

    /// <summary>
    /// Запишем попытку купить что-то в магазине
    /// </summary>
    /// <param name="type"></param>
    void RecordThumbPurchaseAttempt(ProductTypes type)
    {
        if (!IsDDNAInitalized)
            return;

        EventBuilder thumbPurchaseAttempt = new EventBuilder();
        thumbPurchaseAttempt.AddParam("thumbAdvID", type.ToString("g"));
        thumbPurchaseAttempt.AddParam("thumbIAPid", MyMaze.Instance.InApps.GetProduct<InApps.MarketMatching>(type).productId);
        thumbPurchaseAttempt.AddParam("thumbUserDaysInGame", MyMaze.Instance.DaysInGame);
        thumbPurchaseAttempt.AddParam("transactionName", type.ToString("g"));
        thumbPurchaseAttempt.AddParam("transactionType", TransactionTypes.PURCHASE.ToString("g"));
        thumbPurchaseAttempt.AddParam("thumbUserCurrentCashBalance", 0);
        DDNA.Instance.RecordEvent("thumbPurchaseAttempt", thumbPurchaseAttempt);
        Upload();
    }

    /// <summary>
    /// Запишем покупку
    /// </summary>
    /// <param name="type"></param>
    void RecordTransaction(ProductTypes type)
    {
        if (!IsDDNAInitalized)
            return;

        Debug.Log("RecordTransaction " + type.ToString("g"));
        EventBuilder thumbPurchaseAttempt = new EventBuilder();
        thumbPurchaseAttempt.AddParam("thumbAdvID", type.ToString("g"));
        thumbPurchaseAttempt.AddParam("thumbUserDaysInGame", MyMaze.Instance.DaysInGame);
        thumbPurchaseAttempt.AddParam("transactionName", type.ToString("g"));
        thumbPurchaseAttempt.AddParam("transactionType", TransactionTypes.PURCHASE.ToString("g"));
        thumbPurchaseAttempt.AddParam("productsReceived", new ProductBuilder()
                                .AddItem(type.ToString("g"), type.ToString("g"), 1));     
        thumbPurchaseAttempt.AddParam("productsSpent", new ProductBuilder()
                                .AddRealCurrency("USD", MyMaze.Instance.InApps.GetProduct<InApps.MarketMatching>(type).cost));
        thumbPurchaseAttempt.AddParam("afAttrAdgroupID", "none");
        thumbPurchaseAttempt.AddParam("afAttrAdgroupName", "none");
        thumbPurchaseAttempt.AddParam("afAttrAdsetID", "none");
        thumbPurchaseAttempt.AddParam("afAttrAdsetName", "none");
        thumbPurchaseAttempt.AddParam("afAttrCampaignID", "none");
        thumbPurchaseAttempt.AddParam("afAttrIsFacebook", false);
        thumbPurchaseAttempt.AddParam("afAttrMediaSource", "none");
        thumbPurchaseAttempt.AddParam("afAttrStatus", "ORGANIC");

        DDNA.Instance.RecordEvent("transaction", thumbPurchaseAttempt);
        Upload();
    }

    void Upload()
    {
        if (!uploadRoutine)
        {
            uploadRoutine = true;
            StartCoroutine(UploadNumerator());
        }
    }

    IEnumerator UploadNumerator()
    {
        yield return new WaitForSeconds(uploadDelay);
        if (!DDNA.Instance.IsUploading)
        {
            DDNA.Instance.Upload();
            DDNA.Instance.OnUploadEnd += OnUploadEnd;
        }
        else
        {
            StartCoroutine(UploadNumerator());
        }
    }

    void OnUploadEnd()
    {
        DDNA.Instance.OnUploadEnd -= OnUploadEnd;
        uploadRoutine = false;
    }
}
