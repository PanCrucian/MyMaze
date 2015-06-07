using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressResultsUI : MonoBehaviour {

    public Text starsCountText;
    public Image barImage;
    public float animationSpeed = 10f;

    private Pack pack;
    private float velo;

    void Start()
    {
        pack = MyMaze.Instance.LastSelectedPack;
        barImage.fillAmount = (float)pack.StarsRecived / (float)pack.StarsCount;
    }

    void Update()
    {
        if (pack == null)
        {
            pack = MyMaze.Instance.LastSelectedPack;
            return;
        }
        starsCountText.text = pack.StarsRecived.ToString();
        barImage.fillAmount = Mathf.SmoothDamp(
            barImage.fillAmount,
            (float)pack.StarsRecived / (float)pack.StarsCount,
            ref velo,
            animationSpeed * Time.deltaTime);
    }
}
