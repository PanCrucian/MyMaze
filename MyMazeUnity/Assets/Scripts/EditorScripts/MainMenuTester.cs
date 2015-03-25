using UnityEngine;
using System.Collections;

public class MainMenuTester : MonoBehaviour {
    [System.Serializable]
    public class LocalData
    {
        public string mainMenuName = "cg_MainMenu";
        public CanvasGroup[] menus;
        public CanvasGroup pages;
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
            Page page = t.GetComponent<Page>();
            if (!page)
                continue;
            CanvasGroup cg = t.GetComponent<CanvasGroup>();
            if (!cg)
                continue;
            if (index > 0)
                ToggleCG(cg, false);
            else
                ToggleCG(cg, true);
            index++;
        }
    }

    public void PrepareForWork(CanvasGroup workMenu, int workPage)
    {
        if (!CheckPlayData() || !workMenu)
            return;
        foreach (CanvasGroup cg in __data.menus)
            ToggleCG(cg, false);
        ToggleCG(workMenu, true);
        ToggleCG(__data.pages, true);

        int index = 0;
        foreach (Transform t in __data.pages.transform)
        {
            Page page = t.GetComponent<Page>();
            if (!page)
                continue;
            CanvasGroup cg = t.GetComponent<CanvasGroup>();
            if (!cg)
                continue;
            if (index == workPage)
                ToggleCG(cg, true);
            else
                ToggleCG(cg, false);
            index++;
        }
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
        if (flag)
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
