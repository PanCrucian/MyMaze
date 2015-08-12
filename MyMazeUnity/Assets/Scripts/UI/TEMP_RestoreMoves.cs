using UnityEngine;
using System.Collections;

public class TEMP_RestoreMoves : MonoBehaviour {

    public void Restore()
    {
        GameLevel.Instance.AddFiveMoves();
        GameLevel.Instance.UnPause();
        AdsMovesUI adsMovesUI = GameObject.FindObjectOfType<AdsMovesUI>();
        CGSwitcher.Instance.SetHideObject(adsMovesUI.GetComponent<Animator>());
        CGSwitcher.Instance.Switch();
    }
}
