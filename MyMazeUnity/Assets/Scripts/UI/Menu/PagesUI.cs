﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PagesUI : MonoBehaviour {
    public Deligates.IntegerEvent OnPageSwitch;

    [System.Serializable]
    public class Local_data
    {
        public PageUI[] pages;
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
        pageNumber = MyMaze.Instance.LastSelectedPageNumber;
        if (OnPageSwitch != null)
            OnPageSwitch(pageNumber);
        SetlastSelectedPageVisability();
    }

    /// <summary>
    /// Сделаем видимым последнюю выбранную страницу с паками, НЕДОДЕЛАНО УДАЛИЛ LastSelctedPage -> if pages[0] ==...
    /// </summary>
    void SetlastSelectedPageVisability()
    {
        foreach (PageUI page in __data.pages)
        {
            if (pageNumber == page.transform.GetSiblingIndex())
            {
                page.GetComponent<Animator>().SetTrigger("FadeIn");

                if (pageNumber == 0)
                    if (!MyMaze.Instance.Achievements.GetElement(AchievementsTypes.Page00Opened).IsAchieved)
                        MyMaze.Instance.Achievements.Achieve(AchievementsTypes.Page00Opened);
            }
            else
            {
                CanvasGroup cg = page.GetComponent<CanvasGroup>();
                cg.alpha = 0;
                cg.blocksRaycasts = false;
                cg.interactable = false;
                page.SetupButtons();
                page.gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        PageUI currentPage = __data.pages[PageNumber];

        if (PageNumber - 1 < 0)
            currentPage.buttons.prevButton.gameObject.SetActive(false);
        else
            currentPage.buttons.prevButton.gameObject.SetActive(true);

        if (PageNumber + 1 >= __data.pages.Length)
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
        if (Time.time - lastSwipe < swipeDelay)
            return;
        lastSwipe = Time.time;

        PageNumber++;
        //__data.pages[PageNumber].gameObject.SetActive(true);
        CGSwitcher.Instance.SetHideObject(__data.pages[PageNumber - 1].GetComponent<Animator>());
        CGSwitcher.Instance.SetShowObject(__data.pages[PageNumber].GetComponent<Animator>());        
        //CGSwitcher.Instance.SetDelayTime(0.35f);
        CGSwitcher.Instance.Switch();
        MyMaze.Instance.LastSelectedPageNumber = PageNumber;
        if (OnPageSwitch != null)
            OnPageSwitch(pageNumber);
        //StartCoroutine(OffPage(PageNumber - 1));
    }

    /// <summary>
    /// Вернуться на предыдущую страницу
    /// </summary>
    public void PrevPage()
    {
        if (Time.time - lastSwipe < swipeDelay)
            return;
        lastSwipe = Time.time;

        PageNumber--;
        //__data.pages[PageNumber].gameObject.SetActive(true);
        CGSwitcher.Instance.SetHideObject(__data.pages[PageNumber + 1].GetComponent<Animator>());
        CGSwitcher.Instance.SetShowObject(__data.pages[PageNumber].GetComponent<Animator>());
        //CGSwitcher.Instance.SetDelayTime(0.35f);
        CGSwitcher.Instance.Switch();
        MyMaze.Instance.LastSelectedPageNumber = PageNumber;
        if (OnPageSwitch != null)
            OnPageSwitch(pageNumber);
        //StartCoroutine(OffPage(PageNumber + 1));
    }

    IEnumerator OffPage(int number)
    {
        yield return new WaitForSeconds(0.55f);
        __data.pages[number].gameObject.SetActive(false);
    }

    public void Drag(BaseEventData data) {
        PointerEventData pointer = (PointerEventData)data;
        if (Mathf.Abs(pointer.delta.x) >= 7.5f) {
            if (pointer.delta.x < 0f)
                InputSimulator.Instance.SimulatePressThenClick(__data.pages[PageNumber].buttons.nextButton.gameObject);
            else
                InputSimulator.Instance.SimulatePressThenClick(__data.pages[PageNumber].buttons.prevButton.gameObject);
        }
    }
}
