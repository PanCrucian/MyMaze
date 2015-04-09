using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GridDraggableObject))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class GameLevelObject : MonoBehaviour, IRecordingElement, IPauseable, IRestartable {

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public GridDraggableObject draggable;

    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        draggable = GetComponent<GridDraggableObject>();
        GameLevel.Instance.OnRestart += Restart;
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
        if (pause)
            animator.speed = 0f;
        else
            animator.speed = 1f;
    }

    /// <summary>
    /// Уровень перезапустился
    /// </summary>
    public virtual void Restart()
    {
        draggable.SetPositionVars(draggable.StartPosition);
        draggable.UpdatePosition();
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
}
