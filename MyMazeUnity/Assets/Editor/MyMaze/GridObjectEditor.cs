using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GridObject))]
[CanEditMultipleObjects]
public class GridObjectEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GridObject obj = (GridObject)target;
        if (Application.isPlaying)
            return;
        obj.UpdatePositionVars();
        EditorUtility.SetDirty(obj);
    }
}