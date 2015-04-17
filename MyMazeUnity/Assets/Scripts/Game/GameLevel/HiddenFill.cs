using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class HiddenFill : GameLevelObject {

    private BoxCollider2D boxCollider;

    public override void Start()
    {
        base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider.enabled)
            boxCollider.enabled = false;
    }

    /// <summary>
    /// Стенка поломалась чуть чуть
    /// </summary>
    public void CrackEasy()
    {
        animator.SetTrigger("Crack");
    }

    /// <summary>
    /// Стенка полностью сломана
    /// </summary>
    public void CrackHard()
    {
        animator.SetTrigger("Destroy");
    }

    public override void Restart()
    {
        base.Restart();
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
            animator.SetTrigger("Reset");
    }
}
