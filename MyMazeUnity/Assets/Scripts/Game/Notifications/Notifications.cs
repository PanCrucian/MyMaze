using UnionAssets.FLE;
using UnityEngine;
#if UNITY_IPHONE
using UnityEngine.iOS;
#endif
using System.Collections;

public class Notifications : MonoBehaviour {

    void Start()
    {
#if UNITY_IPHONE
        IOSNotificationController.instance.RequestNotificationPermissions();
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
        int id = IOSNotificationController.instance.ScheduleNotification(5, MyMaze.Instance.Localization.GetLocalized("gobacktogame"), true);
        Debug.Log("Регистрация уведомления о жизнях id: " + id.ToString());
#endif
    }

    /// <summary>
    /// Отменяем все уведомления
    /// </summary>
    void CancelAllNotifications()
    {
#if UNITY_IPHONE
        IOSNotificationController.instance.CancelAllLocalNotifications();
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
