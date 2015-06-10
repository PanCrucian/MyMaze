using UnityEngine;
using System.Collections;

public class EnergyForRateControl : MonoBehaviour, ISavingElement {

    public Animator rateUI;
    public int maxZeroRate = 2;

    private int lastRateValue;
    private int zeroRateCount;
    private bool autoShow = true;

    void Start()
    {
        rateUI.GetComponent<RateUI>().OnRate += OnRate;
        MyMaze.Instance.Energy.NotEnoughEnergy += NotEnoughEnergy;
        Load();
    }

    void OnRate(int value)
    {
        if (value == 0)
            zeroRateCount++;
        lastRateValue = value;
        Save();
    }

    void NotEnoughEnergy()
    {
        if (zeroRateCount < maxZeroRate)
            ShowRateUI();
    }

    void Update()
    {
        if (autoShow)
        {
            if (!rateUI.gameObject.activeSelf && MyMaze.Instance.Energy.Units == 0 && zeroRateCount < maxZeroRate)
            {
                ShowRateUI();
                autoShow = false;
            }
        }
    }

    void ShowRateUI()
    {
        CGSwitcher.Instance.SetShowObject(rateUI);
        CGSwitcher.Instance.Switch();
        Debug.Log("Rate Ui Showing");
    }

    void OnDestroy()
    {
        if (MyMaze.Instance == null)
            return;
        MyMaze.Instance.Energy.NotEnoughEnergy -= NotEnoughEnergy;
    }

    public void Save()
    {
        PlayerPrefs.SetInt("RateUI#lastRateValue", lastRateValue);
        PlayerPrefs.SetInt("RateUI#zeroRateCount", zeroRateCount);
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("RateUI#lastRateValue"))
            lastRateValue = PlayerPrefs.GetInt("RateUI#lastRateValue");
        if (PlayerPrefs.HasKey("RateUI#zeroRateCount"))
            zeroRateCount = PlayerPrefs.GetInt("RateUI#zeroRateCount");
    }

    public void ResetSaves()
    {
        throw new System.NotImplementedException();
    }
}
