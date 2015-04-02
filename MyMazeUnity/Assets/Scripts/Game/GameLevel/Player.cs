using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(GridDraggableObject))]
public class Player : MonoBehaviour, IRecordingElement, IPauseable {
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
    }
	public static Player Instance {
        get
        {
            return _instance;
        }
    }

    private static Player _instance;
    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private GridDraggableObject draggable;
    private Directions moveDirection;
    private int _movesCount;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        SetupStartVars();
        Spawn();
        GameLevel.Instance.OnPlayerMoveRequest += OnMoveRequest;
    }

    /// <summary>
    /// Делаем ссылки и умолчания для переменных
    /// </summary>
    void SetupStartVars()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        draggable = GetComponent<GridDraggableObject>();
    }

    /// <summary>
    /// Игрок появляется
    /// </summary>
    void Spawn()
    {
        animator.SetTrigger("FadeIn");
        _movesCount = 0;
    }

    void Update()
    {
        if (GameLevel.Instance.state == GameLevelStates.Pause)
            TogglePause(true);
        else
            TogglePause(false);
    }


    /// <summary>
    /// Останавливает движение игрока
    /// </summary>
    public void StopMoving()
    {
        state = PlayerStates.Idle;
        draggable.UpdatePositionVars();
        draggable.UpdatePosition();
        rigidbody2d.velocity = Vector2.zero;
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
    /// Если игра встала на паузу
    /// </summary>
    /// <param name="pause">Состояние bool паузы</param>
    public void TogglePause(bool pause)
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
}
