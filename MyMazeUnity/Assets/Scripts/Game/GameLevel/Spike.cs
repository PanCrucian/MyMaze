using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Spike : GameLevelObject {

    public override void Start()
    {
        base.Start();
        StartCoroutine(IdleNumerator());
    }

    IEnumerator IdleNumerator()
    {
        yield return new WaitForSeconds(Random.Range(3,15));
        animator.SetTrigger("Idle");
        StartCoroutine(IdleNumerator());
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
            Player.Instance.Die();
    }
}
