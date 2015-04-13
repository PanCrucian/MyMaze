using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Box : GameLevelObject
{
    public Deligates.SimpleEvent OnPlayerEnter;

    public BoxColors color;
    [HideInInspector]
    public BoxCollider2D boxCollider;

	public override void Start () {
        base.Start();
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();
	}

    public override void Restart()
    {
        base.Restart();
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
            animator.SetTrigger("Reset");

        boxCollider.enabled = true;
    }

    void FadeOut()
    {
        boxCollider.enabled = false;
        switch (color)
        {
            case BoxColors.White: animator.SetTrigger("FadeWhite"); break;
            case BoxColors.Yellow: animator.SetTrigger("FadeYellow"); break;
        }
    }

    /// <summary>
    /// Если что-то врезалось в кубик
    /// </summary>
    /// <param name="coll"></param>
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
        {
            Player.Instance.StopMoving();
            if (OnPlayerEnter != null)
                OnPlayerEnter();
            FadeOut();
        }
    }
}
