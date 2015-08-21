using UnityEngine;
using System.Collections;

public class TimeMachineBooster : Booster, ISavingElement {
    public Level[] levelsWithoutPremium;

    public override void Start()
    {
        base.Start();
        MyMaze.Instance.InApps.OnTimeMachineBuyed += OnTimeMachineBuyed;
    }

    /// <summary>
    /// Купили бустер машина времени
    /// </summary>
    void OnTimeMachineBuyed()
    {
        if (IsClosed)
            this.Open();
    }

    /// <summary>
    /// Доступен ли бустер для использования
    /// </summary>
    /// <returns></returns>
    public bool IsAvaliable(Level level)
    {
        if (MyMaze.Instance.InApps.IsOwned(ProductTypes.BoosterTimeMachine))
            return true;

        foreach (Level l in levelsWithoutPremium)
        {
            if (l.name.Equals(level.name))
                return true;
        }

        return false;
    }

    public override void Open()
    {
        base.Open();
        MyMaze.Instance.Tutorial.GetStep(TutorialPhase.TimeMachine).Start();
    }

    public void Save()
    {
        PlayerPrefs.SetInt("TimeMachine#IsClosed", System.Convert.ToInt32(IsClosed));
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("TimeMachine#IsClosed"))
            if (!System.Convert.ToBoolean(PlayerPrefs.GetInt("TimeMachine#IsClosed")))
                base.Open();
    }

    public void ResetSaves()
    {
        throw new System.NotImplementedException();
    }
}
