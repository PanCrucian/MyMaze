using UnityEngine;
using System.Collections.Generic;

public class TopStarsUI : MonoBehaviour {

    public GameObject[] stars;
    
    private List<Star> levelStars;
    
    void Update()
    {
        if (levelStars == null)
        {
            if (MyMaze.Instance.LastSelectedLevel == null)
                return;
            levelStars = MyMaze.Instance.LastSelectedLevel.GetSimpleStars();
            return;
        }

        for (int i = 0; i < levelStars.Count; i++)
        {
            Star star = levelStars[i];
            if (Player.Instance.MovesCount > star.movesToGet)
            {
                if (stars[i].activeSelf)
                    stars[i].SetActive(false);
            }
            else
            {
                if (!stars[i].activeSelf)
                    stars[i].SetActive(true);
            }
        }
    }
}
