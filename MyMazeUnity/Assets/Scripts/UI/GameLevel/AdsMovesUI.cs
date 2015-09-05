using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AdsMovesUI : GentleMonoBeh {


    private bool isFirstShowUp = true;

    public Text restartOrExitText;
    public Button replayButton;
    public Button menuButton;

    void Start()
    {
        MyMaze.Instance.Ads.OnMyMazeUILifeAds += OnLifeWindowAds;
        GameLevel.Instance.OnRestart += OnGameRestart;
        SetGentleCPURate(30);
    }

    void OnDestroy()
    {
        MyMaze.Instance.Ads.OnMyMazeUILifeAds -= OnLifeWindowAds;
    }

    public override void GentleUpdate()
    {
        base.GentleUpdate();
        SetRestartOrExitText();
    }

    void SetRestartOrExitText()
    {
        if(MyMaze.Instance.Life.Units > 0)
            restartOrExitText.text = MyMaze.Instance.Localization.GetLocalized("Restart_level");
        else
            restartOrExitText.text = MyMaze.Instance.Localization.GetLocalized("Back_to_menu");
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

    /// <summary>
    /// Нажали кнопку рестарта или выхода в меню
    /// </summary>
    public void RestartOrExitButton()
    {
        if (MyMaze.Instance.Life.Units > 0)
            InputSimulator.Instance.SimulateClick(replayButton.gameObject);
        else
            InputSimulator.Instance.SimulateClick(menuButton.gameObject);
    }
}
