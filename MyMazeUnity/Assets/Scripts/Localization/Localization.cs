using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO;

public class Localization : MonoBehaviour
{
    public const string MISSING_KEY = "missing_key";
    public Deligates.SimpleEvent OnRefreshTextMeshes;

    public Dictionary<string, LocalizationData> words = new Dictionary<string,LocalizationData>();

    public SystemLanguage language;
    public bool updateDinamically = true;

    private string region;

    void Awake()
    {
        if(updateDinamically)
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
            return word.represent;
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
        switch (language)
        {
            case SystemLanguage.Russian:
                region = "ru";
                break;
            case SystemLanguage.English:
                region = "en";
                break;
            default:
                region = "en";
                break;
        }
        try
        {
            using (StreamReader stream = new StreamReader(Path.Combine(Application.dataPath, "Localization/" + region + ".xml")))
            {
                XElement xElement = XElement.Parse(stream.ReadToEnd());
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
        }
        catch (FileNotFoundException e)
        {
            Debug.LogWarning("Файл локализации " + region + " не найден" + "\n" + e.Message);
        }
    }
}
