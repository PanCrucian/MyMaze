using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

    public Vector2 gridStep
    {
        get
        {
            return _gridStep;
        }
    }

    private Vector2 _gridStep = new Vector2(0.62f, 0.62f);
}
