using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
public class MoveEffector : GameLevelObject {
    public Directions direction;
    public float effectDelayTime = 0.065f;
    public GameObject Arrow
    {
        get
        {
            if (GetComponentInChildren<MoveEffectorArrow>() == null)
                return null;
            return GetComponentInChildren<MoveEffectorArrow>().gameObject;
        }
    }
    
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
        {
            StartCoroutine(EffectAfterDelay());
        }
    }

    IEnumerator EffectAfterDelay()
    {
        yield return new WaitForSeconds(effectDelayTime);
        Player.Instance.StopMoving(true);
        Player.Instance.MoveImpulse(direction, true);
        animator.SetTrigger("Action");
        GetComponent<SoundsPlayer>().PlayOneShootSound();
    }
}
