using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PackUI : MonoBehaviour {
    
    public Pack pack;

    public Text numberText;
    public Text starsText;
    public Image progressImage;

    void Update()
    {
        if (pack == null)
        {
            Debug.LogWarning("Для " + name + " не установленна ссылка на пак уровней");
            return;
        }
        progressImage.fillAmount = (float)((float)pack.StarsRecived * 100f / (float)pack.StarsCount) / 100f;
        starsText.text = System.String.Format("{0:00}", pack.StarsRecived);
        numberText.text = (pack.transform.GetSiblingIndex() + 1).ToString();
    }
}
