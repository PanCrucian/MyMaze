using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GameLevel))]
public class GameLevelEditor : Editor {

	public override void OnInspectorGUI()
    {
 	    DrawDefaultInspector();
        EditorGUILayout.Space();
        if(GUILayout.Button("Объекты к сетке")) {
            CorrectDraggablePosition();
        }
    }

    void CorrectDraggablePosition() {
        GameLevel gameLevel = (GameLevel) target;
        GridDraggableObject[] draggables = gameLevel.transform.GetComponentsInChildren<GridDraggableObject>();
        foreach(GridDraggableObject draggable in draggables) {
            draggable.UpdatePositionVars();
            draggable.UpdatePosition();
            EditorUtility.SetDirty(draggable);
        }
    }
}
