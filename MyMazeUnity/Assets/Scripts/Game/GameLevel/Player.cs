using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Player : GameLevelObject
{
    public DesignSettings designSettings;
    public PlayerStates state;
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
        public float moveImpulsePower = 5f;
        public float stopMovingDelay = 0.075f;
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

    /// <summary>
    /// Игрок появляется
    /// </summary>
    void Spawn()
    {
        EnableControl();
        _movesCount = 0;
        allowControl = true;
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
    public void StopMoving()
    {
        if (state != PlayerStates.Move)
            return;

        state = PlayerStates.Idle;
        draggable.UpdatePositionVars();
        draggable.UpdatePosition();
        rigidbody2d.velocity = Vector2.zero;
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
    /// Дает импульс движения компоненте rigidbody2d
    /// </summary>
    /// <param name="direction">Направление движения</param>
    void MoveImpulse(Directions direction)
    {
        Vector2 impulseVector = Vector2.zero;
        switch (direction)
        {
            case Directions.Up: impulseVector.y = designSettings.moveImpulsePower; break;
            case Directions.Right: impulseVector.x = designSettings.moveImpulsePower; break;
            case Directions.Down: impulseVector.y = -1f * designSettings.moveImpulsePower; break;
            case Directions.Left: impulseVector.x = -1f * designSettings.moveImpulsePower; break;
            default: break;
        }
        rigidbody2d.AddRelativeForce(impulseVector, ForceMode2D.Impulse);

        state = PlayerStates.Move;
        _movesCount++;
        MyMaze.Instance.MovesCounter++;
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
}
