using UnityEngine;
using System.Collections;

public class PackPreview4x3Kludge : MonoBehaviour
{

    public float value = 13f;
    public float value2 = 4f;
    public RectTransform rTrans;
    public Vector2 screenAspect;

    void Start() {
        rTrans = GetComponent<RectTransform>();
    }

    void Update()
    {
        screenAspect = AspectRatio.GetAspectRatio(Camera.main.pixelWidth, Camera.main.pixelHeight);
        if (screenAspect.x == 3 && screenAspect.y == 4)
            rTrans.anchoredPosition = new Vector2(value, rTrans.anchoredPosition.y);
        else if(screenAspect.x == 2 && screenAspect.y == 3)
            rTrans.anchoredPosition = new Vector2(value2, rTrans.anchoredPosition.y);
        else
            rTrans.anchoredPosition = new Vector2(0, rTrans.anchoredPosition.y);
    }
}
