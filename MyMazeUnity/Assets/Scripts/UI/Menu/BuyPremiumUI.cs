using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BuyPremiumUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {

    public Animator textBuy;
    
    void Buy()
    {
        MyMaze.Instance.InApps.BuyPremium();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Buy();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        textBuy.SetTrigger("Up");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        textBuy.SetTrigger("Down");
    }
}
