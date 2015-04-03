using UnityEngine;
using System.Collections;

public class GridObject : Grid {
    [System.Serializable]
    public class Position
    {
        public int xCell;
        public int yRow;
    }

    public Position position;

    /// <summary>
    /// Стратовая позиция при загрузке сцены
    /// </summary>
    public Position StartPosition
    {
        get
        {
            return _startPosition;
        }
    }
    private Position _startPosition;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        _startPosition = new Position() { xCell = position.xCell, yRow = position.yRow};
    }

    void Update()
    {
        UpdatePositionVars();
    }

    /// <summary>
    /// Обновляет информацию о положении объекта в сетке исходя из локальных координат позиции
    /// </summary>
    public void UpdatePositionVars()
    {
        position.xCell = (int) Mathf.Round(transform.localPosition.x / gridStep.x);
        position.yRow = (int) Mathf.Round(transform.localPosition.y / gridStep.y);
    }

    public void SetPositionVars(Position position)
    {
        Position newposition = new Position();
        newposition.xCell = position.xCell;
        newposition.yRow = position.yRow;
        this.position = newposition;
    }
}
