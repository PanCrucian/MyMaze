using UnityEngine;
using System.Collections;

public class ButtonPlayNextUI : MonoBehaviour {

    public static ButtonPlayNextUI Instance
    {
        get
        {
            return _instance;
        }
    }
    private static ButtonPlayNextUI _instance;

    void Awake()
    {
        _instance = this;
    }

    public void OnNextLevelRequest()
    {
        GameLevel.Instance.OnNextLevelRequest();
    }
}
