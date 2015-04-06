using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameLevel : MonoBehaviour {
    public Deligates.DirectionEvent OnPlayerMoveRequest;
    public Deligates.SimpleEvent OnRestart;
    public DesignSettings designSettings;
    public Animator uiGame;
    public Animator uiResults;
    public GameLevelStates state;
    public List<Pyramid> pyramids;

    private Level nextLevel;

    [System.Serializable]
    public class DesignSettings
    {
        /// <summary>
        /// Диапазон значение из события инпута drag delta при котором не учитывается событие drag
        /// </summary>
        public float dragTreshold = 3f;

        /// <summary>
        /// Запрос на движение не чаще времени в этой переменной
        /// </summary>
        public float moveRequestDelayTime = 0.25f;

        /// <summary>
        /// Задержка перед завершением игры
        /// </summary>
        public float gameOverDelay = 0.5f;

        /// <summary>
        /// Задержка между анимациями получения звезд
        /// </summary>
        public float starsCollectingDelay = 0.75f;

        /// <summary>
        /// задержка перед загрузкой следующего уровня
        /// </summary>
        public float nextLevelDelay = 1f;

        [HideInInspector]
        public float lastMoveRequestTime = 0f;
    }

    public static GameLevel Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Не могу найти экземпляр класса GameLevel");
            return _instance;
        }
    }

    private static GameLevel _instance;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        //включим управление
        InputSimulator.Instance.OnAllInput();

        pyramids = transform.GetComponentsInChildren<Pyramid>().ToList<Pyramid>();
    }

    void Update()
    {
        if (state != GameLevelStates.GameOver)
            if (IsGameOver())
                GameOver();
    }

    /// <summary>
    /// Когда человек провел пальцом по экрану
    /// </summary>
    public void Drag(BaseEventData data)
    {
        PointerEventData pointer = (PointerEventData)data;
        Vector2 delta = pointer.delta;
        float threshold = designSettings.dragTreshold;
        if (Mathf.Abs(delta.x) >= threshold)
        {
            if (delta.x < 0)
                CallMoveRequest(Directions.Left);
            else
                CallMoveRequest(Directions.Right);

        }
        else if (Mathf.Abs(delta.y) >= threshold)
        {
            if (delta.y < 0)
                CallMoveRequest(Directions.Down);
            else
                CallMoveRequest(Directions.Up);
        }
    }

    /// <summary>
    /// Вызывает событие которое попросит игрока двигаться
    /// </summary>
    /// <param name="direction"></param>
    void CallMoveRequest(Directions direction)
    {
        if (Time.time - designSettings.lastMoveRequestTime < designSettings.moveRequestDelayTime)
            return;
        if (state == GameLevelStates.Pause)
        {
            Debug.Log("Сейчас у игры пауза. Не могу вызвать событие запроса на перемещение");
            return;
        }
        if (OnPlayerMoveRequest != null)
            OnPlayerMoveRequest(direction);

        designSettings.lastMoveRequestTime = Time.time;
    }

    /// <summary>
    /// Проверяет, выполнены ли условия конца игры
    /// </summary>
    /// <returns></returns>
    bool IsGameOver()
    {
        bool isGameOver = true;
        foreach (Pyramid pyramid in pyramids)
        {
            if (!pyramid.IsUsed) { 
                isGameOver = false;
                break;
            }
        }
        return isGameOver;
    }

    /// <summary>
    /// Уровень выполнил условия победы. Завершаем игру
    /// </summary>
    void GameOver()
    {
        state = GameLevelStates.GameOver;
        Player.Instance.DisableControl();
        Player.Instance.StopMovingDelay();
        StartCoroutine(GameOverEnumerator());
        Debug.Log("Игра окончена");
    }

    /// <summary>
    /// Методы по завершению игры после задержки
    /// </summary>
    /// <returns></returns>
    IEnumerator GameOverEnumerator()
    {
        //выключим любое управление
        InputSimulator.Instance.OffAllInput();

        //подождем немного и переключим экраны
        yield return new WaitForSeconds(designSettings.gameOverDelay);
        CGSwitcher.Instance.SetHideObject(uiGame);
        CGSwitcher.Instance.SetShowObject(uiResults);
        CGSwitcher.Instance.Switch();

        //установим рекорд движений
        Level lastSelectedLevel = MyMaze.Instance.LastSelectedLevel;
        if (lastSelectedLevel.MinMovesRecord == 0)
            lastSelectedLevel.MinMovesRecord = Player.Instance.MovesCount;
        else
            if (Player.Instance.MovesCount <= lastSelectedLevel.MinMovesRecord)
                lastSelectedLevel.MinMovesRecord = Player.Instance.MovesCount;
        
        //проверим какие звезды мы должны получить и получим их
        List<Star> stars = MyMaze.Instance.LastSelectedLevel.GetSimpleStars();  
        foreach (Star star in stars)
        {
            if (Player.Instance.MovesCount <= star.movesToGet)
                star.IsCollected = true;
            if (star.IsCollected) {
                Animator starAnimator = StarsResultsUI.Instance.GetNotCollectedStar();
                if (starAnimator != null)
                {
                    starAnimator.SetBool("IsCollected", true);
                    yield return new WaitForSeconds(designSettings.starsCollectingDelay);
                }
            }
        }
                
        //откроем следующий уровень
        Pack pack = MyMaze.Instance.LastSelectedPack;
        nextLevel = pack.GetNextLevel(lastSelectedLevel);
        if (nextLevel == null)
        {
            Debug.Log("Пак " + pack.packName + " пройден");
            pack = MyMaze.Instance.GetNextPack(pack);
            if (pack != null)
                nextLevel = pack.levels[0];
        }

        if (nextLevel != null)
            nextLevel.IsClosed = false;
        else
            Debug.Log("Игра пройдена, нет уровня для загрузки");

        //включим управление Input
        InputSimulator.Instance.OnAllInput();
    }

    /// <summary>
    /// Пробуем загрузить следующий игровой уровень
    /// </summary>
    public void OnNextLevelRequest()
    {
        //выключим любое управление
        InputSimulator.Instance.OffAllInput();

        StartCoroutine(NextLevelNumerator());
    }

    IEnumerator NextLevelNumerator()
    {
        yield return new WaitForSeconds(designSettings.nextLevelDelay);
        ScreenOverlayUI.Instance.FadeIn();
        yield return new WaitForSeconds(ScreenOverlayUI.Instance.FadeDelay);

        //загружаем меню
        if (nextLevel == null)
        {
            Debug.Log("Уровни кончились, загружаю меню");
            MyMaze.Instance.LevelLoader.LoadMenu();
        }

        //обновляем курсоры на паки и уровни
        foreach (Pack pack in MyMaze.Instance.packs)
        {
            if (pack.IsYourLevel(nextLevel))
            {
                MyMaze.Instance.LastSelectedPack = pack;
                MyMaze.Instance.LastSelectedLevel = nextLevel;
                break;
            }
        }

        //загружаем следующий уровень
        Debug.Log("Загружаю следующий уровень " + nextLevel.name);
        Application.LoadLevel(nextLevel.name);
    }

    /// <summary>
    /// Нажали кнопку рестарта уровня
    /// </summary>
    public void OnRestartRequest()
    {
        if (state == GameLevelStates.GameOver)
        {
            CGSwitcher.Instance.SetHideObject(uiResults);
            CGSwitcher.Instance.SetShowObject(uiGame);
            CGSwitcher.Instance.Switch();
        }
        state = GameLevelStates.Game;
        if (OnRestart != null)
            OnRestart();
    }
}
