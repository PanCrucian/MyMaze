using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CartButtonUI : MonoBehaviour {

    public Animator showUI;
    public Animator hideUI;

    public void OnClick()
    {
        CGSwitcher.Instance.SetHideObject(hideUI);
        CGSwitcher.Instance.SetShowObject(showUI);
        CGSwitcher.Instance.Switch();
    }
}
