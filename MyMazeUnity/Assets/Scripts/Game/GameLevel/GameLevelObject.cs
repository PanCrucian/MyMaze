using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(GridDraggableObject))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class GameLevelObject : MonoBehaviour, IRecordingElement, IPauseable, IRestartable {

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public GridDraggableObject draggable;

    private Dictionary<int, PositionRecordData> moveHistory = new Dictionary<int, PositionRecordData>();

    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        draggable = GetComponent<GridDraggableObject>();
        GameLevel.Instance.OnRestart += Restart;
        Player.Instance.OnMoveEnd += Record;
        GameLevel.Instance.OnReturnToMove += ReturnToMove;

        Record(0);
    }

    public virtual void Update()
    {
        if (GameLevel.Instance.state == GameLevelStates.Pause)
            TogglePause(true);
        else
            TogglePause(false);
    }
        
    /// <summary>
    /// Если игра встала на паузу
    /// </summary>
    /// <param name="pause">Состояние bool паузы</param>
    public virtual void TogglePause(bool pause)
    {
        if (pause) {
            animator.speed = 0f;
        }
        else 
        { 
            animator.speed = 1f;
        }
    }

    /// <summary>
    /// Уровень перезапустился
    /// </summary>
    public virtual void Restart()
    {
        draggable.SetPositionVars(draggable.StartPosition.Clone());
        draggable.UpdatePosition();
        RecordsReset();
        Record(0);
    }

    /// <summary>
    /// Проверяет тэг объекта на Player
    /// </summary>
    /// <param name="obj">Игровой объект</param>
    /// <returns></returns>
    public bool CheckForPlayer(GameObject obj)
    {
        if (obj.tag.Equals("Player"))
            return true;

        return false;
    }

    /// <summary>
    /// Записывает состояние объекта на ход
    /// </summary>
    /// <param name="move">Номер хода</param>
    public virtual void Record(int move)
    {
        if (moveHistory.ContainsKey(move))
            moveHistory.Remove(move);

        PositionRecordData recordData = new PositionRecordData();
        recordData.position = draggable.position.Clone();
        moveHistory.Add(move, recordData);
    }

    /// <summary>
    /// Сбрасывает записи о состоянии объекта в ходах
    /// </summary>
    public virtual void RecordsReset()
    {
        moveHistory = new Dictionary<int, PositionRecordData>();
    }

    /// <summary>
    /// Возвращаемся в состояние на конкретный ход
    /// </summary>
    /// <param name="move">Ход в который хотим вернуться</param>
    public virtual void ReturnToMove(int move)
    {
        PositionRecordData recordData;
        if (moveHistory.TryGetValue(move, out recordData))
        {
            draggable.SetPositionVars(recordData.position.Clone());
            draggable.UpdatePosition();
        }
    }
}
