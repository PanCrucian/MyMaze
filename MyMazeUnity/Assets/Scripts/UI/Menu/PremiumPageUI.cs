using UnityEngine;
using UnityEngine.UI;

public class PremiumPageUI : MonoBehaviour {

    private Button button;
    private ColorBlock tempColorBlock;

    void Start()
    {
        tempColorBlock = new ColorBlock();
        button = GetComponent<Button>();
        tempColorBlock = button.colors;
    }
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

    /// <summary>
    /// Выключим объект если купили премиум
    /// </summary>
    void DeactivateIfPremium() {
        //премиума больше нет так что всегда выключено
        gameObject.SetActive(false);
    }

#if UNITY_IPHONE
    void Update()
    {
        DeactivateIfPremium();
        if (MyMaze.Instance.AppStore.IsInitalized)
            EnableButton();
        else
            DisableButton();
    }
#else
    void Update()
    {
        DeactivateIfPremium();
        DisableButton();
    }
#endif
}
