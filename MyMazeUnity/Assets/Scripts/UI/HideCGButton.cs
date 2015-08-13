using UnityEngine;
using System.Collections;

public class HideCGButton : MonoBehaviour {

    public Animator target;

    public void Hide()
    {
        CGSwitcher.Instance.SetHideObject(target);
        CGSwitcher.Instance.Switch();
    }
}
