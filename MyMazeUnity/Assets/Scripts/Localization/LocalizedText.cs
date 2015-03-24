using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocalizedText : MonoBehaviour {

    public string key;

    private Text textMesh;

    void Start()
    {
        textMesh = GetComponent<Text>();
        if (!textMesh)
        {
            Debug.LogWarning("Вы пытаетесь перевести текст, на котором нет компоненты Text");
            return;
        }

        Localization.OnRefreshTextMeshes += Refresh;

        if (key.Contains(" "))
        {
            Debug.LogWarning("Ключ не должен содержать пробелы");
        }

        if (key.Equals("") || key.Contains(" "))
        {
            key = Localization.MISSING_KEY;
            Debug.LogWarning("Ключ не должен содержать пробелы");
        }

        //Refresh();
    }

    void Refresh()
    {
        textMesh.text = Localization.GetLocalezed(key);
    }

    void OnDestroy()
    {
        Localization.OnRefreshTextMeshes -= Refresh;
    }
}
