using UnityEngine;
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
        {
            effector.direction = Directions.Up;
            EditorUtility.SetDirty(effector);
        }

        if (GUILayout.Button("--> Вправо"))
        {
            effector.direction = Directions.Right;
            EditorUtility.SetDirty(effector);
        }

        if (GUILayout.Button("V Вниз"))
        {
            effector.direction = Directions.Down;
            EditorUtility.SetDirty(effector);
        }
        if (GUILayout.Button("<-- Влево"))
        {
            effector.direction = Directions.Left;
            EditorUtility.SetDirty(effector);
        }
    }
}