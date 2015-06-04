using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class TeleportTutorialUI : TutorialUI {

    public override void Start()
    {
        base.Start();
        if (!MyMaze.Instance.Tutorial.GetStep(TutorialPhase.Teleport).IsTeach())
            gameObject.SetActive(false);
        else
            FadeIn();
    }

    public override void OnCloseRequest()
    {
        base.OnCloseRequest();
        MyMaze.Instance.Tutorial.GetStep(TutorialPhase.Teleport).Complete();
        //MyMaze.Instance.Tutorial.StartStep(TutorialPhase.NotTeach);
    }
}
