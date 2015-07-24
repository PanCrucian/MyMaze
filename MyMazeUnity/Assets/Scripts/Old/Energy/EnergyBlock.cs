using UnityEngine;
using System.Collections;

public class EnergyBlock {
    /// <summary>
    /// Доступен ли блок
    /// </summary>
    public bool IsAvaliable
    {
        get
        {
            return isAvaliable;
        }
        set
        {
            isAvaliable = value;
        }
    }

    public int index;

    private bool isAvaliable = true;

    /// <summary>
    /// Время после которого можно считать блок доступным
    /// </summary>
    public int regenerationTime = 0;

    /// <summary>
    /// Восстановить блок
    /// </summary>
    public void Restore()
    {
        isAvaliable = true;
        regenerationTime = 0;
    }
}
