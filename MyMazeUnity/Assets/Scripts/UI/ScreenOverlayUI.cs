﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class ScreenOverlayUI : MonoBehaviour {

    public float FadeDelay
    {
        get
        {
            return  0.755f;
        }
    }

    public static ScreenOverlayUI Instance
    {
        get
        {
            return _instance;
        }
    }

    private static ScreenOverlayUI _instance;
    private Image image;
    private Animator animator;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {

        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
        if (MyMaze.Instance == null)
            return;
        canvasGroup.alpha = 1f;
        FadeOut();
    }

    public void FadeIn()
    {
        animator.SetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }
}
