using UnityEngine;
using System.Collections;

public static class Localization
{
    public const string MISSING_KEY = "missing_key";
    public static Deligates.SimpleEvent OnRefreshTextMeshes;
    
    public static string GetLocalezed(string key)
    {
        //TODO переводчик
        return key;
    }

    public static void RefreshTextMeshes()
    {
        if (OnRefreshTextMeshes != null)
        {
            OnRefreshTextMeshes();
            Debug.Log("Компоненты Text были обновлены с учетом текущего языка");
        }

    }
}
