using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LivesCounterHeartUI : MonoBehaviour
{

    public float hideTime = 0.5f;
    public Image fillStar;
    public Image outlineStar;

    /// <summary>
    /// Звезда будет в заливке
    /// </summary>
    public void Collect()
    {
        fillStar.color = new Color(fillStar.color.r, fillStar.color.g, fillStar.color.b, 1f);
        outlineStar.color = new Color(outlineStar.color.r, outlineStar.color.g, outlineStar.color.b, 0f);
    }

    /// <summary>
    /// Звезда будет контуром
    /// </summary>
    public void Lose()
    {
        outlineStar.color = new Color(outlineStar.color.r, outlineStar.color.g, outlineStar.color.b, 1f);
        if(gameObject.activeInHierarchy)
            StartCoroutine(AnimateLose());
    }

    /// <summary>
    /// Звезда закрашена?
    /// </summary>
    /// <returns></returns>
    public bool IsCollected()
    {
        return fillStar.gameObject.activeSelf;
    }

    IEnumerator AnimateLose()
    {
        Color c = fillStar.color;
        for (float t = 0; t <= hideTime; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(1f, 0f, t / hideTime);
            fillStar.color = c;
            yield return new WaitForEndOfFrame();
        }
        c.a = 0f;
        fillStar.color = c;
    }
}
