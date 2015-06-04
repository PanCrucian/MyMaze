using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class MoveTutorialUI : TutorialUI {

    public override void Start()
    {
        base.Start();
        if (!MyMaze.Instance.Tutorial.GetStep(TutorialPhase.HowToMove).IsTeach())
            gameObject.SetActive(false);
        else
            FadeIn();
    }

    public override void OnCloseRequest()
    {
        base.OnCloseRequest();
        MyMaze.Instance.Tutorial.GetStep(TutorialPhase.HowToMove).Complete();
        //MyMaze.Instance.Tutorial.StartStep(TutorialPhase.NotTeach);
    }
}
