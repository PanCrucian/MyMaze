using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressResultsUI : MonoBehaviour {

    public Text starsCountText;
    public Image barImage;
    public float animationSpeed = 10f;

    private Pack pack;
    private float velo;
    void Update()
    {
        if (pack == null)
        {
            pack = MyMaze.Instance.LastSelectedPack;
        }
        else
        {
            starsCountText.text = pack.StarsRecived.ToString();
            barImage.fillAmount = Mathf.SmoothDamp(
                barImage.fillAmount, 
                (float)pack.StarsRecived * 100f / (float)pack.StarsCount / 100f, 
                ref velo,
                animationSpeed * Time.deltaTime);
        }
    }
}
