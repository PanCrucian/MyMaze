using UnityEngine;
using System.Collections;

public class AdsMovesUI : MonoBehaviour {


    private bool isFirstShowUp = true;

    void Start()
    {
        MyMaze.Instance.Ads.OnMyMazeUILifeAds += OnLifeWindowAds;
        GameLevel.Instance.OnRestart += OnGameRestart;
    }

    void OnDestroy()
    {
        MyMaze.Instance.Ads.OnMyMazeUILifeAds -= OnLifeWindowAds;
    }

    /// <summary>
    /// Спрячем окно если рестарт уровня
    /// </summary>
    void OnGameRestart()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg.alpha >= 0.99f)
        {
            Hide();
        }
    }

    /// <summary>
    /// Спрячем это окно
    /// </summary>
    void Hide()
    {
        CGSwitcher.Instance.SetHideObject(GetComponent<Animator>());
        CGSwitcher.Instance.Switch();
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

    /// <summary>
    /// нажали кнопку для просмотра рекламы и получения 5 ходов
    /// </summary>
    public void OnFiveMovesButton()
    {
        MyMaze.Instance.Ads.ShowRewardVideoForMoves();
#if UNITY_EDITOR
        AddMovesAndClose();
#endif
    }

    /// <summary>
    /// Добавляет ходы игроку и закрывает окно
    /// </summary>
    public void AddMovesAndClose()
    {
        GameLevel.Instance.AddFiveMoves();
        GameLevel.Instance.UnPause();
        Hide();
    }
}
