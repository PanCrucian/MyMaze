using UnityEngine;
using System.Collections;
using System;

public class RestartGameConfirmUI : MonoBehaviour {

    public Action OnPositive;

    /// <summary>
    /// Спрячем экран с предложением рекламы
    /// </summary>
    public void Hide()
    {
        if (GetComponent<CanvasGroup>().alpha < 0.1f)
            return;
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

        CGSwitcher.Instance.SetShowObject(GetComponent<Animator>());
        CGSwitcher.Instance.Switch();

        GetComponent<SoundsPlayer>().PlayOneShootSound();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (GetComponent<CanvasGroup>().alpha >= 0.1f)
                Hide();
    }

    public void Positive()
    {
        GameLevel.Instance.OnRestartRequest();
        if (OnPositive != null)
            OnPositive();
        Hide();
    }

    public void Negative()
    {
        Hide();
    }
}
