using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class MoveTutorialUI : MonoBehaviour {

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (!MyMaze.Instance.Tutorial.GetStep(TutorialPhase.HowToMove).IsTeach())
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            animator.SetTrigger("FadeIn");
        }
        GameLevel.Instance.OnPlayerMoveRequest += OnPlayerMoveRequest;
    }

    void OnPlayerMoveRequest(Directions direction)
    {
        animator.SetTrigger("FadeOut");
        MyMaze.Instance.Tutorial.GetStep(TutorialPhase.HowToMove).Complete();
        MyMaze.Instance.Tutorial.StartStep(TutorialPhase.NotTeach);
        GameLevel.Instance.OnPlayerMoveRequest -= OnPlayerMoveRequest;
    }
}
