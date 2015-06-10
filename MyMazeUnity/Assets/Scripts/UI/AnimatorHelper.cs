using UnityEngine;
using System.Collections;

public class AnimatorHelper : MonoBehaviour {
    public Deligates.SimpleEvent OnDeactiveObject;

    public void CanvasSwitcherAnimationEnd()
    {
        if (!gameObject.name.Equals("cg_Results"))
        {
            if (OnDeactiveObject != null)
                OnDeactiveObject();
            gameObject.SetActive(false);
        }
        else
            Debug.Log("Это cg_Results, не буду выключать чтобы звезды не лагали");
    }
}
