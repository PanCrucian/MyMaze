using UnityEngine;
using System.Collections;

public class MovingWall : GameLevelObject {

    public ButtonLevel button;
    public GridObject.Position toPosition;
    public float moveTime = 1.75f;

    private GameObject wallBorder;
    private float movedTime = 0f;

    private Vector3 moveToPosition;
    private Vector3 moveStartPosition;
    private Vector3 oldLocalPosition;

    public override void Start()
    {
        base.Start();
        animator.enabled = false;
        wallBorder = GetComponentInChildren<WallBorder>().gameObject;
        button.OnPress += OnPress;
        button.OnRelease += OnRelease;
        ResetLocalVars();
    }

    void ResetLocalVars()
    {
        oldLocalPosition = transform.localPosition;
        moveStartPosition = oldLocalPosition;
        moveToPosition = moveStartPosition;
        movedTime = 0f;
    }

    public override void Update()
    {
        base.Update();
        //двигаем стенку
        transform.localPosition = Vector3.Lerp(moveStartPosition, moveToPosition, movedTime / moveTime);
        movedTime += Time.deltaTime;
        if (movedTime > moveTime)
            movedTime = moveTime;

        //Включим или выключим бордюр
        if (transform.localPosition != oldLocalPosition)
        {
            if (!wallBorder.activeSelf)
                wallBorder.SetActive(true);
            oldLocalPosition = transform.localPosition;
        }
        else
            if (wallBorder.activeSelf)
                wallBorder.SetActive(false);
    }

    /// <summary>
    /// Едем к позиции toPosition
    /// </summary>
    /// <param name="button"></param>
    void OnPress(ButtonLevel button)
    {
        moveToPosition = draggable.GetWorldPosition(toPosition);
        moveStartPosition = transform.localPosition;
        movedTime = 0f;
    }

    /// <summary>
    /// Уезжаем в стартовую позицию
    /// </summary>
    /// <param name="button"></param>
    void OnRelease(ButtonLevel button)
    {
        moveToPosition = draggable.GetWorldPosition(draggable.StartPosition);
        moveStartPosition = transform.localPosition;
        movedTime = 0f;
    }

    /// <summary>
    /// Если что-то врезалось в стену
    /// </summary>
    /// <param name="coll"></param>
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
            Player.Instance.StopMoving();
    }

    public override void Restart()
    {
        base.Restart();
        ResetLocalVars();
    }
}
