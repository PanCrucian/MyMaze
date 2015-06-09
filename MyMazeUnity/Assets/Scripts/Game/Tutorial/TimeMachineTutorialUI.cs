using UnityEngine;
using System.Collections;

public class TimeMachineTutorialUI : TutorialUI {

    public override void Start()
    {
        base.Start();
        if (!MyMaze.Instance.Tutorial.GetStep(TutorialPhase.TimeMachine).IsTeach())
            gameObject.SetActive(false);
        else
            FadeIn();
    }

    public override void OnCloseRequest()
    {
        base.OnCloseRequest();
        MyMaze.Instance.Tutorial.GetStep(TutorialPhase.TimeMachine).Complete();
    }
}
