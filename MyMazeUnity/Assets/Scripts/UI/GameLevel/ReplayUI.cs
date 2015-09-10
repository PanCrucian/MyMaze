using UnityEngine;
using System.Collections;

public class ReplayUI : MonoBehaviour {

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

        CGSwitcher.Instance.SetShowObject(GetComponent<Animator>());
        CGSwitcher.Instance.Switch();

        GetComponent<SoundsPlayer>().PlayOneShootSound();
    }

    /// <summary>
    /// Нажали кнопку рестарта уровня
    /// </summary>
    public void OnRestartButton()
    {
        Hide();
        GameLevel.Instance.OnRestartRequest();
    }
}
