using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameLevel : MonoBehaviour {
    public Deligates.DirectionEvent OnPlayerMoveRequest;
    public DesignSettings designSettings;
    public GameLevelStates state;
    public List<Pyramid> pyramids;

    [System.Serializable]
    public class DesignSettings
    {
        /// <summary>
        /// Диапазон значение из события инпута drag delta при котором не учитывается событие drag
        /// </summary>
        public float dragTreshold = 3f;

        /// <summary>
        /// Запрос на движение не чаще времени в этой переменной
        /// </summary>
        public float moveRequestDelayTime = 0.25f;
        public string sceneMenuName = "Main";

        [HideInInspector]
        public float lastMoveRequestTime = 0f;
    }

    public static GameLevel Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Не могу найти экземпляр класса GameLevel");
            return _instance;
        }
    }

    private static GameLevel _instance;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        pyramids = transform.GetComponentsInChildren<Pyramid>().ToList<Pyramid>();
    }

    /// <summary>
    /// Когда человек провел пальцом по экрану
    /// </summary>
    public void Drag(BaseEventData data)
    {
        PointerEventData pointer = (PointerEventData)data;
        Vector2 delta = pointer.delta;
        float threshold = designSettings.dragTreshold;
        if (Mathf.Abs(delta.x) >= threshold)
        {
            if (delta.x < 0)
                CallMoveRequest(Directions.Left);
            else
                CallMoveRequest(Directions.Right);

        }
        else if (Mathf.Abs(delta.y) >= threshold)
        {
            if (delta.y < 0)
                CallMoveRequest(Directions.Down);
            else
                CallMoveRequest(Directions.Up);
        }
    }

    /// <summary>
    /// Вызывает событие которое попросит игрока двигаться
    /// </summary>
    /// <param name="direction"></param>
    void CallMoveRequest(Directions direction)
    {
        if (Time.time - designSettings.lastMoveRequestTime < designSettings.moveRequestDelayTime)
            return;
        if (state == GameLevelStates.Pause)
        {
            Debug.Log("Сейчас у игры пауза. Не могу вызвать событие запроса на перемещение");
            return;
        }
        if (OnPlayerMoveRequest != null)
            OnPlayerMoveRequest(direction);

        designSettings.lastMoveRequestTime = Time.time;
    }
}
