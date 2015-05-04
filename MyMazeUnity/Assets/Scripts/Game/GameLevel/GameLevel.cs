using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameLevel : MonoBehaviour {
    public Deligates.DirectionEvent OnPlayerMoveRequest;
    public Deligates.SimpleEvent OnRestart;
    public Deligates.IntegerEvent OnReturnToMove;
    public Deligates.Vector2Event OnPointerDown;
    public Animator uiGame;
    public Animator uiResults;
    public GameLevelStates state;
    public List<Pyramid> pyramids;

    private Level nextLevel;
    
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
        uiGame = GameObject.FindGameObjectWithTag("uiGame").GetComponent<Animator>();
        uiResults = GameObject.FindGameObjectWithTag("uiResults").GetComponent<Animator>();
        pyramids = transform.GetComponentsInChildren<Pyramid>().ToList<Pyramid>();
    }

    void Update()
    {
        if (state != GameLevelStates.GameOver && state != GameLevelStates.NextLevelLoading)
            if (IsGameOver())
                GameOver();
        if (Input.GetKeyDown(KeyCode.UpArrow))
            CallMoveRequest(Directions.Up);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            CallMoveRequest(Directions.Right);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            CallMoveRequest(Directions.Down);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            CallMoveRequest(Directions.Left);
        if (Input.GetKeyDown(KeyCode.R))
            OnRestartRequest();
        if (Input.GetKeyDown(KeyCode.Space))
            InputSimulator.Instance.SimulateClick(ButtonPlayNextUI.Instance.gameObject);
    }

    /// <summary>
    /// Когда человек провел пальцом по экрану
    /// </summary>
    public void Drag(BaseEventData data)
    {
        PointerEventData pointer = (PointerEventData)data;
        Vector2 delta = pointer.delta;
        float threshold = GameLevelDesign.Instance.dragTreshold;
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
        if (Time.time - GameLevelDesign.Instance.lastMoveRequestTime < GameLevelDesign.Instance.moveRequestDelayTime)
            return;
        if (state == GameLevelStates.Pause)
        {
            Debug.Log("Сейчас у игры пауза. Не могу вызвать событие запроса на перемещение");
            return;
        }
        if (OnPlayerMoveRequest != null)
            OnPlayerMoveRequest(direction);

        GameLevelDesign.Instance.lastMoveRequestTime = Time.time;
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
        //подождем немного и переключим экраны
        yield return new WaitForSeconds(GameLevelDesign.Instance.gameOverDelay);
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
                star.Collect();
            if (star.IsCollected) {
                Animator starAnimator = StarsResultsUI.Instance.GetNotCollectedStar();
                if (starAnimator != null)
                {
                    starAnimator.SetBool("IsCollected", true);
                    yield return new WaitForSeconds(GameLevelDesign.Instance.starsCollectingDelay);
                }
            }
        }

        //пройдем текущий уровень
        if (lastSelectedLevel.IsClosed)
            lastSelectedLevel.Open();
        lastSelectedLevel.Pass();
                
        //откроем следующий уровень
        Pack pack = MyMaze.Instance.LastSelectedPack;
        nextLevel = pack.GetNextLevel(lastSelectedLevel);
        if (nextLevel == null)
        {
            Debug.Log("Пак " + pack.packName + " пройден!");
            pack = MyMaze.Instance.GetNextPack(pack);
            if (pack != null)
                nextLevel = pack.levels[0];
        }

        if (nextLevel != null)
            nextLevel.Open();
        else
            Debug.Log("Игра пройдена! Нет уровня для загрузки");

        //сохраним весь прогресс
        MyMaze.Instance.Save();
    }

    /// <summary>
    /// Пробуем загрузить следующий игровой уровень
    /// </summary>
    public void OnNextLevelRequest()
    {
        if (state != GameLevelStates.GameOver )
            return;

        StartCoroutine(NextLevelNumerator());
    }

    IEnumerator NextLevelNumerator()
    {
        state = GameLevelStates.NextLevelLoading;
        yield return new WaitForSeconds(GameLevelDesign.Instance.nextLevelDelay);
        ScreenOverlayUI.Instance.FadeIn();
        yield return new WaitForSeconds(ScreenOverlayUI.Instance.FadeDelay);

        //загружаем меню
        if (nextLevel == null)
        {
            Debug.Log("Уровни кончились, загружаю меню");
            MyMaze.Instance.LevelLoader.LoadMenu();
        }
        else
        {
            Pack pack = MyMaze.Instance.GetPackViaLevel(nextLevel);
            if (pack.IsClosed)
            {
                Debug.Log("Пак " + pack.packName + " закрыт, загружаю меню");
                MyMaze.Instance.LevelLoader.LoadMenu();
            }
            else
            {
                //обновляем курсоры на паки и уровни
                MyMaze.Instance.LastSelectedPack = MyMaze.Instance.GetPackViaLevel(nextLevel);
                MyMaze.Instance.LastSelectedLevel = nextLevel;

                //загружаем следующий уровень
                Debug.Log("Загружаю следующий уровень " + nextLevel.name);
                try
                {
                    Application.LoadLevel(nextLevel.name);
                }
                catch
                {
                    Debug.LogWarning("Не найдень уровень " + nextLevel.name + " в BuildSettings. Вместо уровня загружаю меню");
                    MyMaze.Instance.LevelLoader.LoadMenu();
                }
            }
        }
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

    /// <summary>
    /// Просим вернуть состояния игровых объектов на конкретный ход
    /// </summary>
    /// <param name="move">Номер хода</param>
    public void ReturnToMoveRequest(int move)
    {
        if (move < 0)
            return;
        if (Player.Instance.state != PlayerStates.Idle)
            return;

        if(OnReturnToMove != null)
            OnReturnToMove(move);
    }

    public void PointerDownRequest(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData) data;
        if (OnPointerDown != null)
            OnPointerDown(pointerData.position);
    }
}
