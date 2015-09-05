using UnityEngine;
using System.Collections;

public class SecretStarTutorialUI : TutorialUI {

    public override void Start()
    {
        base.Start();
        if (!MyMaze.Instance.Tutorial.GetStep(TutorialPhase.SecretStar).IsTeach())
            gameObject.SetActive(false);
        else
            FadeIn();
    }

    public override void OnCloseRequest()
    {
        MyMaze.Instance.Tutorial.GetStep(TutorialPhase.SecretStar).Complete();
        base.OnCloseRequest();
    }
}
