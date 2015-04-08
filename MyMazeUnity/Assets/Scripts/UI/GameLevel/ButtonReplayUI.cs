using UnityEngine;
using System.Collections;

public class ButtonReplayUI : MonoBehaviour {

    public void OnRestartRequest()
    {
        GameLevel.Instance.OnRestartRequest();
    }
}
