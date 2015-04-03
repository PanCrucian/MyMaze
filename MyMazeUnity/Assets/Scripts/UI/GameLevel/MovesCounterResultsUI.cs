using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovesCounterResultsUI : MonoBehaviour
{
    public Text movesText;
    public Text movesRecordText;

    void Update()
    {
        if (MyMaze.Instance.LastSelectedLevel != null)
            movesRecordText.text = MyMaze.Instance.LastSelectedLevel.MinMovesRecord.ToString();

        movesText.text = Player.Instance.MovesCount.ToString();
    }
}
