using UnityEngine;
using System.Collections;

public class PackPageDotsUI : MonoBehaviour {
    private PageDotUI[] dots;
    private int dotsCount;

    void Start()
    {
        dots = GetComponentsInChildren<PageDotUI>();
        dotsCount = dots.Length;
    }

    void Update()
    {
        int pageNum = MyMaze.Instance.LastSelectedPageNumber;

        if (pageNum < dotsCount)
        {
            for(int i = 0; i < dotsCount; i++) {
                if (i == pageNum)
                    dots[i].Show();
                else
                    dots[i].Hide();
            }
        }
    }
}
