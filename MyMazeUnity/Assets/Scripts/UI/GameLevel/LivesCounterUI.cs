﻿using UnityEngine;
using System.Collections;

public class LivesCounterUI : MonoBehaviour {

    public LivesCounterHeartUI[] hearts; 

    void Start()
    {
        StartCoroutine(StartNumerator());
        MyMaze.Instance.Life.OnUseLife += OnUseLife;
        MyMaze.Instance.Life.OnRestoreLife += OnRestoreLife;
    }

    IEnumerator StartNumerator()
    {
        yield return new WaitForEndOfFrame();
        SetupLifeUI();
    }

    void OnEnable()
    {
        SetupLifeUI();
    }

    void OnDestroy()
    {
        MyMaze.Instance.Life.OnUseLife -= OnUseLife;
        MyMaze.Instance.Life.OnRestoreLife -= OnRestoreLife;
    }

    /// <summary>
    /// Использовали жизнь
    /// </summary>
    /// <param name="units"></param>
    void OnUseLife(int units)
    {
        hearts[units].Lose();
        GetComponent<SoundsPlayer>().PlayOneShootSound();
    }

    /// <summary>
    /// Восстановили жизнь
    /// </summary>
    /// <param name="units"></param>
    void OnRestoreLife(int units)
    {
        hearts[units].Collect();
        GetComponent<SoundsPlayer>().PlayOneShootSound();
    }

    /// <summary>
    /// Первая установка отображения жизней
    /// </summary>
    void SetupLifeUI()
    {
        if (MyMaze.Instance == null)
            return;

        for (int i = 0; i < MyMaze.Instance.Life.MaxUnits; i++)
        {
            if (i < MyMaze.Instance.Life.Units)
                hearts[i].Collect();
            else
                hearts[i].Lose();
        }
    }

    void LoseAll()
    {
        foreach (LivesCounterHeartUI heart in hearts)
            heart.Lose();
    }
}