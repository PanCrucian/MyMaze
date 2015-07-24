using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovesCounterUI : MonoBehaviour {

    public Text movesText;
    public MovesCounterStarsUI starsUI;

    void Update()
    {
        //посчитаем ходы
        int movesCount = Player.Instance.MovesCount;
        int movesToTheLeft_3s = starsUI.levelStars[2].movesToGet - movesCount;
        int movesToTheLeft_2s = starsUI.levelStars[2].movesToGet +
                                starsUI.levelStars[1].movesToGet - movesCount;
        int movesToTheLeft_1s = starsUI.levelStars[2].movesToGet +
                                starsUI.levelStars[1].movesToGet +
                                starsUI.levelStars[0].movesToGet - movesCount;
        int displayMoves = movesToTheLeft_3s > 0 ? movesToTheLeft_3s : movesToTheLeft_2s > 0 ? movesToTheLeft_2s : movesToTheLeft_1s;
        displayMoves = GetTruncatedCount(displayMoves);

        //покажем ходы в тексте
        movesText.text = displayMoves.ToString();

        //контролируем 3-ю звезду
        if (movesToTheLeft_3s >= 0)
            starsUI.uiStars[2].Collect();
        else
            starsUI.uiStars[2].Lose();

        //контролируем 2-ю звезду
        if (movesToTheLeft_2s >= 0)
            starsUI.uiStars[1].Collect();
        else
            starsUI.uiStars[1].Lose();
    }

    /// <summary>
    /// Усечем значение для отображаемых ходов
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int GetTruncatedCount(int value)
    {
        if (value > 99)
            return 99;
        if (value < 0)
            return 0;
        return value;
    } 
}
