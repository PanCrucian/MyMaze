using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonReplayUI : MonoBehaviour {

    private Button button;

    void Start()
    {
        MyMaze.Instance.Life.OnRestoreLife += OnRestoreLife;
        button = GetComponent<Button>();
    }

    public void OnRestartRequest()
    {
        GameLevel.Instance.uiAdsLife.Hide();
        GameLevel.Instance.uiReplay.Hide();
        GameLevel.Instance.OnRestartRequest();
    }

    void Update()
    {
        if (GameLevel.Instance.state == GameLevelStates.Game && !Player.Instance.allowControl)
            button.interactable = false;
        else
            button.interactable = true;
    }

    void OnDestroy()
    {
        MyMaze.Instance.Life.OnRestoreLife -= OnRestoreLife;
    }

    void OnRestoreLife(int index)
    {
        GetComponent<Button>().interactable = true;
    }
}
