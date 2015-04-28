﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Player : GameLevelObject
{
    public Deligates.IntegerEvent OnMoveEnd;
    public DesignSettings designSettings;
    public PlayerControlTypes controlType;
    public PlayerStates state;
    /// <summary>
    /// Можно ли управлять, контролируется из анимации
    /// </summary>
    public bool allowControl = true;

    public int MovesCount
    {
        get {
            return _movesCount;
        }
    }

    [System.Serializable]
    public class DesignSettings
    {
        public float moveImpulsePower = 4.75f;
        public float stopMovingDelay = 0.055f;
    }
	public static Player Instance {
        get
        {
            return _instance;
        }
    }

    private static Player _instance;
    private Rigidbody2D rigidbody2d;
    private Directions moveDirection;
    private int _movesCount;

    /// <summary>
    /// Прыгаем?
    /// </summary>
    private bool isJump;

    /// <summary>
    /// Позиция в которой должна начать играть анимация приземления
    /// </summary>
    private GridObject.Position jumpEndAnimationPosition;

    /// <summary>
    /// Позиция в которой игрок должен остановиться после прыжка
    /// </summary>
    private GridObject.Position jumpEndPosition;

    void Awake()
    {
        _instance = this;
    }

    public override void Start()
    {
        base.Start();
        rigidbody2d = GetComponent<Rigidbody2D>();
        Spawn();
        GameLevel.Instance.OnPlayerMoveRequest += OnMoveRequest;
    }

    public override void Update()
    {
        base.Update();

        if (state == PlayerStates.Move)
        {
            Vector2 impulseVector = Vector2.zero;
            switch (moveDirection)
            {
                case Directions.Up: impulseVector.y = designSettings.moveImpulsePower * Time.deltaTime; break;
                case Directions.Right: impulseVector.x = designSettings.moveImpulsePower * Time.deltaTime; break;
                case Directions.Down: impulseVector.y = -1f * designSettings.moveImpulsePower * Time.deltaTime; break;
                case Directions.Left: impulseVector.x = -1f * designSettings.moveImpulsePower * Time.deltaTime; break;
                default: break;
            }
            if (controlType == PlayerControlTypes.TransformChange)
                transform.localPosition = new Vector3(
                    transform.localPosition.x + impulseVector.x,
                    transform.localPosition.y + impulseVector.y,
                    transform.localPosition.z
                    );
            if(controlType == PlayerControlTypes.RigidbodyImpulse)
                rigidbody2d.velocity = new Vector3(impulseVector.x, impulseVector.y, 0);
        }

        //Обрабатываем прыжок
        if (!isJump)
            return;
        if (draggable.position.Equals(jumpEndAnimationPosition))
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerJumpOut"))
                animator.SetTrigger("JumpOut");
        }
        if (draggable.position.Equals(jumpEndPosition))
        {
            StopMovingDelay();
            isJump = false;
        }
    }

    /// <summary>
    /// Игрок появляется
    /// </summary>
    void Spawn()
    {
        EnableControl();
        _movesCount = 0;
        isJump = false;
        moveDirection = Directions.Unknown;
        state = PlayerStates.Idle;
    }

    /// <summary>
    /// Разрешает управлять игроком + анимация появления
    /// </summary>
    public void EnableControl()
    {
        animator.SetTrigger("FadeIn");
    }

    /// <summary>
    /// Запрещает управлять игроком + анимация исчезания
    /// </summary>
    public void DisableControl()
    {
        animator.SetTrigger("FadeOut");
    }
    
    /// <summary>
    /// Останавливает движение игрока
    /// </summary>
    /// <param name="isEffector">Это вызвал игровой объект ускоритель?</param>
    public void StopMoving(bool isEffector)
    {
        if (!isEffector)
            if (state != PlayerStates.Move)
                return;

        state = PlayerStates.Idle;
        draggable.UpdatePositionVars();
        draggable.UpdatePosition();
        rigidbody2d.velocity = Vector2.zero;

        if (!isEffector)
            if (OnMoveEnd != null)
                OnMoveEnd(MovesCount);
    }

    public void StopMoving()
    {
        StopMoving(false);
    }

    /// <summary>
    /// Остановка движения игрока после временной задержки
    /// </summary>
    public void StopMovingDelay()
    {
        StartCoroutine(StopMovingDelayEnumerator());
    }

    IEnumerator StopMovingDelayEnumerator()
    {
        yield return new WaitForSeconds(designSettings.stopMovingDelay);
        StopMoving();
    }

    /// <summary>
    /// Дает импульс движения
    /// </summary>
    /// <param name="direction">Направление движения</param>
    /// <param name="isEffector">Это вызвал игровой объект ускоритель?</param>
    public void MoveImpulse(Directions direction, bool isEffector)
    {
        moveDirection = direction;
        Vector2 impulseVector = Vector2.zero;
        switch (moveDirection)
        {
            case Directions.Up: impulseVector.y = designSettings.moveImpulsePower; break;
            case Directions.Right: impulseVector.x = designSettings.moveImpulsePower; break;
            case Directions.Down: impulseVector.y = -1f * designSettings.moveImpulsePower; break;
            case Directions.Left: impulseVector.x = -1f * designSettings.moveImpulsePower; break;
        }
        //if (controlType == PlayerControlTypes.RigidbodyImpulse)
            //rigidbody2d.AddRelativeForce(impulseVector, ForceMode2D.Impulse);
        state = PlayerStates.Move;
        if (!isEffector)
        {
            _movesCount++;
            MyMaze.Instance.MovesCounter++;
        }
    }

    public void MoveImpulse(Directions direction)
    {
        MoveImpulse(direction, false);
    }

    /// <summary>
    /// Просьба заставить двигаться игрока
    /// </summary>
    /// <param name="direction"></param>
    void OnMoveRequest(Directions direction)
    {
        if (!allowControl)
        {
            Debug.Log("Управление игроком отключено, не могу выполнить запрос на движение");
            return;
        }
        if (state == PlayerStates.Move)
            return;
        MoveImpulse(direction);
    }

    /// <summary>
    /// Телпортация игрока
    /// </summary>
    /// <param name="portal">Портал в который нужно телепортироваться</param>
    public void Teleport(Portal portal)
    {
        GridObject.Position portalPosition = portal.GetComponent<GridObject>().position;
        GridObject.Position newPosition = new GridObject.Position()
        {
            xCell = portalPosition.xCell,
            yRow = portalPosition.yRow
        };
        draggable.SetPositionVars(newPosition);
        draggable.UpdatePosition();
    }

    /// <summary>
    /// Делаем прыжок
    /// </summary>
    /// <param name="jumpCellDistance">На сколько клеток прыгать</param>
    public void Jump(Trampoline trampoline)
    {
        int jumpCellDistance = trampoline.jumpCellDistance;        
        isJump = true;
        animator.SetTrigger("JumpIn");
        jumpEndPosition = new GridObject.Position()
        {
            xCell = trampoline.draggable.position.xCell,
            yRow = trampoline.draggable.position.yRow
        };
        jumpEndAnimationPosition = jumpEndPosition.Clone();

        if (state != PlayerStates.Move)
        {
            MoveImpulse(moveDirection);
        }

        switch (moveDirection)
        {
            case Directions.Up:
                jumpEndAnimationPosition.yRow += jumpCellDistance - 1;
                jumpEndPosition.yRow += jumpCellDistance; 
                break;
            case Directions.Right:
                jumpEndAnimationPosition.xCell += jumpCellDistance - 1;
                jumpEndPosition.xCell += jumpCellDistance; 
                break;
            case Directions.Down:
                jumpEndAnimationPosition.yRow -= jumpCellDistance - 1;
                jumpEndPosition.yRow -= jumpCellDistance; 
                break;
            case Directions.Left:
                jumpEndAnimationPosition.xCell -= jumpCellDistance - 1;
                jumpEndPosition.xCell -= jumpCellDistance; 
                break;
            default: break;
        }
        //Если прыжок короткий, то конец приземления и будет анимацией
        if (jumpCellDistance < 2)
            jumpEndAnimationPosition = jumpEndPosition.Clone();
    }

    /// <summary>
    /// Игрок умер
    /// </summary>
    public void Die()
    {
        StopMoving();
        state = PlayerStates.Die;
        animator.SetTrigger("Death");
    }

    /// <summary>
    /// Анимация смерти закончилась
    /// </summary>
    public void OnDie()
    {
        GameLevel.Instance.OnRestartRequest();
    }

    /// <summary>
    /// Если игра встала на паузу
    /// </summary>
    /// <param name="pause">Состояние bool паузы</param>
    public override void TogglePause(bool pause)
    {
        if (pause)
        {
            animator.speed = 0f;
            rigidbody2d.simulated = false;
        }
        else
        {
            animator.speed = 1f;
            rigidbody2d.simulated = true;
        }
    }

    /// <summary>
    /// Уровень перезапустился
    /// </summary>
    public override void Restart()
    {
        StopMoving();
        base.Restart();
        Spawn();
    }

    public override void ReturnToMove(int move)
    {
        base.ReturnToMove(move);
        _movesCount = move;
    }
}
