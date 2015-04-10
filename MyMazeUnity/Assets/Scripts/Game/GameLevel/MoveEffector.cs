using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class MoveEffector : GameLevelObject {
    public Directions direction;

    public GameObject Arrow
    {
        get
        {
            return GetComponentInChildren<MoveEffectorArrow>().gameObject;
        }
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
