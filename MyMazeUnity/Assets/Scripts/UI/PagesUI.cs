using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PagesUI : MonoBehaviour {
    [System.Serializable]
    public class Local_data
    {
        public PageUI[] pages;
    }

    public Local_data __data;
    public CGSwitcherStruct switcherData;
    public float swipeDelay = 0.25f;

    private float lastSwipe = 0f;
    public int PageNumber 
    {
        get
        {
            return pageNumber;
        }
        set
        {
            if (value >= 0)
                pageNumber = value;
            else
            {
                pageNumber = 0;
                Debug.LogWarning("Значение индекса не может быть отрицательным, установил свое значение");
            }
        }
    }

    private int pageNumber;

    void Start()
    {
        SetupPages();
        MyMaze game = MyMaze.Instance;

        if (game.LastSelectedPage == null)
            game.LastSelectedPage = __data.pages[0];

        foreach (PageUI page in __data.pages)
        {
            CanvasGroup cg = page.GetComponent<CanvasGroup>();
            if (game.LastSelectedPage.Equals(page))
            {
                cg.alpha = 1f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
            else
            {
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
    }

    void Update()
    {
        PageUI currentPage = __data.pages[pageNumber];
        
        if(pageNumber - 1 < 0)
            currentPage.buttons.prevButton.gameObject.SetActive(false);
        else
            currentPage.buttons.prevButton.gameObject.SetActive(true);

        if(pageNumber + 1 >= __data.pages.Length)
            currentPage.buttons.nextButton.gameObject.SetActive(false);
        else
            currentPage.buttons.nextButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Установки для страниц с уровнями
    /// </summary>
    void SetupPages()
    {
        SetupLocalData();
    }

    /// <summary>
    /// Установим ссылки на объекты
    /// </summary>
    void SetupLocalData()
    {
        FindPages();
    }
    
    /// <summary>
    /// Находим детишек с компонентой Page чтобы была ссылка на страницы
    /// </summary>
    void FindPages()
    {
        int pagescount = 0;
        foreach (Transform t in transform)
        {
            if (t.GetComponent<PageUI>())
                pagescount++;
        }
        __data.pages = new PageUI[pagescount];
        int i = 0;
        foreach (Transform t in transform)
        {
            PageUI page = t.GetComponent<PageUI>();
            if (page)
            {
                __data.pages[i] = page;
                i++;
            }
        }
    }

    /// <summary>
    /// Перейти на следующую страницу
    /// </summary>
    public void NextPage()
    {
        PageNumber++;
        Debug.Log(pageNumber);
        CGSwitcher switcher = switcherData.switcher;
        switcherData.hideObject = __data.pages[pageNumber - 1].GetComponent<Animator>();
        switcher.SetHideObject(switcherData.hideObject);
        switcherData.showObject = __data.pages[pageNumber].GetComponent<Animator>();
        switcher.SetShowObject(switcherData.showObject);
        switcher.SetDelayTime(switcherData.delay);
        switcher.Switch();
        MyMaze.Instance.LastSelectedPage = __data.pages[pageNumber];
    }

    /// <summary>
    /// Вернуться на предыдущую страницу
    /// </summary>
    public void PrevPage()
    {        
        PageNumber--;

        CGSwitcher switcher = switcherData.switcher;
        switcherData.hideObject = __data.pages[pageNumber + 1].GetComponent<Animator>();
        switcher.SetHideObject(switcherData.hideObject);
        switcherData.showObject = __data.pages[pageNumber].GetComponent<Animator>();
        switcher.SetShowObject(switcherData.showObject);
        switcher.SetDelayTime(switcherData.delay);
        switcher.Switch();
        MyMaze.Instance.LastSelectedPage = __data.pages[pageNumber];
    }

    public void Drag(BaseEventData data) {
        PointerEventData pointer = (PointerEventData)data;
        if (Mathf.Abs(pointer.delta.x) >= 7.5f) {
            if (Time.time - lastSwipe < swipeDelay)
                return;
            if (pointer.delta.x < 0f)
                InputSimulator.Instance.SimulatePressThenClick(__data.pages[PageNumber].buttons.nextButton.gameObject);
            else
                InputSimulator.Instance.SimulatePressThenClick(__data.pages[PageNumber].buttons.prevButton.gameObject);
            lastSwipe = Time.time;
        }
    }
}
