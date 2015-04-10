using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Gate : GameLevelObject {

    private GateStates state;
    private BoxCollider2D boxCollider;

    public override void Start()
    {
        base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Двери закрылись (Закончилась анимация)
    /// </summary>
    public void OnClose()
    {
        state = GateStates.Closed;
        boxCollider.isTrigger = false;        
    }

    /// <summary>
    /// Закрыть двери
    /// </summary>
    void Close()
    {
        animator.SetTrigger("Close");
    }

    /// <summary>
    /// Двери открылись
    /// </summary>
    public void OnOpen()
    {
        state = GateStates.Opened;
        boxCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
            if (state == GateStates.Opened)
                Close();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
            Player.Instance.StopMoving();
    }

    /// <summary>
    /// Игра перезапустилась
    /// </summary>
    public override void Restart()
    {
        base.Restart();
        animator.SetTrigger("Reset");
        OnOpen();
    }
}
