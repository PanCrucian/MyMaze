using UnityEngine;
using System.Collections;

public class MovesCounterStarUI : MonoBehaviour {

    public GameObject fillStar;
    public GameObject outlineStar;

    /// <summary>
    /// Звезда будет в заливке
    /// </summary>
    public void Collect()
    {
        fillStar.SetActive(true);
        outlineStar.SetActive(false);
    }

    /// <summary>
    /// Звезда будет контуром
    /// </summary>
    public void Lose()
    {
        fillStar.SetActive(false);
        outlineStar.SetActive(true);
    }

    /// <summary>
    /// Звезда закрашена?
    /// </summary>
    /// <returns></returns>
    public bool IsCollected()
    {
        return fillStar.gameObject.activeSelf;
    }
}
