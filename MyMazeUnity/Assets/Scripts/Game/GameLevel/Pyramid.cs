﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class Pyramid : MonoBehaviour, IRecordingElement, IPauseable {
    public Deligates.PyramidEvent OnPyramidPickUp;

    /// <summary>
    /// Подобрана ли пирамида
    /// </summary>
    public bool IsUsed
    {
        get
        {
            return _isUsed;
        }
    }

    private bool _isUsed;
    private BoxCollider2D collider2d;
    private Animator animator;

    void Start()
    {
        collider2d = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameLevel.Instance.state == GameLevelStates.Pause)
            TogglePause(true);
        else
            TogglePause(false);
    }

    /// <summary>
    /// Подобрать пирамиду
    /// </summary>
    public void PickUp()
    {
        _isUsed = true;
        collider2d.enabled = false;
        animator.SetTrigger("PickUp");

        if (OnPyramidPickUp != null)
            OnPyramidPickUp(this);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag.Equals("Player"))
        {
            PickUp();
        }
    }

    /// <summary>
    /// Если игра встала на паузу
    /// </summary>
    /// <param name="pause">Состояние bool паузы</param>
    public void TogglePause(bool pause)
    {
        if (pause)
        {
            animator.speed = 0f;
        }
        else
        {
            animator.speed = 1f;
        }
    }
}
