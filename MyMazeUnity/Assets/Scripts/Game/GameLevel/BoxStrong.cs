using UnityEngine;
using System.Collections;

public class BoxStrong : GameLevelObject {

    private Box boxWhite;
    private Box boxYellow;

    public override void Start()
    {
        base.Start();
        animator.enabled = false;
        FindBoxes();        
        boxYellow.OnPlayerEnter += OnPlayerEnter;
        StartCoroutine(RestartNumerator());
    }

    /// <summary>
    /// Врезался в желтый ящик
    /// </summary>
    void OnPlayerEnter()
    {
        boxWhite.boxCollider.enabled = true;
    }

    /// <summary>
    /// Ищем коробки в детишках
    /// </summary>
    void FindBoxes()
    {
        Box[] boxes = GetComponentsInChildren<Box>();
        foreach (Box box in boxes)
        {
            switch (box.color)
            {
                case BoxColors.White: boxWhite = box; break;
                case BoxColors.Yellow: boxYellow = box; break;
            }

            if (box.boxCollider == null)
                box.boxCollider = box.GetComponent<BoxCollider2D>();
        }
    }

    public override void Restart()
    {
        base.Restart();
        StartCoroutine(RestartNumerator());
    }

    /// <summary>
    /// В следующем кадре чтобы выполнилось после Restart детишек
    /// </summary>
    /// <returns></returns>
    IEnumerator RestartNumerator() {
        yield return new WaitForEndOfFrame();
        boxWhite.boxCollider.enabled = false;
        boxYellow.boxCollider.enabled = true;
    }

}
