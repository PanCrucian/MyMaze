using UnityEngine;
using System.Collections;
using Heyzap;

public class AndroidBackButton : MonoBehaviour {
    
#if UNITY_ANDROID || UNITY_EDITOR
    MenuLoadConfirmUI menuLoadConfirm;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HeyzapAds.OnBackPressed();

            bool backBtnFound = false;
            BackButtonUI[] backBtns = GameObject.FindObjectsOfType<BackButtonUI>();
            foreach (BackButtonUI btn in backBtns)
                if (btn.isActiveAndEnabled)
                {
                    backBtnFound = true;
                    InputSimulator.Instance.SimulateClick(btn.gameObject);
                    break;
                }
            if (!backBtnFound)
            {
                if (GameLevel.Instance == null)
                {
                    GameObject cg_Main = GameObject.Find("cg_Main");
                    if(cg_Main != null)
                    {
                        if (cg_Main.activeSelf)
                        {
                            if(menuLoadConfirm == null)
                                menuLoadConfirm = GameObject.FindObjectOfType<MenuLoadConfirmUI>();

                            if (menuLoadConfirm != null)
                            {
                                menuLoadConfirm.Show();
                                menuLoadConfirm.OnPositive = () => Application.Quit();
                            }
                        }
                    }

                }
                else
                {
                    InputSimulator.Instance.SimulateClick(GameObject.Find("b_Menu"));
                }
            }
        }
    }
#endif
}
