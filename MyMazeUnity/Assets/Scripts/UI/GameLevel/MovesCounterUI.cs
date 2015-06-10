using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovesCounterUI : MonoBehaviour {

    public Text withStarsCounterText;
    public Text withCupCounterText;

    public GameObject starsContainer;
    public GameObject cupContainer;

    void Update()
    {
        if (withStarsCounterText == null)
        {
            Debug.LogWarning("Не могу найти ссылку на компоненту текст");
            return;
        }
        int movesCount = GetTruncatedCount(Player.Instance.MovesCount);
        int recordMovesCount = GetTruncatedCount(MyMaze.Instance.LastSelectedLevel.MinMovesRecord);
        if (MyMaze.Instance.LastSelectedLevel != null)
        {
            int lastStarMovesToGet = MyMaze.Instance.LastSelectedLevel.GetSimpleStars()[2].movesToGet;
            withStarsCounterText.text = movesCount.ToString() + "/" + lastStarMovesToGet.ToString();
            withCupCounterText.text = movesCount.ToString() + "/" + recordMovesCount.ToString();
            if (lastStarMovesToGet < recordMovesCount)
                ActivateStarsContainer();
            else
                ActivateCupContainer();
        }
        else
            withStarsCounterText.text = movesCount.ToString() + "/N";
    }

    void ActivateStarsContainer()
    {
        if (!starsContainer.activeSelf)
            starsContainer.SetActive(true);
        if (cupContainer.activeSelf)
            cupContainer.SetActive(false);
    }
    void ActivateCupContainer()
    {
        if (!cupContainer.activeSelf)
            cupContainer.SetActive(true);
        if (starsContainer.activeSelf)
            starsContainer.SetActive(false);
    }

    int GetTruncatedCount(int value)
    {
        if (value > 99)
            return 99;
        return value;
    } 
}
