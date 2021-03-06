﻿using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MoveEffector))]
[CanEditMultipleObjects]
public class MoveEffectorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MoveEffector effector = (MoveEffector)target;
        
        if (effector.Arrow == null)
            return;
        switch (effector.direction)
        {
            case Directions.Up:
                effector.Arrow.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            case Directions.Right:
                effector.Arrow.transform.localRotation = Quaternion.Euler(0, 0, -90);
                break;
            case Directions.Down:
                effector.Arrow.transform.localRotation = Quaternion.Euler(0, 0, -180);
                break;
            case Directions.Left:
                effector.Arrow.transform.localRotation = Quaternion.Euler(0, 0, -270);
                break;
        }

        if (GUILayout.Button("^ Наверх"))
            effector.direction = Directions.Up;

        if (GUILayout.Button("--> Вправо"))
            effector.direction = Directions.Right;

        if (GUILayout.Button("V Вниз"))
            effector.direction = Directions.Down;

        if (GUILayout.Button("<-- Влево"))
            effector.direction = Directions.Left;
        if(!Application.isPlaying)
            EditorUtility.SetDirty(effector);
    }
}