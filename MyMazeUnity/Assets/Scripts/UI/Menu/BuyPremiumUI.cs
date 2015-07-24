using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BuyPremiumUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {

    public Animator menuUI;
    public Animator textBuy;

    /*private Button button;
    private InApps inApps;
    private Animator premiumUI;
    
    void Start()
    {
        button = GetComponent<Button>();
        inApps = MyMaze.Instance.InApps;
        premiumUI = GetComponentInParent<PremiumUI>().GetComponent<Animator>();
        inApps.OnPremiumBuyed += OnPremiumBuyed;
    }

    void OnPremiumBuyed()
    {
        CGSwitcher.Instance.SetHideObject(premiumUI);
        CGSwitcher.Instance.SetShowObject(menuUI);
        CGSwitcher.Instance.Switch();
    }

    void Buy()
    {
        if (!inApps.IsPremium)
            inApps.BuyPremiumRequest();
    }
    */
    public void OnPointerClick(PointerEventData eventData)
    {
        //Buy();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        /*if (!button.interactable)
            return;
        textBuy.SetTrigger("Up");*/
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        /*if (!button.interactable)
            return;
        textBuy.SetTrigger("Down");*/
    }
}
