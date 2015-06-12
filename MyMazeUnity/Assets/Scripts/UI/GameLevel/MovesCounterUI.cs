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
            int starMovesToGet = MyMaze.Instance.LastSelectedLevel.GetSimpleStars()[2].movesToGet; // 3 зевзды
            if (movesCount > starMovesToGet)
                starMovesToGet = MyMaze.Instance.LastSelectedLevel.GetSimpleStars()[1].movesToGet; // 2 зевзды
            if (movesCount > starMovesToGet)
                starMovesToGet = GetTruncatedCount(MyMaze.Instance.LastSelectedLevel.GetSimpleStars()[0].movesToGet); // 1 зевзда

            withStarsCounterText.text = movesCount.ToString() + "/" + starMovesToGet.ToString();
            withCupCounterText.text = movesCount.ToString() + "/" + recordMovesCount.ToString();

            if (MyMaze.Instance.LastSelectedLevel.GetSimpleStars()[2].movesToGet < recordMovesCount)
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
