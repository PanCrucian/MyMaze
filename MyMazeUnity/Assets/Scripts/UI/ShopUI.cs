using UnityEngine;
using System.Collections;

public class ShopUI : MonoBehaviour {

    public Animator showUI;

    public void OnBackButton()
    {
        CGSwitcher.Instance.SetHideObject(GetComponent<Animator>());
        CGSwitcher.Instance.SetShowObject(showUI);
        CGSwitcher.Instance.Switch();
    }

    /// <summary>
    /// Нажали кнопку без рекламы
    /// </summary>
    public void OnNoAdsButton()
    {
        ProductTypes productType = ProductTypes.NoAds;
        if (!MyMaze.Instance.InApps.IsOwned(productType))
            MyMaze.Instance.InApps.BuyRequest(productType);
    }
    
    /// <summary>
    /// Нажали кнопку открыть бустер возврата на ход
    /// </summary>
    public void OnBoostButton()
    {
        ProductTypes productType = ProductTypes.BoosterTimeMachine;
        if (!MyMaze.Instance.InApps.IsOwned(productType))
            MyMaze.Instance.InApps.BuyRequest(productType);
    }

    /// <summary>
    /// Нажали кнопку бесконечные жизни
    /// </summary>
    public void OnUnlimitedLivesButton()
    {
        ProductTypes productType = ProductTypes.UnlimitedLives;
        if (!MyMaze.Instance.InApps.IsOwned(productType))
            MyMaze.Instance.InApps.BuyRequest(productType);
    }

    /// <summary>
    /// Нажали кнопку восстановления покупок
    /// </summary>
    public void OnRestoreIAPsButton()
    {
        MyMaze.Instance.InApps.RestoreIAPs();
    }
}
