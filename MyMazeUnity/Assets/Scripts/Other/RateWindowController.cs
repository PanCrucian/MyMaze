using UnityEngine;
using System.Collections;

public class RateWindowController : MonoBehaviour {
    public Animator rateUI;

    void Start()
    {
        DelayedEvent dEvent = MyMaze.Instance.GetDelayedEventViaType(DelayedEventTypes.RateGame);
        if (dEvent != null)
        {
            CGSwitcher.Instance.SetShowObject(rateUI);
            CGSwitcher.Instance.Switch();
            StartCoroutine(BackButtonAndRestoreOneLifeNumerator());
        }
    }

    IEnumerator BackButtonAndRestoreOneLifeNumerator()
    {
        yield return new WaitForSeconds(0.6f);
        MyMaze.Instance.Life.RestoreOneUnit();
        BackButtonUI[] backBtns = GameObject.FindObjectsOfType<BackButtonUI>();
        foreach (BackButtonUI btn in backBtns)
            if (btn.isActiveAndEnabled)
            {
                InputSimulator.Instance.SimulateClick(btn.gameObject);
                Debug.Log("Костыль в RateWindowController, принудительно жму кнопку назад чтобы попасть на страницу с паками");
                break;
            }
    }
}
