using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Portal : GameLevelObject
{
    public Portal outPortal;
    public bool AllowTeleporting
    {
        get
        {
            return _allowTeleporting; 
        }
        set
        {
            _allowTeleporting = value;
        }
    }

    private bool _allowTeleporting = true;
    
    /// <summary>
    /// Если в телепорт что-то попало
    /// </summary>
    /// <param name="coll"></param>
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!AllowTeleporting || outPortal == null)
            return;

        if (CheckForPlayer(coll.gameObject))
            Player.Instance.Teleport(outPortal);

        //запретим телепортировать выходному порталу, чтобы наш объект не скакал туда-сюда
        outPortal.AllowTeleporting = false;
    }

    /// <summary>
    /// Если из телепорта что-то вышло
    /// </summary>
    /// <param name="coll"></param>
    void OnTriggerExit2D(Collider2D coll)
    {
        AllowTeleporting = true;
    }
}
