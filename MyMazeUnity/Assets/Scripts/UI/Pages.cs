using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Pages : MonoBehaviour {
    [System.Serializable]
    public class Local_data
    {
        public Page[] pages;
    }

    public Local_data __data;
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
            if (t.GetComponent<Page>())
                pagescount++;
        }
        __data.pages = new Page[pagescount];
        int i = 0;
        foreach (Transform t in transform)
        {
            Page page = t.GetComponent<Page>();
            if (page)
                __data.pages[i] = page;
            i++;
        }
    }

    /// <summary>
    /// Перейти на следующую страницу
    /// </summary>
    public void NextPage()
    {
        PageNumber++;
    }

    /// <summary>
    /// Вернуться на предыдущую страницу
    /// </summary>
    public void PrevPage()
    {        
        PageNumber--;
    }

    public void Drag(BaseEventData data) {
        PointerEventData pointer = (PointerEventData)data;
        if (Mathf.Abs(pointer.delta.x) >= 7.5f) {
            if (Time.time - lastSwipe < swipeDelay)
                return;
            if (pointer.delta.x < 0f)
                InputSimulator.Instance.SimulateClick(__data.pages[PageNumber].buttons.nextButton.gameObject);
            else
                InputSimulator.Instance.SimulateClick(__data.pages[PageNumber].buttons.prevButton.gameObject);
            lastSwipe = Time.time;
        }
    }
}
