using UnityEngine;
#if UNITY_IPHONE
using UnityEngine.iOS;
#endif
using System.Collections;

public class Notifications : MonoBehaviour {

    void Start()
    {        
#if UNITY_IPHONE
        IOSNotificationController.Instance.RequestNotificationPermissions();
        if (IOSNotificationController.Instance.LaunchNotification != null)
        {
            ISN_LocalNotification notification = IOSNotificationController.Instance.LaunchNotification;

            Debug.Log("Запустили приложение из уведомления \n" + 
                      "Messgae: " + notification.Message + "\n" + 
                      "Notification Data: " + notification.Data);
        }
#endif
    }

    /// <summary>
    /// Регистрируем уведомление когда все жизни отрегенирируют
    /// </summary>
    void ScheduleRestoreLivesNotification()
    {
        if (MyMaze.Instance.Life.Units == MyMaze.Instance.Life.MaxUnits)
            return;

#if UNITY_IPHONE
        ISN_LocalNotification notification = new ISN_LocalNotification(
            System.DateTime.Now.AddSeconds(Mathf.Abs(MyMaze.Instance.Life.GetLastBlock().regenerationTime - Timers.Instance.UnixTimestamp)), 
            MyMaze.Instance.Localization.GetLocalized("gobacktogame"), 
            true);
        notification.SetData("lives_restored");
        notification.SetBadgesNumber(1);
        notification.Schedule();
        Debug.Log("Регистрация уведомления о жизнях id: " + notification.Id.ToString());
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
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
            ScheduleRestoreLivesNotification();
        else
            CancelAllNotifications();
    }
}
