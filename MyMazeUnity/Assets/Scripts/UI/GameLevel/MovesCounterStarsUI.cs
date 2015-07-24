using UnityEngine;
using System.Collections.Generic;

public class MovesCounterStarsUI : MonoBehaviour
{
    public MovesCounterStarUI[] uiStars;
    [HideInInspector]
    public List<Star> levelStars;
    
    void Awake()
    {
        levelStars = MyMaze.Instance.LastSelectedLevel.GetSimpleStars();
    }
}
