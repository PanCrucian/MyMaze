using UnityEngine;
using System.Collections;

public class MainMenuTester : MonoBehaviour {
    [System.Serializable]
    public class LocalData
    {
        public string mainMenuName = "cg_Main";
        public CanvasGroup[] menus;
        public CanvasGroup pages;
        public CanvasGroup mainMenuContent;
        public ScreenOverlayUI screenOverlay;
    }

    public LocalData __data;
    
    public void PrepareForPlay()
    {
        if (!CheckPlayData())
            return;
        foreach (CanvasGroup cg in __data.menus)
        {
            if (cg.name.Equals(__data.mainMenuName))
                ToggleCG(cg, true);
            else
                ToggleCG(cg, false);
        }
        int index = 0;
        foreach (Transform t in __data.pages.transform)
        {
            PageUI page = t.GetComponent<PageUI>();
            if (!page)
                continue;
            CanvasGroup pagecg = t.GetComponent<CanvasGroup>();
            if (!pagecg)
                continue;

            ToggleCG(pagecg, false);
            pagecg.gameObject.SetActive(true);

            ToggleCG(page.containers.packsContainer, true);
            if (page.containers.levelsContainer == null)
                Debug.LogWarning("Потерян контейнер с уровнями на странице " + index.ToString());
            ToggleCG(page.containers.levelsContainer, false);
            index++;
        }
        __data.mainMenuContent.alpha = 0f;
        __data.screenOverlay.gameObject.SetActive(true);
    }

    public void PrepareForWork(CanvasGroup workMenu, int workPage, bool isPackWork)
    {
        __data.mainMenuContent.alpha = 1f;
        if (!CheckPlayData() || !workMenu)
            return;
        foreach (CanvasGroup cg in __data.menus)
        {
            ToggleCG(cg, false);
        }
        ToggleCG(workMenu, true);
        ToggleCG(__data.pages, true);

        int index = 0;
        foreach (Transform t in __data.pages.transform)
        {
            PageUI page = t.GetComponent<PageUI>();
            if (!page)
                continue;
            CanvasGroup pagecg = t.GetComponent<CanvasGroup>();
            if (!pagecg)
                continue;
            if (index == workPage)
            {
                ToggleCG(pagecg, true);
                if (isPackWork)
                {
                    ToggleCG(page.containers.packsContainer, true);
                    if (page.containers.levelsContainer == null)
                        Debug.LogWarning("Потерян контейнер с уровнями на странице " + index.ToString());
                    ToggleCG(page.containers.levelsContainer, false);
                }
                else
                {
                    ToggleCG(page.containers.packsContainer, false);
                    if (page.containers.levelsContainer == null)
                        Debug.LogWarning("Потерян контейнер с уровнями на странице " + index.ToString());
                    ToggleCG(page.containers.levelsContainer, true);
                }
            }
            else
                ToggleCG(pagecg, false);
            index++;
        }
        __data.screenOverlay.gameObject.SetActive(false);
    }

    bool CheckPlayData()
    {
        if (__data == null)
            return false;
        if (__data.menus == null)
            return false;
        if (__data.pages == null)
            return false;
        return true;
    }

    void ToggleCG(CanvasGroup cg, bool flag)
    {
        if (cg == null)
        {
            Debug.LogWarning("Тестер обнаружил потерянные объекты в ссылках на компоненты");
            return;
        }
        if (flag)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
            cg.gameObject.SetActive(true);
        }
        else
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
            cg.gameObject.SetActive(false);
        }
    }

    void ToggleGameObject(GameObject obj, bool flag)
    {
        if (flag)
        {
            obj.SetActive(true);
        }
        else
        {
            obj.SetActive(false);
        }
    }

}
