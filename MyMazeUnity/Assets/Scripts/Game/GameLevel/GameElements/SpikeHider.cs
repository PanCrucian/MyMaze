using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class SpikeHider : GameLevelObject {

    public float hideTime = 0.75f;
    public bool inversed = true;
    public Spike[] spikes;

    public override void Start()
    {
        base.Start();
        if (spikes.Length == 0)
            spikes = GameObject.FindObjectsOfType<Spike>();
        if (animator.enabled)
            animator.enabled = false;
    }

    /// <summary>
    /// Вошли в пряталку
    /// </summary>
    /// <param name="coll">Вошедший объект</param>
    void OnTriggerEnter2D(Collider2D coll) {
        if (CheckForPlayer(coll.gameObject))
        {
            if (!inversed)
                ShowSpikes();
            else
                HideSpikes();
        }
    }

    /// <summary>
    /// Вышли из пряталки
    /// </summary>
    /// <param name="coll">Вышедший объект</param>
    void OnTriggerExit2D(Collider2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
        {
            if (!inversed)
                HideSpikes();
            else
                ShowSpikes();
        }
    }

    /// <summary>
    /// Прячем шипы. Альфу в ноль
    /// </summary>
    void HideSpikes()
    {
        foreach (Spike spike in spikes)
            spike.FadeOut(hideTime);
        GetComponent<SoundsPlayer>().PlayOneShootSound(SoundNames.SpikeHide);
    }

    /// <summary>
    /// Показываем шипы, альфу в еденицу
    /// </summary>
    void ShowSpikes()
    {
        foreach (Spike spike in spikes)
            spike.FadeIn(hideTime);
        GetComponent<SoundsPlayer>().PlayOneShootSound(SoundNames.SpikeShow);
    }

    public override void Restart()
    {
        base.Restart();
    }
}
