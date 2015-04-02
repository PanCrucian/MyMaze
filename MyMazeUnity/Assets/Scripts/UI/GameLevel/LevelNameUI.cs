using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class LevelNameUI : MonoBehaviour {

    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (MyMaze.Instance.LastSelectedLevel == null)
            return;
        text.text = MyMaze.Instance.LastSelectedLevel.displayText;           
    }
}
