using UnityEngine;
using System.Collections;

public class AndroidBackButton : MonoBehaviour {
    
#if UNITY_ANDROID || UNITY_EDITOR
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackButtonUI[] backBtns = GameObject.FindObjectsOfType<BackButtonUI>();
            foreach (BackButtonUI btn in backBtns)
                if (btn.isActiveAndEnabled)
                {
                    InputSimulator.Instance.SimulateClick(btn.gameObject);
                    break;
                }
        }
    }
#endif
}
