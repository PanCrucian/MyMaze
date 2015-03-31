using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocalizedTextUI : MonoBehaviour {

    public string key;
    public bool upperCase;

    private Text textMesh;
    private Localization localization;

    void Start()
    {
        localization = MyMaze.Instance.Localization;
        textMesh = GetComponent<Text>();
        if (textMesh == null || localization == null)
        {
            Debug.LogWarning("Вы пытаетесь перевести текст, на котором нет необходимой компоненты");
            return;
        }

        localization.OnRefreshTextMeshes += Refresh;

        if (key.Contains(" "))
        {
            Debug.LogWarning("Ключ не должен содержать пробелы");
        }

        if (key.Equals("") || key.Contains(" "))
        {
            key = Localization.MISSING_KEY;
            Debug.LogWarning("Ключ не должен содержать пробелы");
        }

        Refresh();
    }

    void Refresh()
    {
        string text = localization.GetLocalezed(key);
        if (upperCase)
            text = text.ToUpper();
        textMesh.text = text;
    }

    void OnDestroy()
    {
        localization.OnRefreshTextMeshes -= Refresh;
    }
}
