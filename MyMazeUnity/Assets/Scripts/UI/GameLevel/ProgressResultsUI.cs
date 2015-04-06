using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressResultsUI : MonoBehaviour {

    public Text starsCountText;
    public Image barImage;

    private Pack pack;
    
    void Update()
    {
        if (pack == null)
        {
            pack = MyMaze.Instance.LastSelectedPack;
        }
        else
        {
            starsCountText.text = pack.StarsRecived.ToString();
            barImage.fillAmount = (float)pack.StarsRecived * 100f / (float)pack.StarsCount / 100f;
        }
    }
}
