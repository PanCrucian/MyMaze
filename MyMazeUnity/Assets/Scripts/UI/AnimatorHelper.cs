using UnityEngine;
using System.Collections;

public class AnimatorHelper : MonoBehaviour {

    public void CanvasSwitcherAnimationEnd()
    {
        gameObject.SetActive(false);
    }
}
