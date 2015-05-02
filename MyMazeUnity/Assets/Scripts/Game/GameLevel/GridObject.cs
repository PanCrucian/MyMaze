using UnityEngine;
using System.Collections;

public class GridObject : Grid {
    [System.Serializable]
    public class Position
    {
        public int xCell;
        public int yRow;

        /// <summary>
        /// Сравнение позиций
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool Equals(Position position)
        {
            if (this.xCell == position.xCell && this.yRow == position.yRow)
                return true;
            return false;
        }

        /// <summary>
        /// Делаем клон в памяти
        /// </summary>
        /// <returns></returns>
        public Position Clone()
        {
            Position position = new Position();
            position.xCell = this.xCell;
            position.yRow = this.yRow;
            return position;
        }
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
    private float oldUpdateTime = 0f;

    void Start()
    {
        UpdatePositionVars();
        _startPosition = position.Clone();
        oldUpdateTime = Time.time;
    }

    void Update()
    {
        if (Mathf.Abs(oldUpdateTime - Time.time) >= 0.5f)
        {
            UpdatePositionVars();
            oldUpdateTime = Time.time;
        }
    }

    /// <summary>
    /// Обновляет информацию о положении объекта в сетке исходя из локальных координат позиции
    /// </summary>
    public void UpdatePositionVars()
    {
        position.xCell = (int) Mathf.Round(transform.localPosition.x / gridStep.x);
        position.yRow = (int) Mathf.Round(transform.localPosition.y / gridStep.y);
    }

    /// <summary>
    /// Устанавливает MyMaze координаты объекта в Position
    /// </summary>
    /// <param name="position">Координаты</param>
    public void SetPositionVars(Position position)
    {
        this.position = position.Clone();
    }
}
