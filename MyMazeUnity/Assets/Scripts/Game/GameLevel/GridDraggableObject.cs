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
    /// Получить координаты позиции в системе координат Unity исходя из позиции
    /// </summary>
    /// <param name="position">Позиция как объект GridObject.Position</param>
    /// <returns></returns>
    public Vector3 GetWorldPosition(Position position)
    {
        Vector3 worldPosition = Vector3.zero;
        worldPosition = new Vector3(
            position.xCell * gridStep.x,
            position.yRow * gridStep.y,
            transform.localPosition.z
            );
        return worldPosition;
    }
}
