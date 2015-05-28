using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class Trampoline : GameLevelObject {

    public int jumpCellDistance = 2;

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
        {
            Player.Instance.Jump(this);
            animator.SetTrigger("Action");
            GetComponent<SoundsPlayer>().PlayOneShootSound();
        }
    }
}
