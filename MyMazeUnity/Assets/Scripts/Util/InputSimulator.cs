using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InputSimulator : MonoBehaviour {

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
    /// Задержка между кликом и нажатием для красивой симуляции
    /// </summary>
    private float presThenClickDelay = 0.25f;

    void Awake()
    {
        //Сохраняем инстанс в каждой сцене
        if (Application.isPlaying)
            DontDestroyOnLoad(gameObject);

        _instance = this;
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
        ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerDownHandler);
        yield return new WaitForSeconds(presThenClickDelay);
        pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerClickHandler);
    }
}
