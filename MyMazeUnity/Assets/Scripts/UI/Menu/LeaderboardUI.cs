using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LeaderboardUI : MonoBehaviour, IPointerClickHandler
{
    private Button button;
    private ColorBlock tempColorBlock;

    void Start()
    {
        tempColorBlock = new ColorBlock();
        button = GetComponent<Button>();
        tempColorBlock = button.colors;
    }

#if UNITY_IPHONE
    void Update()
    {
        if (MyMaze.Instance.GameCenter.IsAuth)
            EnableButton();
        else
            DisableButton();
    }
#elif UNITY_ANDROID
    void Update()
    {
        if (MyMaze.Instance.GooglePlayServices.IsAuth && MyMaze.Instance.GooglePlayServices.IsLeaderboardsLoaded)
            EnableButton();
        else
            DisableButton();
    }
#else
    void Update()
    {
        DisableButton();
    }
#endif

    void EnableButton()
    {
        tempColorBlock.disabledColor = tempColorBlock.normalColor;
        button.interactable = true;
        button.colors = tempColorBlock;
    }

    void DisableButton()
    {
        tempColorBlock.disabledColor = tempColorBlock.pressedColor;
        button.interactable = false;
        button.colors = tempColorBlock;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
#if UNITY_IPHONE
        GameCenterManager.ShowLeaderboard(MyMaze.Instance.Leaderboards.GetMarketId(LeaderboardTypes.Stars));
#endif
#if UNITY_ANDROID
        if (MyMaze.Instance.GooglePlayServices.IsAuth && MyMaze.Instance.GooglePlayServices.IsLeaderboardsLoaded)
            GooglePlayManager.Instance.ShowLeaderBoardsUI();

        if (!MyMaze.Instance.GooglePlayServices.IsAuth)
            GooglePlayConnection.Instance.Connect();
#endif
    }
}
