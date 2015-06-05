using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeleportUI : MonoBehaviour {

    /// <summary>
    /// Ссылка на заполняемую картинку
    /// </summary>
    public Image imageFill;

    /// <summary>
    /// По причине "Гениального" геймдизайна нужно делить прогресс востановления на сегменты
    /// </summary>
    public int maxSegments = 12;

    private TeleportBooster teleport;
    private float[] segments;

    void Start()
    {
        teleport = MyMaze.Instance.TeleportBooster;
        if (teleport.IsClosed) // && teleport.avaliableAtLevel != MyMaze.Instance.LastSelectedLevel для тестов
        {
            gameObject.SetActive(false);
            return;
        }
        segments = new float[maxSegments];
        float segmentsInterval = 1f / (float)maxSegments;
        for (int i = 0; i < maxSegments; i++)
            segments[i] = segmentsInterval * (float) (i+1);
    }

    void Update()
    {
        float progress = (float)Mathf.Abs(Timers.Instance.UnixTimestamp - teleport.LastUsingTime) / (float) teleport.usingInterval;
        for (int i = 0; i < maxSegments; i++)
            if (progress >= segments[i])
                imageFill.fillAmount = segments[i];
        if(progress < segments[0])
            imageFill.fillAmount = 0f;
    }

    public void InitTeleportAction()
    {
        if(teleport.IsAvaliable())
            Player.Instance.PrepareForTeleport();
    }
}
