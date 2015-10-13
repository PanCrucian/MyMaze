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
            if (teleport.unlimitedUseLevel != MyMaze.Instance.LastSelectedLevel)
            {
                gameObject.SetActive(false);
                return;
            }
        }
        segments = new float[maxSegments];
        float segmentsInterval = 1f / (float)maxSegments;
        for (int i = 0; i < maxSegments; i++)
            segments[i] = segmentsInterval * (float) (i+1);

        MyMaze.Instance.InApps.OnTeleportBuyed += OnTeleportBuyed;
    }

    void OnTeleportBuyed()
    {
        if(MyMaze.Instance.InApps.ConsumeTeleport())
            MyMaze.Instance.TeleportBooster.Reset();
    }

    void OnDestroy()
    {
        MyMaze.Instance.InApps.OnTeleportBuyed -= OnTeleportBuyed;
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
        if (teleport.IsAvaliable())
            Player.Instance.PrepareForTeleport();
        else
            MyMaze.Instance.InApps.BuyRequest(ProductTypes.BoosterTeleport);
    }
}
