using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Page : MonoBehaviour {
    [System.Serializable]
    public struct Buttons
    {
        public Button nextButton;
        public Button prevButton;
    }
    public Buttons buttons;
    private Pages pages;

    void Start()
    {
        pages = transform.parent.GetComponent<Pages>();
        if (!pages)
        {
            Debug.LogError("Page может находиться только внутри Pages!");
            return;
        }
        if (!buttons.nextButton || !buttons.prevButton)
            Debug.LogWarning("Кнопки навигации не найдены");
    }
}
