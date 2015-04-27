using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO;

[RequireComponent(typeof(MyMaze))]
public class Localization : MonoBehaviour
{
    public const string MISSING_KEY = "missing_key";
    public Deligates.SimpleEvent OnRefreshTextMeshes;

    public Dictionary<string, LocalizationData> words = new Dictionary<string,LocalizationData>();

    public SystemLanguage language;
    public bool viaSystemLanguage = true;

    private string region;
    private int tries = 0;

    void Awake()
    {
        if(viaSystemLanguage)
            language = Application.systemLanguage;
        Setup();
    }

    /// <summary>
    /// Возвращает перевод строки через ключ
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetLocalezed(string key)
    {
        LocalizationData word;
        if (words.TryGetValue(key, out word))
        {
            return word.represent.Replace("\\n","\n");
        }

        return key;
    }

    /// <summary>
    /// Обновляет все TextMesh в сцене
    /// </summary>
    public void RefreshTextMeshes()
    {
        if (OnRefreshTextMeshes != null)
        {
            OnRefreshTextMeshes();
            Debug.Log("Компоненты Text были обновлены с учетом региона " + region);
        }

    }

    /// <summary>
    /// Запускает методы по установке локализации
    /// </summary>
    public void Setup()
    {
        ReadFiles();
    }

    /// <summary>
    /// Читает папку с файлами переводов
    /// </summary>
    void ReadFiles()
    {
        region = language.ToString("g");
        try
        {
            TextAsset textAsset = (TextAsset)Resources.Load("Localization/" + region, typeof(TextAsset));
            if (textAsset == null && tries == 0)
            {
                language = SystemLanguage.English;
                Debug.LogWarning("Файл локализации " + region + " не найден ставлю " + language.ToString("g"));
                tries++;
                ReadFiles();
                return;
            }
            XElement xElement = XElement.Parse(textAsset.text);
            textAsset = null;
            words = new Dictionary<string, LocalizationData>();
            foreach (XElement xWords in xElement.Elements())
            {
                LocalizationData word = new LocalizationData();
                foreach (XElement xWord in xWords.Elements())
                {
                    switch (xWord.Name.LocalName)
                    {
                        case "key":
                            word.key = xWord.Value;
                            break;
                        case "represent":
                            word.represent = xWord.Value;
                            break;
                        default:
                            break;
                    }
                }

                words.Add(word.key, word);
            }
        }
        catch (FileNotFoundException e)
        {
            Debug.LogWarning("Файл локализации " + region + " не найден" + "\n" + e.Message);
        }
    }
}
