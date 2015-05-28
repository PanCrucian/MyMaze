using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DragZoneActor : MonoBehaviour {

    public void Drag(BaseEventData data)
    {
        GameLevel.Instance.Drag(data);
    }

    public void PointerDownRequest(BaseEventData data)
    {
        GameLevel.Instance.PointerDownRequest(data);
    }

    public void PointerUpRequest(BaseEventData data)
    {
        GameLevel.Instance.PointerUpRequest(data);
    }
}
