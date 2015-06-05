using UnityEngine;
using System.Collections;

public class InApps : MonoBehaviour, ISavingElement {

    public bool IsPremium;

    public void BuyPremium()
    {
        if (!IsPremium)
        {
            Debug.Log("Купили премиум");
            IsPremium = true;
        }
        else
        {
            Debug.Log("Премиум уже приобретен");
        }
    }

    public void Save()
    {
        PlayerPrefs.SetInt("IsPremium", System.Convert.ToInt32(IsPremium));
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("IsPremium"))
            IsPremium = System.Convert.ToBoolean(PlayerPrefs.GetInt("IsPremium"));
    }

    public void ResetSaves()
    {
        throw new System.NotImplementedException();
    }
}
