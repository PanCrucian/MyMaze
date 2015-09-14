using UnityEngine;
using System.Collections;

/// <summary>
/// Мягкое обновление
/// </summary>
public class GentleMonoBeh : MonoBehaviour {

    private int saveCPURate = 6; //пересчитывать тяжелую логику раз в 6 кадров
    private int frames;

    public virtual void Update()
    {

        if (frames % saveCPURate == 0)
            GentleUpdate();

        frames++;
    }

    /// <summary>
    /// Update экономящий ресурсы устройства
    /// </summary>
    public virtual void GentleUpdate()
    {
        
    }

    /// <summary>
    /// Установить частоту обновления GentleUpdate() чем больше тем экномнее
    /// </summary>
    /// <param name="rate"></param>
    public void SetGentleCPURate(int rate)
    {
        saveCPURate = rate;
    }
}
