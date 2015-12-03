using UnityEngine;
#if UNITY_IPHONE
using UnityEngine.iOS;
#endif
using System.Collections;

public class Notifications : MonoBehaviour {
    /// <summary>
    /// было сгенерировано новое уведомление
    /// </summary>
    public Deligates.NotificationEvent OnNewNotice;

    /// <summary>
    /// Уровень когда будет запрошено разрешение на нотификацию
    /// </summary>
    public Level permissionLevel;
    public string restoredLivesName = "lives_restored";

    void Start()
    {        
#if UNITY_IPHONE        
        if (IOSNotificationController.Instance.LaunchNotification != null)
        {
            ISN_LocalNotification notification = IOSNotificationController.Instance.LaunchNotification;

            Debug.Log("Запустили приложение из уведомления \n" + 
                      "Messgae: " + notification.Message + "\n" + 
                      "Notification Data: " + notification.Data);
        }
#endif
        permissionLevel.OnPassed += OnPermissionLevelPassed;
    }

    /// <summary>
    /// был пройден уровень после которого нужно запросить разрешение на нотификацию
    /// </summary>
    /// <param name="level"></param>
    void OnPermissionLevelPassed(Level level)
    {
        RequestPermission();
    }

    /// <summary>
    /// Запрашиваем разрешение на нотификации у пользователя
    /// </summary>
    public void RequestPermission()
    {
#if UNITY_IPHONE
        IOSNotificationController.Instance.RequestNotificationPermissions();
#endif
    }

    /// <summary>
    /// Регистрируем уведомление когда все жизни отрегенирируют
    /// </summary>
    void ScheduleRestoreLivesNotification()
    {
        if (MyMaze.Instance.Life.Units == MyMaze.Instance.Life.MaxUnits)
            return;
        CancelAllNotifications();
        System.DateTime launchTime = System.DateTime.Now.AddSeconds(
            Mathf.Abs(MyMaze.Instance.Life.GetLastBlock().regenerationTime - Timers.Instance.UnixTimestamp)
        );
#if UNITY_IPHONE        
        ISN_LocalNotification notification = new ISN_LocalNotification(
            launchTime, 
            MyMaze.Instance.Localization.GetLocalized("gobacktogame"), 
            true);
        notification.SetData(restoredLivesName);
        notification.SetBadgesNumber(1);
        notification.Schedule();
        if(OnNewNotice != null)
            OnNewNotice(notification.Id, launchTime, restoredLivesName);
        Debug.Log("Регистрация уведомления о жизнях id: " + notification.Id.ToString());
#endif
#if UNITY_ANDROID
        int lastNotificationId = AndroidNotificationManager.Instance.ScheduleLocalNotification(
            "My Maze", 
            MyMaze.Instance.Localization.GetLocalized("gobacktogame"), 
            Mathf.Abs(MyMaze.Instance.Life.GetLastBlock().regenerationTime - Timers.Instance.UnixTimestamp));
        if (OnNewNotice != null)
            OnNewNotice(lastNotificationId, launchTime, restoredLivesName);
        Debug.Log("Регистрация уведомления о жизнях id: " + lastNotificationId.ToString());
#endif
    }

    /// <summary>
    /// Отменяем все уведомления
    /// </summary>
    void CancelAllNotifications()
    {
#if UNITY_IPHONE
        IOSNotificationController.Instance.CancelAllLocalNotifications();
        IOSNativeUtility.SetApplicationBagesNumber(0);
#endif
#if UNITY_ANDROID
        AndroidNotificationManager.Instance.CancelAllLocalNotifications();
#endif
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
            ScheduleRestoreLivesNotification();
        else
            CancelAllNotifications();
    }
}
