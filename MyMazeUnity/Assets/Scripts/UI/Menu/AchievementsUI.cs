﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AchievementsUI : MonoBehaviour, IPointerClickHandler {
    private Button button;
    private ColorBlock tempColorBlock;

    void Start()
    {
        tempColorBlock = new ColorBlock();
        button = GetComponent<Button>();
        tempColorBlock = button.colors;
    }

#if UNITY_IPHONE
    void Update()
    {
        if (MyMaze.Instance.GameCenter.IsAuth && MyMaze.Instance.GameCenter.IsAchievementsLoaded)
            EnableButton();
        else
            DisableButton();
    }
#else
    void Update()
    {
        DisableButton();
    }
#endif

    void EnableButton()
    {
        tempColorBlock.disabledColor = tempColorBlock.normalColor;
        button.interactable = true;
        button.colors = tempColorBlock;
    }

    void DisableButton()
    {
        tempColorBlock.disabledColor = tempColorBlock.pressedColor;
        button.interactable = false;
        button.colors = tempColorBlock;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameCenterManager.ShowAchievements();
    }
}
