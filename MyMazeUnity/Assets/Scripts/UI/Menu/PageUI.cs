using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PageUI : MonoBehaviour {
    [System.Serializable]
    public struct Buttons
    {
        public Button nextButton;
        public Button prevButton;
    }
    [System.Serializable]
    public struct Containers
    {
        public CanvasGroup packsContainer;
        public CanvasGroup levelsContainer;
    }
    public Buttons buttons;
    public Containers containers;
    private PagesUI pages;

    void Start()
    {
        pages = transform.parent.GetComponent<PagesUI>();
        if (!pages)
        {
            Debug.LogError("Page может находиться только внутри Pages!");
            return;
        }
        if (!buttons.nextButton || !buttons.prevButton)
        {
            buttons.nextButton = GetComponentInChildren<PageNextButtonUI>().GetComponent<Button>();
            buttons.prevButton = GetComponentInChildren<PagePrevButtonUI>().GetComponent<Button>();
        }
    }
}
