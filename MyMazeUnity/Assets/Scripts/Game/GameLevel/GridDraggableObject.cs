using UnityEngine;
using System.Collections;

public class GridDraggableObject : GridObject {

    public void UpdatePosition()
    {
        transform.localPosition = new Vector3(
            base.position.xCell * gridStep.x,
            base.position.yRow * gridStep.y,
            transform.localPosition.z
            );
    }

    /// <summary>
    /// Жестко обновляем координаты MyMaze, меняется и положение на экране
    /// </summary>
    /// <param name="position">Координаты MyMaze</param>
    public void ForceUpdatePosition(Position position)
    {
        SetPositionVars(position);
        UpdatePosition();
    }

    /// <summary>
    /// Жестко Обновляем координаты MyMaze после 1 кадра
    /// </summary>
    /// <param name="position">Координаты</param>
    /// <returns></returns>
    public IEnumerator DelayedForceUpdatePosition(Position position)
    {
        yield return new WaitForEndOfFrame();
        ForceUpdatePosition(position);
    }
}
