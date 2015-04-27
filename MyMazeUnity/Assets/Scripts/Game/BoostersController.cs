using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MyMaze))]
public class BoostersController : MonoBehaviour {

    public List<Booster> boosters;

    /// <summary>
    /// Ищем бустер
    /// </summary>
    /// <param name="boosterType">Тип бустера</param>
    /// <returns></returns>
    public Booster GetBooster(BoosterTypes boosterType)
    {
        return boosters.Find(booster => booster.type == boosterType);
    }
}
