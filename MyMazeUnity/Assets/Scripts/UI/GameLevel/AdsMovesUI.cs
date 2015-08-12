using UnityEngine;
using System.Collections;

public class AdsMovesUI : MonoBehaviour {


    private bool isFirstShowUp = true;
    
    /// <summary>
    /// Попытаться показать экран с просмотром рекламы и восстановления сердец
    /// </summary>
    /// <returns></returns>
    public bool TryForShow()
    {
        if (!isFirstShowUp)
            return false;
        isFirstShowUp = false;
        GameLevel.Instance.Pause();
        CGSwitcher.Instance.SetShowObject(GetComponent<Animator>());
        CGSwitcher.Instance.Switch();
        GetComponent<SoundsPlayer>().PlayOneShootSound();
        return true;
    }
}
