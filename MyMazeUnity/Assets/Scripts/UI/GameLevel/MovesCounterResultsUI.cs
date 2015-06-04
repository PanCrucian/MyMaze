using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovesCounterResultsUI : MonoBehaviour
{
    public Text movesText;
    public Text movesRecordText;

    void Update()
    {
        int movesCount = 0;
        if (MyMaze.Instance.LastSelectedLevel != null)
        {
            movesCount = MyMaze.Instance.LastSelectedLevel.MinMovesRecord;
            movesCount = LimitationMovesCountView(movesCount);
            movesRecordText.text = movesCount.ToString();
        }

        movesCount = Player.Instance.MovesCount;
        movesCount = LimitationMovesCountView(movesCount);
        movesText.text = movesCount.ToString();
    }

    int LimitationMovesCountView(int movesCount)
    {
        if (movesCount > 99)
            return 99;
        return movesCount;
    }
}
