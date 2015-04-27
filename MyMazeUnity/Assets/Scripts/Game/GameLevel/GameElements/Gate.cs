using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class Gate : GameLevelObject {

    private GateStates state;
    private BoxCollider2D boxCollider;

    private Dictionary<int, GateRecordData> gateHistory = new Dictionary<int, GateRecordData>();

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
        ToDefaultState();
        base.Restart();
    }

    void ToDefaultState()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
            animator.SetTrigger("Reset");
        OnOpen();
    }
    
    /// <summary>
    /// Записывает состояние ворот на ход
    /// </summary>
    /// <param name="move">Номер хода</param>
    public override void Record(int move)
    {
        if (gateHistory.ContainsKey(move))
            gateHistory.Remove(move);
        GateRecordData recordData = new GateRecordData();
        recordData.gateState = this.state;
        gateHistory.Add(move, recordData);
        base.Record(move);
    }

    /// <summary>
    /// Сбрасывает записи о состоянии ворот
    /// </summary>
    public override void RecordsReset()
    {
        gateHistory = new Dictionary<int, GateRecordData>();
        base.RecordsReset();
    }

    /// <summary>
    /// Возвращаемся в состояние на конкретный ход
    /// </summary>
    /// <param name="move">Ход в который хотим вернуться</param>
    public override void ReturnToMove(int move)
    {
        GateRecordData recordData;
        if (gateHistory.TryGetValue(move, out recordData))
        {
            switch (recordData.gateState)
            {
                case GateStates.Opened:
                    if (state == GateStates.Closed)
                        ToDefaultState();
                    break;
                case GateStates.Closed:
                    if (state == GateStates.Opened)
                        Close();
                    break;
            }
        }
        base.ReturnToMove(move);
    }
}
