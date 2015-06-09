using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarUI : MonoBehaviour {
    
    [System.Serializable]
    public struct StarsStruct
    {
        public Image recived;
        public Image lost;
    }

    public StarsStruct stars;

    /// <summary>
    /// звезда подобрана
    /// </summary>
    public void SetRecivedStar()
    {
        ToggleStars(true);
    }

    /// <summary>
    /// Звезда утеряна
    /// </summary>
    public void SetLostStar()
    {
        ToggleStars(false);
    }
    
    private void ToggleStars(bool flag)
    {
        if (flag)
        {
            stars.recived.gameObject.SetActive(true);
            stars.lost.gameObject.SetActive(false);
        }
        else
        {
            stars.lost.gameObject.SetActive(true);
            stars.recived.gameObject.SetActive(false);
        }
    }
}
