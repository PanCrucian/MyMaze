using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovesCounterLeftUI : MonoBehaviour {

    private Text t;

    void Start()
    {
        t = GetComponent<Text>();
    }

    void Update()
    {
        t.text = System.String.Format(
                    MyMaze.Instance.Localization.GetLocalized("moves_left"), 
                    GetTruncatedCount(GameLevel.Instance.MovesLeft - GameLevel.Instance.addMoves)
                 );
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
