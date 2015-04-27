using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxStrong : GameLevelObject {

    private Box boxWhite;
    private Box boxYellow;

    private Dictionary<int, BoxStrongRecordData> boxStrongHistory = new Dictionary<int, BoxStrongRecordData>();

    public override void Start()
    {
        base.Start();
        animator.enabled = false;
        FindBoxes();        
        boxYellow.OnPlayerEnter += OnPlayerEnter;
        StartCoroutine(RestartNumerator());
    }

    /// <summary>
    /// Врезался в желтый ящик
    /// </summary>
    void OnPlayerEnter()
    {
        boxWhite.boxCollider.enabled = true;
    }

    /// <summary>
    /// Ищем коробки в детишках
    /// </summary>
    void FindBoxes()
    {
        Box[] boxes = GetComponentsInChildren<Box>();
        foreach (Box box in boxes)
        {
            switch (box.color)
            {
                case BoxColors.White: boxWhite = box; break;
                case BoxColors.Yellow: boxYellow = box; break;
            }

            if (box.boxCollider == null)
                box.boxCollider = box.GetComponent<BoxCollider2D>();
        }
    }

    public override void Restart()
    {
        StartCoroutine(RestartNumerator());
        base.Restart();
    }

    /// <summary>
    /// В следующем кадре чтобы выполнилось после Restart детишек
    /// Вначале сетапятся дети, а потом мы их поправляем
    /// </summary>
    /// <returns></returns>
    IEnumerator RestartNumerator() {
        yield return new WaitForEndOfFrame();
        ToDefaultState();
    }

    /// <summary>
    /// В обычное состояние
    /// </summary>
    void ToDefaultState()
    {
        boxYellow.ToDefaultState();
        boxWhite.ToDefaultState();
        boxWhite.boxCollider.enabled = false;
    }

    void ToDamagedState()
    {
        boxWhite.boxCollider.enabled = true;
        boxYellow.boxCollider.enabled = false;
    }

    void ToDestroyedState()
    {
        boxWhite.boxCollider.enabled = false;
        boxYellow.boxCollider.enabled = false;
    }
    
    /// <summary>
    /// Записывает состояние сильного ящика на ход
    /// </summary>
    /// <param name="move">Номер хода</param>
    public override void Record(int move)
    {
        StartCoroutine(RecordNumerator(move));
        base.Record(move);
    }

    /// <summary>
    /// В следующем кадре чтобы выполнилось после Record детишек
    /// </summary>
    /// <returns></returns>
    IEnumerator RecordNumerator(int move)
    {
        yield return new WaitForEndOfFrame();
        if (boxStrongHistory.ContainsKey(move))
            boxStrongHistory.Remove(move);
        BoxStrongRecordData recordData = new BoxStrongRecordData();
        recordData.yellowColliderEnabled = boxYellow.boxCollider.enabled;
        recordData.whiteColliderEnabled = boxWhite.boxCollider.enabled;
        boxStrongHistory.Add(move, recordData);
    }

    /// <summary>
    /// Сбрасывает записи о состоянии сильного ящика
    /// </summary>
    public override void RecordsReset()
    {
        boxStrongHistory = new Dictionary<int, BoxStrongRecordData>();
        base.RecordsReset();
    }

    /// <summary>
    /// Возвращаемся в состояние на конкретный ход
    /// </summary>
    /// <param name="move">Ход в который хотим вернуться</param>
    public override void ReturnToMove(int move)
    {
        StartCoroutine(ReturnToMoveNumerator(move));
        base.ReturnToMove(move);
    }
    
    /// <summary>
    /// В следующем кадре чтобы выполнилось после Record детишек
    /// </summary>
    /// <returns></returns>
    IEnumerator ReturnToMoveNumerator(int move)
    {
        yield return new WaitForEndOfFrame();
        BoxStrongRecordData recordData;
        if (boxStrongHistory.TryGetValue(move, out recordData))
        {
            if (recordData.yellowColliderEnabled && !recordData.whiteColliderEnabled)
            {
                ToDefaultState();
            }
            if (!recordData.yellowColliderEnabled && recordData.whiteColliderEnabled)
            {
                boxWhite.ToDefaultState();
                boxYellow.FadeOut();
            }
            if (!recordData.yellowColliderEnabled && !recordData.whiteColliderEnabled)
            {
                if(boxWhite.boxCollider.enabled)
                    boxWhite.FadeOut();
            }
        }
    }
}
