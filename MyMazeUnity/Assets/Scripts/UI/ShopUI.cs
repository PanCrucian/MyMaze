using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopUI : GentleMonoBeh {

    public Animator showUI;

    public Button removeAdsBtn;
    public Button boostTimeBtn;
    public Button livesUnlmBtn;
    public string iconName = "i_Icon";
    public string ownedName = "i_Owned";

    void Start()
    {
        SetGentleCPURate(30);
    }

    public override void GentleUpdate()
    {
        base.GentleUpdate();

        if (MyMaze.Instance.InApps.IsOwned(ProductTypes.NoAds))
            SetUIOwnedForButton(removeAdsBtn);
        else
            SetUINotOwnedForButton(removeAdsBtn);

        if (MyMaze.Instance.InApps.IsOwned(ProductTypes.BoosterTimeMachine))
            SetUIOwnedForButton(boostTimeBtn);
        else
            SetUINotOwnedForButton(boostTimeBtn);

        if (MyMaze.Instance.InApps.IsOwned(ProductTypes.UnlimitedLives))
            SetUIOwnedForButton(livesUnlmBtn);
        else
            SetUINotOwnedForButton(livesUnlmBtn);
    }

    /// <summary>
    /// переключим кнопки магазина в "приобретенное" состояние
    /// </summary>
    /// <param name="button"></param>
    void SetUIOwnedForButton(Button button)
    {
        Image icon = button.transform.FindChild(iconName).GetComponent<Image>();
        Image owned = button.transform.FindChild(ownedName).GetComponent<Image>();
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 0f);
        owned.color = new Color(owned.color.r, owned.color.g, owned.color.b, 1f);
        button.interactable = false;
    }

    /// <summary>
    /// переключим кнопки магазина в "НЕ приобретенное" состояние
    /// </summary>
    /// <param name="button"></param>
    void SetUINotOwnedForButton(Button button)
    {
        Image icon = button.transform.FindChild(iconName).GetComponent<Image>();
        Image owned = button.transform.FindChild(ownedName).GetComponent<Image>();
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1f);
        owned.color = new Color(owned.color.r, owned.color.g, owned.color.b, 0f);
        button.interactable = true;
    }

    /// <summary>
    /// Нажали кнопку возврата в предыдущее меню
    /// </summary>
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
        GentleUpdate();
    }
    
    /// <summary>
    /// Нажали кнопку открыть бустер возврата на ход
    /// </summary>
    public void OnBoostButton()
    {
        ProductTypes productType = ProductTypes.BoosterTimeMachine;
        if (!MyMaze.Instance.InApps.IsOwned(productType))
            MyMaze.Instance.InApps.BuyRequest(productType);
        GentleUpdate();
    }

    /// <summary>
    /// Нажали кнопку бесконечные жизни
    /// </summary>
    public void OnUnlimitedLivesButton()
    {
        ProductTypes productType = ProductTypes.UnlimitedLives;
        if (!MyMaze.Instance.InApps.IsOwned(productType))
            MyMaze.Instance.InApps.BuyRequest(productType);
        GentleUpdate();
    }

    /// <summary>
    /// Нажали кнопку восстановления покупок
    /// </summary>
    public void OnRestoreIAPsButton()
    {
        MyMaze.Instance.InApps.RestoreIAPs();
    }
}
