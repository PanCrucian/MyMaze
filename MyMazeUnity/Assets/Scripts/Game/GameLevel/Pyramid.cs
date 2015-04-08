using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Pyramid : GameLevelObject
{
    public Deligates.PyramidEvent OnPyramidPickUp;

    /// <summary>
    /// Подобрана ли пирамида
    /// </summary>
    public bool IsUsed
    {
        get
        {
            return _isUsed;
        }
    }

    private bool _isUsed;
    private BoxCollider2D boxCollider;

    public override void Start()
    {
        base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Подобрать пирамиду
    /// </summary>
    public void PickUp()
    {
        _isUsed = true;
        boxCollider.enabled = false;
        animator.SetBool("PickUp", true);

        if (OnPyramidPickUp != null)
            OnPyramidPickUp(this);
    }

    /// <summary>
    /// Что-то вошло в зону тригера
    /// </summary>
    /// <param name="coll"></param>
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag.Equals("Player"))
        {
            PickUp();
        }
    }

    /// <summary>
    /// Уровень перезапустился
    /// </summary>
    public override void Restart()
    {
        base.Restart();
        animator.SetBool("PickUp", false);
        boxCollider.enabled = true;
        _isUsed = false;
    }
}
