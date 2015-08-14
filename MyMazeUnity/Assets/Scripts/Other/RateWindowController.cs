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
        }
    }
}
