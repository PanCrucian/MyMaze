using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Spike : GameLevelObject {

    private SpriteRenderer spriteRenderer;
    private float hideTime;

    public override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (CheckForPlayer(coll.gameObject))
            Player.Instance.Die();
    }

    /// <summary>
    /// Спрятать шипы
    /// </summary>
    public void FadeOut(float hideTime)
    {
        this.hideTime = hideTime;
        StartCoroutine(FadeNumerator(false));
    }

    /// <summary>
    /// Показать шипы
    /// </summary>
    public void FadeIn(float hideTime)
    {
        this.hideTime = hideTime;
        StartCoroutine(FadeNumerator(true));
    }

    IEnumerator FadeNumerator(bool isShow)
    {
        float time = hideTime;
        for (float t = 0; t <= time; t += Time.deltaTime)
        {
            spriteRenderer.color = new Color(
                spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,
                isShow ? Mathf.Lerp(0f, 1f, (time - t) / time) : Mathf.Lerp(1f, 0, (time - t) / time)
                );
            yield return new WaitForEndOfFrame();
        }
        spriteRenderer.color = new Color(
                spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,
                isShow ? 0f : 1f
                );
    }
}
