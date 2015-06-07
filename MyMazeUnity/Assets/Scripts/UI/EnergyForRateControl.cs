using UnityEngine;
using System.Collections;

public class EnergyForRateControl : MonoBehaviour {

    public Animator RateUI;

    void Start()
    {
        MyMaze.Instance.Energy.OnEnergyEmpty += OnEnergyEmpty;
    }

    void OnEnergyEmpty()
    {
        if (!MyMaze.Instance.Energy.WasEnergyEmptyBeforeInMenu())
        {
            CGSwitcher.Instance.SetShowObject(RateUI);
            CGSwitcher.Instance.Switch();
        }
    }

    void OnDestroy()
    {
        if (MyMaze.Instance == null)
            return;
        MyMaze.Instance.Energy.OnEnergyEmpty -= OnEnergyEmpty;
    }
}
