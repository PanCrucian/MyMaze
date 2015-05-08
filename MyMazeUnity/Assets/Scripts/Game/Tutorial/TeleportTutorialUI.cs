using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class TeleportTutorialUI : MonoBehaviour {

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (!MyMaze.Instance.Tutorial.GetStep(TutorialPhase.Teleport).IsTeach())
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            animator.SetTrigger("FadeIn");
        }
        GameLevel.Instance.OnPointerDown += OnPointerDown;
    }

    void OnPointerDown(Vector2 position)
    {
        animator.SetTrigger("FadeOut");
        MyMaze.Instance.Tutorial.GetStep(TutorialPhase.Teleport).Complete();
        MyMaze.Instance.Tutorial.StartStep(TutorialPhase.NotTeach);
        GameLevel.Instance.OnPointerDown -= OnPointerDown;
    }
}
