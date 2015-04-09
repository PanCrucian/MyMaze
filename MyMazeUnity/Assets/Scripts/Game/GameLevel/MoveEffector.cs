using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class MoveEffector : GameLevelObject {
    public Directions direction;

    private CircleCollider2D collider2D;
    public GameObject Arrow
    {
        get
        {
            return GetComponentInChildren<MoveEffectorArrow>().gameObject;
        }
    }

    public override void Start()
    {
        base.Start();
        collider2D = GetComponent<CircleCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
        {
            Player.Instance.StopMoving();
            Player.Instance.MoveImpulse(direction);
            animator.SetTrigger("Action");
        }
    }
}
