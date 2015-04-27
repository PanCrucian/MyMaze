using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class HiddenWall : GameLevelObject {

    public HiddenFill hiddenFill;
    public SpriteRenderer[] hideObjects;

    private int health;
    private int maxHealth = 2;
    private BoxCollider2D boxCollider;

    public override void Start()
    {
        base.Start();
        if (hiddenFill == null)
            Debug.LogWarning("HiddenWall не нашел декорацию для анимаций");
        if(animator.enabled)
            animator.enabled = false;
        health = maxHealth;
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
        {
            Player.Instance.StopMoving();

            /* почему-то заказчик решил только один раз показывать секретную звезду, 
            такой большой труд от всех веток разработки и на 1 раз... пфф идиот */
            Star hiddenStar = MyMaze.Instance.LastSelectedLevel.GetHiddenStar();
            if (hiddenStar == null)
                Debug.LogWarning("У уровня " + MyMaze.Instance.LastSelectedLevel.levelName + " нет секретной звезды!");
            else if (!hiddenStar.IsCollected)
                Hited();
        }
    }

    /// <summary>
    /// В стенку совершен удар
    /// </summary>
    void Hited()
    {
        health--;
        if (health == 1)
        {
            hiddenFill.CrackEasy();
        }
        if (health == 0)
        {
            HideObjects();
            boxCollider.enabled = false;
            hiddenFill.CrackHard();
        }
    }

    public override void Restart()
    {
        base.Restart();
        health = maxHealth;
        ShowObjects();
        boxCollider.enabled = true;
    }

    /// <summary>
    /// Спрятать объекты, типо уничтожены
    /// </summary>
    void HideObjects()
    {
        foreach (SpriteRenderer ren in hideObjects)
        {
            ren.enabled = false;
            Collider2D coll = ren.GetComponent<Collider2D>();
            if (coll != null)
                coll.enabled = false;
        }
    }

    /// <summary>
    /// Вернуть спрятанные объекты
    /// </summary>
    void ShowObjects()
    {
        foreach (SpriteRenderer ren in hideObjects)
        {
            ren.enabled = true;
            Collider2D coll = ren.GetComponent<Collider2D>();
            if (coll != null)
                coll.enabled = true;
        }
    }
}
