using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class HiddenStar : GameLevelObject {

    private BoxCollider2D boxCollider;

    public override void Start()
    {
        base.Start();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
        {
            PickUp();
        }
    }

    /// <summary>
    /// Подбираем секретную звезду
    /// </summary>
    void PickUp()
    {
        Star hiddenStar = MyMaze.Instance.LastSelectedLevel.GetHiddenStar();
        if (hiddenStar == null)
            Debug.LogWarning("У уровня " + MyMaze.Instance.LastSelectedLevel.levelName + " нет секретной звезды!");
        else
            hiddenStar.Collect();

        HiddenStarUI hiddenStarUI = GameObject.FindObjectOfType<HiddenStarUI>();
        if (hiddenStarUI == null)
            Debug.LogWarning("Не могу найти UI элемент для секретной звезды");
        else
            hiddenStarUI.Show();

        animator.SetTrigger("PickUp");
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y,
            Player.Instance.transform.localPosition.z - 1f
            );
        boxCollider.enabled = false;
    }

    public override void Restart()
    {
        base.Restart();
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("HiddenStarIdle"))
            animator.SetTrigger("Reset");
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y,
            1
            );
        boxCollider.enabled = true;
    }
}
