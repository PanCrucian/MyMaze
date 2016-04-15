using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class Box : GameLevelObject
{
    /// <summary>
    /// Игрок входит
    /// </summary>
    public Deligates.SimpleEvent OnPlayerEnter;

    /// <summary>
    /// Цвет
    /// </summary>
    public BoxColors color;

    /// <summary>
    /// Колайдер
    /// </summary>
    [HideInInspector]
    public BoxCollider2D boxCollider;

    /// <summary>
    /// История состояний
    /// </summary>
    private Dictionary<int, BoxRecordData> boxHistory = new Dictionary<int, BoxRecordData>();

    /// <summary>
    /// Старт
    /// </summary>
	public override void Start () {
        boxCollider = GetComponent<BoxCollider2D>();
        base.Start();
	}

    /// <summary>
    /// Рестарт уровня
    /// </summary>
    public override void Restart()
    {
        ToDefaultState();
        base.Restart();
    }

    /// <summary>
    /// В обычное состояние
    /// </summary>
    public void ToDefaultState()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
            animator.SetTrigger("Reset");
        boxCollider.enabled = true;
    }

    /// <summary>
    /// Исчезаем
    /// </summary>
    public void FadeOut()
    {
        boxCollider.enabled = false;
        switch (color)
        {
            case BoxColors.White: 
                animator.SetTrigger("FadeWhite");
                break;
            case BoxColors.Yellow: 
                animator.SetTrigger("FadeYellow");
                break;
        }

        GetComponent<SoundsPlayer>().PlayOneShootSound();
    }

    /// <summary>
    /// Если что-то врезалось в кубик
    /// </summary>
    /// <param name="coll"></param>
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
        {
            Player.Instance.StopMoving();
            if (OnPlayerEnter != null)
                OnPlayerEnter();
            FadeOut();
        }
    }

    /// <summary>
    /// Записывает состояние ящика на ход
    /// </summary>
    /// <param name="move">Номер хода</param>
    public override void Record(int move)
    {
        if (boxHistory.ContainsKey(move))
            boxHistory.Remove(move);
        BoxRecordData recordData = new BoxRecordData();
        recordData.colliderEnabled = boxCollider.enabled;
        boxHistory.Add(move, recordData);
        base.Record(move);
    }

    /// <summary>
    /// Сбрасывает записи о состоянии ящика
    /// </summary>
    public override void RecordsReset()
    {
        boxHistory = new Dictionary<int, BoxRecordData>();
        base.RecordsReset();
    }

    /// <summary>
    /// Возвращаемся в состояние на конкретный ход
    /// </summary>
    /// <param name="move">Ход в который хотим вернуться</param>
    public override void ReturnToMove(int move)
    {
        BoxRecordData recordData;
        if (boxHistory.TryGetValue(move, out recordData))
        {
            if (recordData.colliderEnabled)
                ToDefaultState();
        }
        base.ReturnToMove(move);
    }
}
