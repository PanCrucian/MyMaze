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
    private bool allowReturn;
    private int returnToMove;
    private TimeMachineBooster timeMachine;

    void Start()
    {
        timeMachine = MyMaze.Instance.TimeMachineBooster;
        if (timeMachine.IsClosed) // && timeMachine.avaliableAtLevel != MyMaze.Instance.LastSelectedLevel для тестов
        {
            gameObject.SetActive(false);
            return;
        }
        button = GetComponent<Button>();
        Player.Instance.OnMoveEnd += OnPlayerMoveEnd;
    }

    void Update()
    {
        if (Player.Instance.MovesCount <= 0)
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
    public void OnPlayerMoveEnd(int move)
    {
        button.interactable = true;
    }

    /// <summary>
    /// Меняем состояние объектов на определенный ход
    /// </summary>
    public void ReturnToMove()
    {
        if (timeMachine.IsAvaliable(MyMaze.Instance.LastSelectedLevel))
        {
            GameLevel.Instance.ReturnToMoveRequest(Player.Instance.MovesCount - 1);
            button.interactable = false;
        }
        else
        {
            MyMaze.Instance.InApps.BuyPremium();
        }
    }
}
