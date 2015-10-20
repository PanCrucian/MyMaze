using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonReplayUI : MonoBehaviour {

    private Button button;
    public bool useLife = true;

    void Start()
    {
        MyMaze.Instance.Life.OnRestoreLife += OnRestoreLife;
        button = GetComponent<Button>();
    }

    public void OnRestartRequest()
    {
        GameLevel.Instance.uiAdsLife.Hide();
        GameLevel.Instance.uiReplay.Hide();
        GameLevel.Instance.notUseLifeFlag = !useLife;
        GameLevel.Instance.OnRestartRequest();
    }

    void Update()
    {
        if (GameLevel.Instance.state == GameLevelStates.Game && !Player.Instance.allowControl)
            button.interactable = false;
        else if (MyMaze.Instance.Life.Units <= 0 && useLife)
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
