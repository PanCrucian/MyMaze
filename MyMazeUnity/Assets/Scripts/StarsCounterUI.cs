using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarsCounterUI : MonoBehaviour {

    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

	void Update () {
        text.text = System.String.Format("{0:000}", MyMaze.Instance.StarsRecived) + "/" + System.String.Format("{0:000}", MyMaze.Instance.StarsCount);
	}
}
