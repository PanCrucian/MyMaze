using UnityEngine;
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
        if (MyMaze.Instance.LastSelectedLevel != null)
            counterText.text = Player.Instance.MovesCount.ToString() + "/" + MyMaze.Instance.LastSelectedLevel.MinMovesRecord.ToString();
        else
            counterText.text = Player.Instance.MovesCount.ToString() + "/N";
    }
}
