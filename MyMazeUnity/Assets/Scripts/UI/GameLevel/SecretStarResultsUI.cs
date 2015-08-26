using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SecretStarResultsUI : MonoBehaviour {

    public Image bg;
    public Image fill;

    private Star star;

    void Start()
    {
        star = MyMaze.Instance.LastSelectedLevel.GetHiddenStar();

        if (star == null)
            Hide();
    }

    void Update()
    {
        if (star != null)
            if (star.IsCollected)
                Collect();
            else
                Lose();
    }

    /// <summary>
    /// Спрячем плейсхолдер секретной звезды
    /// </summary>
    void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Покажем визуально что звезда подобрана
    /// </summary>
    void Collect()
    {
        if (fill.color.a >= 0.99f)
            return;

        fill.color = new Color(
            fill.color.r, fill.color.g, fill.color.b,
            1f
            );
    }

    /// <summary>
    /// Покажем визуально что зведа не собрана
    /// </summary>
    void Lose()
    {
        if (fill.color.a <= 0.01f)
            return;

        fill.color = new Color(
            fill.color.r, fill.color.g, fill.color.b,
            0f
            );
    }
}
