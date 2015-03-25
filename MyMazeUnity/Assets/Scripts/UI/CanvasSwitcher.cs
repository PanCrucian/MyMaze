using UnityEngine;
using System.Collections;

public class CanvasSwitcher : MonoBehaviour {

    private Animator animatorForHide;
    private Animator animatorForShow;
    private float delay = 0f;

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
        animatorForHide.SetTrigger("FadeOut");
        animatorForShow.SetTrigger("FadeIn");
        delay = 0f;
    }
}
