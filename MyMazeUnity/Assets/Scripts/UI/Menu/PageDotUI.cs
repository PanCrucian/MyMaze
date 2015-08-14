using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PageDotUI : MonoBehaviour {
    public GameObject outline;
    public GameObject fill;

    /// <summary>
    /// в заливке
    /// </summary>
    public void Show()
    {
        fill.SetActive(true);
        outline.SetActive(false);
    }

    /// <summary>
    /// контуром
    /// </summary>
    public void Hide()
    {
        fill.SetActive(false);
        outline.SetActive(true);
    }
}
