using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InputSimulator : MonoBehaviour
{

    public static InputSimulator Instance
    {
        get
        {
            if (!_instance)
            {
                Debug.LogError("Не могу найти экземляр класса InputSimulator");
                return null;
            }
            return _instance;
        }
    }

    private static InputSimulator _instance;

    /// <summary>
    /// Включен ли инпут?
    /// </summary>
    public bool InputEnabled
    {
        get
        {
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            return eventSystem.enabled;
        }
    }

    /// <summary>
    /// Задержка между кликом и нажатием для красивой симуляции
    /// </summary>
    private float presThenClickDelay = 0.175f;

    void Awake()
    {
        if (!IsExist())
        {
            //Сохраняем инстанс в каждой сцене
            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);

            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SimulateClick(GameObject go)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerClickHandler);
    }

    public void SimulatePress(GameObject go)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerDownHandler);
    }

    public void SimulatePressThenClick(GameObject go)
    {
        StartCoroutine(PressThenClick(go));
    }

    IEnumerator PressThenClick(GameObject go)
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerEnterHandler);
        yield return new WaitForSeconds(presThenClickDelay);
        ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerDownHandler);
        yield return new WaitForSeconds(presThenClickDelay);
        ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerUpHandler);
        ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerClickHandler);
    }

    /// <summary>
    /// Проверяет на копии себя же
    /// </summary>
    bool IsExist()
    {
        InputSimulator[] objects = GameObject.FindObjectsOfType<InputSimulator>();
        if (objects.Length > 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Выключим любое управление Input, через EventSystem
    /// </summary>
    public void OffAllInput()
    {
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        eventSystem.enabled = false;
    }

    /// <summary>
    /// Включим любое управление Input, через EventSystem
    /// </summary>
    public void OnAllInput()
    {
        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        eventSystem.enabled = true;
    }
}
