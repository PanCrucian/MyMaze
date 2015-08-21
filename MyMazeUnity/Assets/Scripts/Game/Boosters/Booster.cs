using UnityEngine;
[System.Serializable]
public class Booster : MonoBehaviour
{
    public Deligates.SimpleEvent OnOpen;
    public Level avaliableAtLevel;

    /// <summary>
    /// Флаг доступности бустера
    /// </summary>
    public bool IsClosed
    {
        get
        {
            return _isClosed;
        }
    }
    private bool _isClosed = true;

    public virtual void Start()
    {
        MyMaze.Instance.OnLevelLoad += OnLevelLoad;
    }

    /// <summary>
    /// Когда загружается очередной игровой уровень
    /// </summary>
    /// <param name="level"></param>
    void OnLevelLoad(Level level)
    {
        if (level.name.Equals(avaliableAtLevel.name))
            Open();
    }

    /// <summary>
    /// Сделать бустер доступным
    /// </summary>
    public virtual void Open()
    {
        _isClosed = false;

        if (OnOpen != null)
            OnOpen();
    }
}
