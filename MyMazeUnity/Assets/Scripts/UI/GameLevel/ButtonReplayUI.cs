﻿using UnityEngine;
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
        if (GameLevel.Instance.uiAdsLife.GetComponent<CanvasGroup>().alpha > 0f)
        {
            CGSwitcher.Instance.SetHideObject(GameLevel.Instance.uiAdsLife.GetComponent<Animator>());
            CGSwitcher.Instance.Switch();
        }
        GameLevel.Instance.OnRestartRequest();
    }

    void Update()
    {
        if (MyMaze.Instance.Life.Units <= 0)
            button.interactable = false;
        else
        {
            if (GameLevel.Instance.state == GameLevelStates.Game && !Player.Instance.allowControl)
                button.interactable = false;
            else
                button.interactable = true;
        }
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
