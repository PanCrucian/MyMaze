using UnityEngine;
using System.Collections;

public class GridDraggableObject : GridObject {

    public void UpdatePosition()
    {
        transform.localPosition = new Vector3(
            position.xCell * gridStep.x,
            position.yRow * gridStep.y,
            transform.localPosition.z
            );
    }
	
}
