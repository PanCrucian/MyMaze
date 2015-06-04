using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GridDraggableObject))]
[CanEditMultipleObjects]
public class GridDraggableObjectEditor : Editor {

    public GridDraggableObject draggable;
    bool mouseHook;



    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        draggable = (GridDraggableObject)target;
        if (Application.isPlaying)
            return;
        draggable.UpdatePositionVars();
        if (!mouseHook)
            draggable.UpdatePosition();
        EditorUtility.SetDirty(draggable);
    }

    void OnSceneGUI()
    {
        if (Application.isPlaying)
            return;
        Event evt = Event.current;
        if (evt.button != 0)
            return;
        switch (evt.type)
        {
            case EventType.mouseUp:
                draggable.UpdatePosition();
                mouseHook = false;
                break;
            case EventType.mouseDrag:
                mouseHook = true;             
                break;
            default:
                break;
        }
    }
}
