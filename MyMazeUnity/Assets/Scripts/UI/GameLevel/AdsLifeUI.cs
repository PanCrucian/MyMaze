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

    void OnRestoreLife(int units)
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg.alpha >= 0.99f)
        {
            CGSwitcher.Instance.SetHideObject(GetComponent<Animator>());
            CGSwitcher.Instance.Switch();
        }
    }

    /// <summary>
    /// Покажем экран с просмотром рекламы для восстановления сердец
    /// Напоминаю что этот метод вызывать только в главном меню
    /// </summary>
    public void Show()
    {
        if (MyMaze.Instance.IsGameLevel)
        {
            if (GameLevel.Instance.state == GameLevelStates.GameOver)
                return;
            GameLevel.Instance.Pause();
        }
        
        CGSwitcher.Instance.SetShowObject(GetComponent<Animator>());
        CGSwitcher.Instance.Switch();
        GetComponent<SoundsPlayer>().PlayOneShootSound();
        MyMaze.Instance.Ads.CallLifeWindowAds();
    }
}
