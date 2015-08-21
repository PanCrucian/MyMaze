using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeMachineUI : MonoBehaviour {

    [System.Serializable]
    public class ColorsSettings
    {
        public ColorBlock normal;
        public ColorBlock disabled;
    }

    public ColorsSettings colorsSettings;

    private Button button;
    private int returnToMove;
    private TimeMachineBooster timeMachine;
    private int allowReturnCounter;
    private int maxReturns = 3;

    void Start()
    {
        allowReturnCounter = maxReturns;
        timeMachine = MyMaze.Instance.TimeMachineBooster;
        timeMachine.OnOpen += OnBoosterOpen;
        if (timeMachine.IsClosed) // && timeMachine.avaliableAtLevel != MyMaze.Instance.LastSelectedLevel для тестов
        {
            gameObject.SetActive(false);
            return;
        }
        SetupIfBoosterOpened();
    }

    void OnDestroy()
    {
        timeMachine.OnOpen -= OnBoosterOpen;
    }

    void SetupIfBoosterOpened()
    {
        button = GetComponent<Button>();
        Player.Instance.OnMoveEnd += OnPlayerMoveEnd;
        GameLevel.Instance.OnReturnToMove += OnReturnToMove;
    }

    void OnBoosterOpen()
    {
        gameObject.SetActive(true);
        SetupIfBoosterOpened();
    }

    void Update()
    {
        if (Player.Instance.MovesCount <= 0)
            button.interactable = false;
        else if (allowReturnCounter > 0)
            button.interactable = true;
        else
            button.interactable = false;

        if (MyMaze.Instance.TimeMachineBooster.IsAvaliable(MyMaze.Instance.LastSelectedLevel))
            button.colors = colorsSettings.normal;
        else
            button.colors = colorsSettings.disabled;
    }

    /// <summary>
    /// Игрок закончил перемещение
    /// </summary>
    /// <param name="move"></param>
    void OnPlayerMoveEnd(int move)
    {
        if (allowReturnCounter + 1 <= maxReturns)
            allowReturnCounter += 1;
    }

    /// <summary>
    /// Игрок вернулся на ход
    /// </summary>
    /// <param name="move"></param>
    void OnReturnToMove(int move)
    {
        DecrimentAllowReturnCounter();
    }

    /// <summary>
    /// Меняем состояние объектов на определенный ход
    /// </summary>
    public void ReturnToMove()
    {
        if (timeMachine.IsAvaliable(MyMaze.Instance.LastSelectedLevel))
        {
            GameLevel.Instance.ReturnToMoveRequest(Player.Instance.MovesCount - 1);
            //button.interactable = false;
        }
        else
        {
            MyMaze.Instance.InApps.BuyRequest(ProductTypes.BoosterTimeMachine);
        }
    }

    void DecrimentAllowReturnCounter()
    {
        if (allowReturnCounter - 1 >= 0)
            allowReturnCounter -= 1;
    }
}
