using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AdsLifeUI : MonoBehaviour {
    
    //private bool isFirstShowUp = true;

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
    /// Попытаться показать экран с просмотром рекламы и восстановления сердец
    /// </summary>
    /// <returns></returns>
    public bool TryForShow()
    {
        /*if (!isFirstShowUp)
            return false;*/
        Show();
        return true;
    }

    /// <summary>
    /// Покажем экран с просмотром рекламы для восстановления сердец
    /// Напоминаю что этот метод вызывать только в главном меню
    /// </summary>
    public void Show()
    {
        if (GameLevel.Instance != null)
            GameLevel.Instance.Pause();
        //isFirstShowUp = false;
        CGSwitcher.Instance.SetShowObject(GetComponent<Animator>());
        CGSwitcher.Instance.Switch();
        GetComponent<SoundsPlayer>().PlayOneShootSound();
    }
}
