using UnityEngine;
using System.Collections;
using DeltaDNA;

public class MyDDNA : MonoBehaviour {

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
        DDNA.Instance.RecordEvent(
            "adsFreqSet",
            new EventBuilder()
                .AddParam("adsFrequency", MyMaze.Instance.Ads.levelFrequency.ToString())
                .AddParam("platform", Application.platform.ToString("g"))
        );
    }

    /// <summary>
    /// Запишем стату о том что открылся новый пак
    /// </summary>
    /// <param name="pack"></param>
    void RecordLevelUp(Pack pack)
    {
        DDNA.Instance.RecordEvent(
            "levelUp",
            new EventBuilder()
                .AddParam("levelUpName", pack.packName)
                .AddParam("platform", Application.platform.ToString("g"))
        );
    }

    /// <summary>
    /// Запишем стату о прохождении уровня
    /// </summary>
    /// <param name="level"></param>
    void RecordMissionCompleted(Level level)
    {
        DDNA.Instance.RecordEvent(
            "missionCompleted", 
            new EventBuilder()
                .AddParam("missionID", level.displayText)
                .AddParam("missionName", level.levelName)
                .AddParam("platform", Application.platform.ToString("g"))
        );
    }

    /// <summary>
    /// Запишем стату о провале уровня
    /// </summary>
    /// <param name="level"></param>
    void RecordMissionFailed(Level level)
    {
        DDNA.Instance.RecordEvent(
            "missionFailed",
            new EventBuilder()
                .AddParam("missionID", level.displayText)
                .AddParam("missionName", level.levelName)
                .AddParam("platform", Application.platform.ToString("g"))
        );
    }

    /// <summary>
    /// Запишем стату о запуске игрового уровня
    /// </summary>
    /// <param name="level"></param>
    void RecordMissionStarted(Level level)
    {
        DDNA.Instance.RecordEvent(
            "missionStarted",
            new EventBuilder()
                .AddParam("missionID", level.displayText)
                .AddParam("missionName", level.levelName)
                .AddParam("platform", Application.platform.ToString("g"))
        );
    }

    /// <summary>
    /// Запишем стату о том что было создано новое уведомление
    /// </summary>
    /// <param name="id"></param>
    /// <param name="launchTime"></param>
    /// <param name="name"></param>
    void RecordNotificationOpened(int id, System.DateTime launchTime, string name)
    {
        DDNA.Instance.RecordEvent(
            "notificationOpened",
            new EventBuilder()
                .AddParam("notificationId", id.ToString())
                .AddParam("notificationLaunch", launchTime.ToString())
                .AddParam("notificationName", name)
                .AddParam("platform", Application.platform.ToString("g"))
        );
    }

    /// <summary>
    /// Запишем попытку купить что-то в магазине
    /// </summary>
    /// <param name="type"></param>
    void RecordThumbPurchaseAttempt(ProductTypes type)
    {
        EventBuilder thumbPurchaseAttempt = new EventBuilder();
        thumbPurchaseAttempt.AddParam("platform", Application.platform.ToString("g"));
        thumbPurchaseAttempt.AddParam("thumbAdvID", type.ToString("g"));
#if UNITY_IPHONE
        thumbPurchaseAttempt.AddParam("thumbIAPid", MyMaze.Instance.InApps.GetProduct<InApps.AppStoreMatching>(type).productId);
#elif UNITY_ANDROID
        thumbPurchaseAttempt.AddParam("thumbIAPid", "Not setuped yet");
#endif
        thumbPurchaseAttempt.AddParam("thumbUserDaysInGame", MyMaze.Instance.DaysInGame.ToString());
        thumbPurchaseAttempt.AddParam("transactionName", type.ToString("g"));
        thumbPurchaseAttempt.AddParam("transactionType", type.ToString("g"));

        DDNA.Instance.RecordEvent("thumbPurchaseAttempt", thumbPurchaseAttempt);
    }

    /// <summary>
    /// Запишем покупку
    /// </summary>
    /// <param name="type"></param>
    void RecordTransaction(ProductTypes type)
    {
        EventBuilder thumbPurchaseAttempt = new EventBuilder();
        thumbPurchaseAttempt.AddParam("platform", Application.platform.ToString("g"));
        thumbPurchaseAttempt.AddParam("thumbAdvID", type.ToString("g"));
        thumbPurchaseAttempt.AddParam("thumbUserDaysInGame", MyMaze.Instance.DaysInGame.ToString());
        thumbPurchaseAttempt.AddParam("transactionName", type.ToString("g"));
        thumbPurchaseAttempt.AddParam("transactionType", type.ToString("g"));

        DDNA.Instance.RecordEvent("transaction", thumbPurchaseAttempt);
    }
}
