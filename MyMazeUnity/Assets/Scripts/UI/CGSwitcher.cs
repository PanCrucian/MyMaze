using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CGSwitcher : MonoBehaviour {

    public Deligates.SimpleEvent OnSwitched;

    private List<Animator> animatorForHide = new List<Animator>();
    private List<Animator> animatorForShow = new List<Animator>();
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
        animatorForHide.Add(animator);
    }

    public void SetShowObject(Animator animator)
    {
        animatorForShow.Add(animator);
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
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        while (animatorForHide.Count > 0)
        {
            int index = animatorForHide.Count - 1;
            animatorForHide[index].SetTrigger("FadeOut");
            animatorForHide.Remove(animatorForHide[index]);
        }
        while (animatorForShow.Count > 0)
        {
            int index = animatorForShow.Count - 1;
            animatorForShow[index].gameObject.SetActive(true);
            animatorForShow[index].SetTrigger("FadeIn");
            animatorForShow.Remove(animatorForShow[index]);
        }
        delay = 0f;
    }
}
