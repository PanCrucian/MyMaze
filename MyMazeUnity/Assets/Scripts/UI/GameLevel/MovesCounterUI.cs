﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovesCounterUI : MonoBehaviour {

    public Text counterText;

    void Update()
    {
        if (counterText == null)
        {
            Debug.LogWarning("Не могу найти ссылку на компоненту текст");
            return;
        }
        int movesCount = Player.Instance.MovesCount;
        if (movesCount > 99)
            movesCount = 99;
        if (MyMaze.Instance.LastSelectedLevel != null)
            counterText.text = movesCount.ToString() + "/" + MyMaze.Instance.LastSelectedLevel.GetSimpleStars()[2].movesToGet.ToString();
        else
            counterText.text = movesCount.ToString() + "/N";
    }
}
