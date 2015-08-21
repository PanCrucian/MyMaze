using UnityEngine;
using System.Collections;

public class TutorialUI : MonoBehaviour {

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public GameObject bottomUI;
    [HideInInspector]
    public GameObject dragZone;

    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        bottomUI = GameObject.FindGameObjectWithTag("BottomGameUI");
        dragZone = GameObject.Find("DragZone");
    }

    public void FadeIn()
    {
        if (bottomUI != null)
            bottomUI.SetActive(false);
        if (dragZone != null)
            dragZone.SetActive(false);
        animator.SetTrigger("FadeIn");
    }

    void FadeOut()
    {
        if (bottomUI != null)
            bottomUI.SetActive(true);
        if (dragZone != null)
            dragZone.SetActive(true);
        animator.SetTrigger("FadeOut");
    }

    public virtual void OnCloseRequest()
    {
        FadeOut();
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;
        //GameLevel.Instance.OnRestartRequest();
        MyMaze.Instance.Tutorial.Save();
    }
}
