using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LockPosition))]
[CanEditMultipleObjects]
public class LockPositionEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LockPosition lp = (LockPosition)target;
        if (Application.isPlaying)
            return;
        if (lp.doLock)
            lp.Lock();
        else
            lp.SetLockPosition();
    }
}