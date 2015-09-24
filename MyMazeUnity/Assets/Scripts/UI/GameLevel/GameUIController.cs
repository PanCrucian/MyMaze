using UnityEngine;
using System.Collections;

public class GameUIController : MonoBehaviour {

    public ShopUI shopUIPrefab;
    private ShopUI shopUIInstance;
    private CartButtonUI cartBtnUI;
    private Animator resultsUI;

    void Awake()
    {
        cartBtnUI = transform.parent.GetComponentInChildren<CartButtonUI>();
        resultsUI = transform.parent.FindChild("cg_Results").GetComponent<Animator>();
        CreateUiShop();
        shopUIInstance.showUI = resultsUI;
        cartBtnUI.hideUI = resultsUI;
        cartBtnUI.showUI = shopUIInstance.GetComponent<Animator>();
    }

    void CreateUiShop()
    {
        shopUIInstance = Instantiate<ShopUI>(shopUIPrefab);
        shopUIInstance.name = shopUIInstance.name.Replace("(Clone)", "");
        shopUIInstance.transform.SetParent(transform.parent, false);
        CloneRectTransformData(
            GetComponent<RectTransform>(),
            shopUIInstance.GetComponent<RectTransform>()
            );
        shopUIInstance.transform.SetSiblingIndex(transform.parent.childCount - 2);
    }

    /// <summary>
    /// Копирует состояние UI из 1 окна в другое
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    void CloneRectTransformData(RectTransform from, RectTransform to)
    {
        to.localRotation = from.localRotation;
        to.localPosition = from.localPosition;
        to.localScale = from.localScale;
        to.anchorMin = from.anchorMin;
        to.anchorMax = from.anchorMax;
        to.anchoredPosition = from.anchoredPosition;
        to.sizeDelta = from.sizeDelta;
        to.pivot = from.pivot;
    }
}
