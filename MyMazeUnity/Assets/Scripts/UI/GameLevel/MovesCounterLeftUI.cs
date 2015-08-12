using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovesCounterLeftUI : MonoBehaviour {

    private Text t;

    void Start()
    {
        t = GetComponent<Text>();
    }

    void Update()
    {
        t.text = System.String.Format(MyMaze.Instance.Localization.GetLocalized("moves_left"), GameLevel.Instance.MovesLeft);
    }
}
