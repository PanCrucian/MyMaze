using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MovingWall))]
[CanEditMultipleObjects]
public class MovingWallEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MovingWall wall = (MovingWall)target;
        if (wall.draggable == null)
            wall.draggable = wall.GetComponent<GridDraggableObject>();
        if (GUILayout.Button("Установить как точку прибытия"))
        {
            wall.toPosition = new GridObject.Position() { 
                xCell = wall.draggable.position.xCell,
                yRow = wall.draggable.position.yRow
            };
            EditorUtility.SetDirty(wall);
        }
    }
}