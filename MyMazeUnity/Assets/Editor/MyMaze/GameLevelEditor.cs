using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GameLevel))]
public class GameLevelEditor : Editor {

    private bool draw;
    private GridObject drawObject;
    private Transform drawParent;
    private Color color = Color.white;

	public override void OnInspectorGUI()
    {
 	    DrawDefaultInspector();
        EditorGUILayout.Space();
        if(GUILayout.Button("Объекты к сетке")) {
            CorrectDraggablePosition();
        }
        if(Application.isPlaying)
            if (GUILayout.Button("Закончить уровень"))
                PickUpAllPyramids();

        draw = EditorGUILayout.Toggle("Рисовать мышкой", draw);
        drawObject = (GridObject)EditorGUILayout.ObjectField("Что рисуем", drawObject, typeof(GridObject), false);
        color = EditorGUILayout.ColorField("Цвет", color);
        drawParent = (Transform)EditorGUILayout.ObjectField("Где рисуем", drawParent, typeof(Transform), true);

        if (drawObject == null)
            drawObject = (GridObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/GameLevel/Decoration/Fill.prefab", typeof(GridObject));
        if (drawParent == null)
        {
            GameObject gameLevel = GameObject.Find("GameLevel");
            foreach (Transform t in gameLevel.transform)
            {
                if (t.name.Equals("Center"))
                {
                    foreach (Transform tt in t)
                    {
                        if (tt.name.Equals("Decorations"))
                        {
                            drawParent = tt;
                            break;
                        }
                    }
                    break;
                }
            }
        }
        if (color == Color.white)
        {
            if (GameObject.FindGameObjectWithTag("WallFill") != null)
                color = GameObject.FindGameObjectWithTag("WallFill").GetComponent<SpriteRenderer>().color;
        }
    }

    void OnSceneGUI()
    {
        if (Application.isPlaying)
            return;
        
        Event evt = Event.current;
        if (evt.type == EventType.keyDown)
        {
            if (evt.keyCode == KeyCode.LeftAlt)
            {
                draw = !draw;
                EditorUtility.SetDirty(target);
            }
            if(evt.keyCode == KeyCode.Space)
                CorrectDraggablePosition();
        }
        if (draw)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (!draw)
            return;

        if (drawObject == null || drawParent == null)
        {
            Debug.Log("Укажите что рисуем и где рисуем");
            return;
        }
        if (evt.button != 0)
            return;
        switch (evt.type)
        {
            case EventType.mouseDrag:
                GridDraggableObject[] gridObjects = drawParent.GetComponentsInChildren<GridDraggableObject>();
                if (gridObjects.Length <= 0)
                {
                    Debug.Log("Поставьте хотябы 1 объект GridDraggableObject на сцену");
                    return;
                }

                GridDraggableObject draggable = gridObjects[0];
                Vector2 mousePos = Event.current.mousePosition;
                mousePos.y = SceneView.GetAllSceneCameras()[0].pixelHeight - mousePos.y;
                Vector3 worldPosition = SceneView.GetAllSceneCameras()[0].ScreenPointToRay(mousePos).origin;

                Vector3 substractCoordinats = drawParent.localPosition;
                Transform parentTransform = drawParent.parent;
               
                while (parentTransform != null)
                {
                    substractCoordinats += parentTransform.localPosition;
                    parentTransform = parentTransform.parent;
                }
                GridObject.Position gridPosition = draggable.GetGridPosition(worldPosition - substractCoordinats);

                bool allowInstantiate = true;
                foreach (GridDraggableObject gridobject in gridObjects)
                {
                    if (gridobject.position.Equals(gridPosition))
                    {
                        allowInstantiate = false;
                        break;
                    }
                }

                if (allowInstantiate)
                {
                    GridObject newGridObject = (GridObject) PrefabUtility.InstantiatePrefab(drawObject as GridObject);
                    newGridObject.transform.parent = drawParent;
                    newGridObject.position = gridPosition.Clone();
                    newGridObject.GetComponent<GridDraggableObject>().UpdatePosition();
                    newGridObject.GetComponent<SpriteRenderer>().color = color;
                    EditorUtility.SetDirty(newGridObject);
                }

                break;
            default:
                break;
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

    void PickUpAllPyramids()
    {
        GameLevel gameLevel = (GameLevel)target;
        foreach (Pyramid pyramid in gameLevel.pyramids)
            pyramid.PickUp();
    }
}
