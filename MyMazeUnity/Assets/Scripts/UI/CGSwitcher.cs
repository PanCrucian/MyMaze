using UnityEngine;
using System.Collections;

public class CGSwitcher : MonoBehaviour {

    public Deligates.SimpleEvent OnSwitched;

    private Animator animatorForHide;
    private Animator animatorForShow;
    private float delay = 0f;

    public static CGSwitcher Instance
    {
        get
        {
            return _instance;
        }
    }
    private static CGSwitcher _instance;
    
    void Awake()
    {
        _instance = this;
    }

    public void SetHideObject(Animator animator)
    {
        animatorForHide = animator;
    }

    public void SetShowObject(Animator animator)
    {
        animatorForShow = animator;
    }

    public void SetDelayTime(float value)
    {
        delay = value;
    }

    public void Switch()
    {
        StartCoroutine(SwitchAfterDelay());        
    }

    IEnumerator SwitchAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        if (animatorForHide != null)
            animatorForHide.SetTrigger("FadeOut");
        if (animatorForShow != null)
            animatorForShow.SetTrigger("FadeIn");
        delay = 0f;
    }
}
