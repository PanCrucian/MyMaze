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
    public AdsLifeUI uiAdsLife;
    public AdsMovesUI uiAdsMoves;
    public GameLevelStates state;
    public List<Pyramid> pyramids;

    private Level nextLevel;
    private SoundsPlayer soundsPlayer;
    private Vector2[] pointerDownUpVector = new Vector2[2];
    private bool isReturnToMove = false;
    private bool allowMoveRequest = true;

    public int addMoves;
    [HideInInspector]
    public int MovesLeft;
    [HideInInspector]
    public List<Star> CurrentLevelStars;
    
    public static GameLevel Instance
    {
        get
        {
            return _instance;
        }
    }

    private static GameLevel _instance;
    private bool packGroupFirstTimePassed = false;

    void Awake()
    {
        _instance = this;
        MyMaze.Instance.Save();
    }

    void Start()
    {
        CurrentLevelStars = MyMaze.Instance.LastSelectedLevel.GetSimpleStars();
        uiAdsMoves = GameObject.FindObjectOfType<AdsMovesUI>();
        uiAdsLife = GameObject.FindObjectOfType<AdsLifeUI>();
        uiGame = GameObject.FindGameObjectWithTag("uiGame").GetComponent<Animator>();
        uiResults = GameObject.FindGameObjectWithTag("uiResults").GetComponent<Animator>();
        uiResults.gameObject.SetActive(false);
        pyramids = transform.GetComponentsInChildren<Pyramid>().ToList<Pyramid>();
        soundsPlayer = GetComponent<SoundsPlayer>();
        if (soundsPlayer == null)
            Debug.LogWarning("SoundsPlayer не найден");
        else
            PlayLevelTheme();
        MyMaze.Instance.OnMenuLoad += OnMenuLoad;
        Player.Instance.OnMoveEnd += OnPlayerMoveEnd;
        MyMaze.Instance.OnPackGroupFirstTimePassed += OnPackGroupFirstTimePassed;
    }

    /// <summary>
    /// Первый раз прошли все уровни в группе
    /// </summary>
    /// <param name="group"></param>
    void OnPackGroupFirstTimePassed(PackGroupTypes group)
    {
        //выйдем в меню
        //InputSimulator.Instance.SimulateClick(GameObject.FindObjectOfType<LevelLoaderUI>().gameObject);
        //восстановим 1 еденицу энергии, т.к. выход в меню тратит её
        //MyMaze.Instance.Life.RestoreOneUnit();
        packGroupFirstTimePassed = true;
    } 

    /// <summary>
    /// Событие когда Загружается меню
    /// </summary>
    void OnMenuLoad()
    {
        Theme theme = GameObject.FindObjectOfType<Theme>();
        if (theme != null)
        {
            Destroy(theme.gameObject);
        }
    }

    /// <summary>
    /// Играть музыку уровня
    /// </summary>
    void PlayLevelTheme()
    {
        //найдем играющую тему
        Theme theme = GameObject.FindObjectOfType<Theme>();
        if (theme == null) // если ничего не играет
        {
            soundsPlayer.PlayLooped(); //играем то что настроено
        }
        else //если что-то играет
        {
            AudioSource themeAudio = theme.GetComponent<AudioSource>();
            //если играющая музыка это НЕ то же самое что и хочет сейчас играть
            if (themeAudio.clip != MyMaze.Instance.Sounds.GetAudioClip(soundsPlayer.soundName))
            {
                Destroy(themeAudio.gameObject); // то выключим то что играет
                soundsPlayer.PlayLooped(); //включим новый трек
            }
        }
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

        MovesLeft = addMoves + CurrentLevelStars[2].movesToGet + CurrentLevelStars[1].movesToGet + CurrentLevelStars[0].movesToGet - Player.Instance.MovesCount;
    }

    /// <summary>
    /// Добавляет 5 ходов игроку
    /// </summary>
    public void AddFiveMoves()
    {
        addMoves += 5;
    }

    /// <summary>
    /// Когда человек провел пальцом по экрану
    /// </summary>
    public void Drag(BaseEventData data)
    {
        if (Player.Instance.state != PlayerStates.Idle)
            return;

        PointerEventData pointerData = (PointerEventData)data;
        pointerDownUpVector[0] = pointerData.pressPosition;
        pointerDownUpVector[1] = pointerData.position;

        float fingerDistance = Vector2.Distance(pointerDownUpVector[1], pointerDownUpVector[0]);
        float xDist = pointerDownUpVector[1].x - pointerDownUpVector[0].x;
        float yDist = pointerDownUpVector[1].y - pointerDownUpVector[0].y;

        if (fingerDistance >= GameLevelDesign.Instance.scrollTreshold && allowMoveRequest)
        {
            if (Mathf.Abs(xDist) > Mathf.Abs(yDist))
            {
                if (xDist > 0)
                    CallMoveRequest(Directions.Right);
                else
                    CallMoveRequest(Directions.Left);
            }
            else if (Mathf.Abs(xDist) < Mathf.Abs(yDist))
            {
                if (yDist > 0)
                    CallMoveRequest(Directions.Up);
                else
                    CallMoveRequest(Directions.Down);
            }
            allowMoveRequest = false;
        }
    }

    /// <summary>
    /// Когда человек нажал на экран
    /// </summary>
    /// <param name="data"></param>
    public void PointerDownRequest(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        if (OnPointerDown != null)
            OnPointerDown(pointerData.position);
    }

    /// <summary>
    /// Когда человек отпустил палец от экрана
    /// </summary>
    /// <param name="data"></param>
    public void PointerUpRequest(BaseEventData data)
    {
        //PointerEventData pointerData = (PointerEventData)data;
        allowMoveRequest = true;
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
    /// Поставить игру на паузу
    /// </summary>
    public void Pause()
    {
        if (state == GameLevelStates.Game)
            state = GameLevelStates.Pause;
        else
            Debug.LogWarning("Не могу поставить паузу");
    }

    /// <summary>
    /// Снять игру с паузы
    /// </summary>
    /// <returns></returns>
    public void UnPause()
    {
        if(state == GameLevelStates.Pause)
            state = GameLevelStates.Game;
        else
            Debug.LogWarning("Не могу снять с паузы. Уже не пауза");
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

        //Переключим режим воспроизведения звуков из музыки в звуки
        soundsPlayer.type = SoundTypes.sounds;

        //проверим какие звезды мы должны получить и получим их
        yield return new WaitForEndOfFrame();
        List<Star> stars = MyMaze.Instance.LastSelectedLevel.GetSimpleStars();
        int i = 1;
        if (Player.Instance.MovesCount <= stars[2].movesToGet)
            stars[2].Collect();
        if (Player.Instance.MovesCount - stars[2].movesToGet <= stars[1].movesToGet)
            stars[1].Collect();
        stars[0].Collect();
        
        foreach (Star star in stars)
        {
            if (star.IsCollected) {
                Animator starAnimator = StarsResultsUI.Instance.GetNotCollectedStar();
                if (starAnimator != null)
                {
                    starAnimator.SetBool("IsCollected", true);
                    if (i == 1)
                        soundsPlayer.PlayOneShootSound(SoundNames.Star01);
                    else if (i == 2)
                        soundsPlayer.PlayOneShootSound(SoundNames.Star02);
                    else
                        soundsPlayer.PlayOneShootSound(SoundNames.Star03);
                    yield return new WaitForSeconds(GameLevelDesign.Instance.starsCollectingDelay);
                }
            }
            i++;
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

        //обновляем курсоры на пак и последнюю страницу
        if (nextLevel != null)
        {
            Pack nextPack = MyMaze.Instance.GetPackViaLevel(nextLevel);
            if (nextPack.StarsRequired <= MyMaze.Instance.StarsRecived)
            {
                MyMaze.Instance.LastSelectedPack = nextPack;
                MyMaze.Instance.LastSelectedPageNumber = (int)MyMaze.Instance.LastSelectedPack.group;
            }
        }

        if (nextLevel == null)
        {
            Debug.Log("Уровни кончились, загружаю меню");
            MyMaze.Instance.MenuLoadAction();
        }
        else
        {
            Pack pack = MyMaze.Instance.GetPackViaLevel(nextLevel);
            if (pack.IsClosed)
            {
                Debug.Log("Пак " + pack.packName + " закрыт, загружаю меню");
                MyMaze.Instance.MenuLoadAction();
            }
            else
            {
                //обновляем курсоры на уровень
                MyMaze.Instance.LastSelectedLevel = nextLevel;

                if (!packGroupFirstTimePassed)
                {
                    //загружаем следующий уровень
                    Debug.Log("Загружаю следующий уровень " + nextLevel.name);
                    MyMaze.Instance.LevelLoadAction(nextLevel);
                }
                else
                {
                    MyMaze.Instance.MenuLoadAction();
                }
            }
        }
    }

    /// <summary>
    /// Нажали кнопку рестарта уровня
    /// </summary>
    public void OnRestartRequest()
    {
        //Потратим жизнь
        if (MyMaze.Instance.Life.Use())
        {
            if (state == GameLevelStates.GameOver)
            {
                CGSwitcher.Instance.SetHideObject(uiResults);
                CGSwitcher.Instance.SetShowObject(uiGame);
                CGSwitcher.Instance.Switch();
            }
            addMoves = 0;
            state = GameLevelStates.Game;

            if (OnRestart != null)
                OnRestart();
        }
        else
        {
            uiAdsLife.Show();
        }
    }

    /// <summary>
    /// Симулируем нажатие кнопки выхода в меню
    /// </summary>
    void ExitToMenu()
    {
        InputSimulator.Instance.SimulateClick(GameObject.FindObjectOfType<LevelLoaderUI>().gameObject);
    }

    /// <summary>
    /// Игрок закончил ход
    /// </summary>
    /// <param name="move"></param>
    void OnPlayerMoveEnd(int move)
    {
        if (MovesLeft > 0)
            return;

        //ходы закончились
        if (!uiAdsMoves.TryForShow())
            OnRestartRequest();
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
        if (isReturnToMove)
            return;

        isReturnToMove = true;
        Player.Instance.DisableControl();
        StartCoroutine(ReturnToMoveNumerator(move));
    }

    IEnumerator ReturnToMoveNumerator(int move)
    {
        float playerFadeAnimationTime = 1f; // вермя исчезания и появления игрока
        yield return new WaitForSeconds(playerFadeAnimationTime);
        Player.Instance.EnableControl();
        if (OnReturnToMove != null)
            OnReturnToMove(move);
        yield return new WaitForSeconds(playerFadeAnimationTime);
        isReturnToMove = false;
    }
    
    void OnDestroy()
    {
        if (MyMaze.Instance != null)
        {
            MyMaze.Instance.OnMenuLoad -= OnMenuLoad;
            MyMaze.Instance.OnPackGroupFirstTimePassed -= OnPackGroupFirstTimePassed;
        }
    }
}
