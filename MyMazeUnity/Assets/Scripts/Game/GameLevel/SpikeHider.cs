using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class SpikeHider : GameLevelObject {

    public float hideTime = 0.75f;
    public Spike[] spikes;

    public override void Start()
    {
        base.Start();
        if (spikes.Length == 0)
            spikes = GameObject.FindObjectsOfType<Spike>();
        if (animator.enabled)
            animator.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (CheckForPlayer(coll.gameObject))
        {
            ShowSpikes();
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
        {
            HideSpikes();
        }
    }

    /// <summary>
    /// Прячем шипы. Альфу в ноль
    /// </summary>
    void HideSpikes()
    {
        foreach (Spike spike in spikes)
            spike.FadeOut(hideTime);
    }

    /// <summary>
    /// Показываем шипы, альфу в еденицу
    /// </summary>
    void ShowSpikes()
    {
        foreach (Spike spike in spikes)
            spike.FadeIn(hideTime);
    }

    public override void Restart()
    {
        base.Restart();
    }
}
