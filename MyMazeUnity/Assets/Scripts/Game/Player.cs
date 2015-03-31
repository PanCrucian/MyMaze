using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private Rigidbody2D rigidbody2d;
    private GridDraggableObject draggable;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        draggable = GetComponent<GridDraggableObject>();
    }
}
