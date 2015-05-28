using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GameLevel))]
public class GameLevelEditor : Editor {

	public override void OnInspectorGUI()
    {
 	    DrawDefaultInspector();
        EditorGUILayout.Space();
        if(GUILayout.Button("Объекты к сетке") || Input.GetKeyDown(KeyCode.CapsLock)) {
            CorrectDraggablePosition();
        }
        if(Application.isPlaying)
            if (GUILayout.Button("Закончить уровень"))
                PickUpAllPyramids();
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

    void PickUpAllPyramids()
    {
        GameLevel gameLevel = (GameLevel)target;
        foreach (Pyramid pyramid in gameLevel.pyramids)
            pyramid.PickUp();
    }
}
