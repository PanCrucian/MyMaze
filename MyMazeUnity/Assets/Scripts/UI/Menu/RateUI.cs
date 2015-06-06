using UnityEngine;
using System.Collections;

public class RateUI : MonoBehaviour {

    public int toMarketStarsGreatherOrEqual = 4;

    /// <summary>
    /// Игрок пытается оценить игру
    /// </summary>
    /// <param name="rate">От 1 до 5</param>
    public void OnRateRequest(int rate)
    {
        if(rate == 0) {
            Hide();
            return;
        }
        if (rate >= toMarketStarsGreatherOrEqual)
        {
            Debug.Log("Игра оценена на хорошое количество звезд. Идем в маркет и восстановим энергию");
            MyMaze.Instance.Energy.RestoreAllUnits();
            MyMaze.Instance.GoToMarket();
        }
        Hide();
    }

    /// <summary>
    /// Закрывает окно с оценкой игры
    /// </summary>
    void Hide()
    {
        CGSwitcher.Instance.SetHideObject(GetComponent<Animator>());
        CGSwitcher.Instance.Switch();
    }
}
