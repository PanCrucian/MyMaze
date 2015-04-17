using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class HiddenStarUI : MonoBehaviour {

    /// <summary>
    /// Проигрываем анимацию подбора секретной звезды
    /// </summary>
    public void Show()
    {
        GetComponent<Animator>().SetTrigger("Play");
    }
}
