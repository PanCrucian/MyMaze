using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LifeTimerUI : MonoBehaviour {

    private Text timerText;

    void Start()
    {
        timerText = GetComponent<Text>();
        MyMaze.Instance.Life.OnUseLife += OnUseLife;
    }

    void OnDestroy()
    {
        MyMaze.Instance.Life.OnUseLife -= OnUseLife;
    }
    
    void OnUseLife(int units)
    {
        ActiveTimer();
    }

    void Update()
    {

        if (MyMaze.Instance.Life.Units == MyMaze.Instance.Life.MaxUnits)
            timerText.gameObject.SetActive(false);
        else
            ActiveTimer();
    }

    void ActiveTimer()
    {
        timerText.gameObject.SetActive(true);
        timerText.text = GetTimerString();
    }

    /// <summary>
    /// Пересчитываем время до следующего восстановления жизни в человеческий вид
    /// </summary>
    /// <returns></returns>
    string GetTimerString()
    {
        int currentUnixTime = Timers.Instance.UnixTimestamp;
        int regenerationTime = MyMaze.Instance.Life.GetNextBlockRegenerationTime();

        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(Mathf.Abs(regenerationTime - currentUnixTime));

        if (regenerationTime > 0)
            return System.String.Format("{0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
        else
            return "0:00";
    }
}
