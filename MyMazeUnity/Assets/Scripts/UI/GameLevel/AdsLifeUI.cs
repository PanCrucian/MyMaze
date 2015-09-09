using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AdsLifeUI : MonoBehaviour {

    void Start()
    {
        MyMaze.Instance.Life.OnRestoreLife += OnRestoreLife;
    }

    void OnDestroy()
    {
        MyMaze.Instance.Life.OnRestoreLife -= OnRestoreLife;
    }

    /// <summary>
    /// Отрегенерировалась ячейка жизней, закроем экран с рекламой
    /// </summary>
    /// <param name="units"></param>
    void OnRestoreLife(int units)
    {
        if (GetComponent<CanvasGroup>().alpha >= 0.99f)
            Hide();
    }

    /// <summary>
    /// Спрячем экран с предложением рекламы
    /// </summary>
    public void Hide()
    {
        CGSwitcher.Instance.SetHideObject(GetComponent<Animator>());
        CGSwitcher.Instance.Switch();
    }

    /// <summary>
    /// Покажем экран с просмотром рекламы для восстановления сердец
    /// </summary>
    public void Show()
    {
        if (GetComponent<CanvasGroup>().alpha >= 0.1f)
            return;
        if (MyMaze.Instance.IsGameLevel)
        {
            if (GameLevel.Instance.state == GameLevelStates.GameOver)
                return;
            GameLevel.Instance.Pause();
        }
        
        CGSwitcher.Instance.SetShowObject(GetComponent<Animator>());
        CGSwitcher.Instance.Switch();

        GetComponent<SoundsPlayer>().PlayOneShootSound();

        MyMaze.Instance.Ads.CallMyMazeUILifeAdsShowEvent();
    }

    /// <summary>
    /// нажали кнопку просмотра рекламы
    /// </summary>
    public void OnFreeLifeButton()
    {
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
        MyMaze.Instance.Ads.ShowRewardVideoForLife();
#endif
#if UNITY_EDITOR
        MyMaze.Instance.Life.RestoreOneUnit();
#endif
    }

    /// <summary>
    /// Нажали кнопку покупки 5 жизней
    /// </summary>
    public void OnFiveLivesButton()
    {
        ProductTypes productType = ProductTypes.FiveLives;
        if (!MyMaze.Instance.InApps.IsOwned(productType))
            MyMaze.Instance.InApps.BuyRequest(productType);
    }

    /// <summary>
    /// Нажали кнопку покупки бесконечных жизней
    /// </summary>
    public void OnUnlimitedLivesButton()
    {
        ProductTypes productType = ProductTypes.UnlimitedLives;
        if (!MyMaze.Instance.InApps.IsOwned(productType))
            MyMaze.Instance.InApps.BuyRequest(productType);
    }
}
