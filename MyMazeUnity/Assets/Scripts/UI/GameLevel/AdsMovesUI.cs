using UnityEngine;
using System.Collections;

public class AdsMovesUI : MonoBehaviour {


    private bool isFirstShowUp = true;

    void Start()
    {
        MyMaze.Instance.Ads.OnLifeWindowAds += OnLifeWindowAds;
    }

    void OnDestroy()
    {
        MyMaze.Instance.Ads.OnLifeWindowAds -= OnLifeWindowAds;
    }

    void OnLifeWindowAds()
    {
        Reset();
    }

    /// <summary>
    /// Попытаться показать экран с просмотром рекламы и восстановления сердец
    /// </summary>
    /// <returns></returns>
    public bool TryForShow()
    {
        if (!isFirstShowUp)
            return false;
        Show();
        return true;
    }

    void Show()
    {
        if (GameLevel.Instance.state == GameLevelStates.GameOver)
            return;
        isFirstShowUp = false;
        GameLevel.Instance.Pause();
        CGSwitcher.Instance.SetShowObject(GetComponent<Animator>());
        CGSwitcher.Instance.Switch();
        GetComponent<SoundsPlayer>().PlayOneShootSound();
    }

    void Reset()
    {
        isFirstShowUp = true;
    }
}
